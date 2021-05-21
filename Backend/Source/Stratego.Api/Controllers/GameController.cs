using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using Stratego.Api.Models;
using Stratego.AppLogic.Contracts;
using Stratego.AppLogic.Dto;
using Stratego.Common;
using Stratego.Domain.BoardDomain;

namespace Stratego.Api.Controllers
{
    /// <summary>
    /// Provides game-play functionality.
    /// </summary>
    [Route("api/[controller]")]
    public class GameController : ApiControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Gets information about your game (your army, positions of the opponents army, who's turn is it?, is the game ready to start?).
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PlayerGameDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult GetPlayerGame(Guid id)
        {
            PlayerGameDto playerGameDto = _gameService.GetPlayerGameDto(id, UserId);
            return Ok(playerGameDto);
        }

        /// <summary>
        /// Get information about the game board (size, squares, obstacles, home territories)
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        [HttpGet("{id}/board")]
        [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult GetBoard(Guid id)
        {
            BoardDto boardDto = _gameService.GetBoardDto(id);
            return Ok(boardDto);
        }

        /// <summary>
        /// Position a piece on the board in the game setup phase.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="model">Info about where the piece should be placed.</param>
        [HttpPost("{id}/position-piece")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult PositionPiece(Guid id, [FromBody] MoveModel model)
        {
            Result result = _gameService.PositionPiece(id, model.PieceId, UserId, model.TargetCoordinate);
            if (result.IsFailure)
            {
                return BadRequest(new ErrorModel(result.Message));
            }
            return Ok();
        }

        /// <summary>
        /// Marks the player as 'Ready'.
        /// Should be called after the army of the player is positioned (otherwise a bad request will be returned).
        /// When both players are ready the game will be 'Started'.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        [HttpPost("{id}/ready")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult SetPlayerReady(Guid id)
        {
            _gameService.SetPlayerReady(id, UserId);
            return Ok();
        }

        /// <summary>
        /// Move a piece on the board after the game has been started.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="model">Info about where the piece should be moved to.</param>
        [HttpPost("{id}/move-piece")]
        [ProducesResponseType(typeof(MoveDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult MovePiece(Guid id, [FromBody] MoveModel model)
        {
            Result<MoveDto> result = _gameService.MovePiece(id, model.PieceId, UserId, model.TargetCoordinate);
            if (result.IsFailure)
            {
                return BadRequest(new ErrorModel(result.Message));
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Get information about the last move that was made in the game.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        [HttpGet("{id}/last-move")]
        [ProducesResponseType(typeof(MoveDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetLastMove(Guid id)
        {
            MoveDto move = _gameService.GetLastMove(id);
            if (move == null)
            {
                return NotFound();
            }
            return Ok(move);
        }

        /// <summary>
        /// Retrieves the possible coordinates to which a piece can be moved
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="pieceId">Id (guid) of the piece</param>
        [HttpGet("{id}/pieces/{pieceId}/possible-targets")]
        [ProducesResponseType(typeof(IList<BoardCoordinate>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPossibleTargets(Guid id, Guid pieceId)
        {
            Result<IList<BoardCoordinate>> result = _gameService.GetPossibleTargets(id, UserId, pieceId);
            if (result.IsFailure)
            {
                return BadRequest(new ErrorModel(result.Message));
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Upload a file that contains army positions that should be used for a game (for the logged in user).
        /// The army will be positioned according to the specifications in the file.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="file">The uploaded file</param>
        /// <remarks>This method is needed for an EXTRA. Not needed to implement the minimal requirements.</remarks>
        [HttpPost("{id}/upload-army-positions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult UploadArmyPositions(Guid id, IFormFile file)
        {
            using (Stream fileStream = file.OpenReadStream())
            {
                Result result = _gameService.LoadArmyPositions(id, UserId, fileStream);
                if (result.IsFailure)
                {
                    return BadRequest(new ErrorModel(result.Message));
                }
            }
            return Ok();
        }

        /// <summary>
        /// Download a file that contains the positions of the army of the logged in user.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <remarks>This method is needed for an EXTRA. Not needed to implement the minimal requirements.</remarks>
        [HttpPost("{id}/download-army-positions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult DownloadArmyPositions(Guid id)
        {
            Result<DownloadDto> result = _gameService.GetArmyPositionsDownload(id, UserId);
            if (result.IsFailure)
            {
                return BadRequest(new ErrorModel(result.Message));
            }
            return File(result.Value.Contents, "text/plain", result.Value.FileDownloadName);
        }

        /// <summary>
        /// Randomly positions all pieces of the army of the player
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <remarks>This method is needed for an EXTRA. Not needed to implement the minimal requirements.</remarks>
        [HttpPost("{id}/position-all-pieces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult PositionAllPieces(Guid id)
        {
            //TODO
            Result result = Result.CreateSuccessResult(); //TODO: call a method "PositionAllPieces" from IGameService that returns a Result
            if (result.IsFailure)
            {
                return BadRequest(new ErrorModel(result.Message));
            }
            return Ok();
        }
    }
}
