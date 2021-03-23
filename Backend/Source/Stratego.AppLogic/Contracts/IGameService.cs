using System;
using System.Collections.Generic;
using System.IO;
using Stratego.AppLogic.Dto;
using Stratego.Common;
using Stratego.Domain;
using Stratego.Domain.BoardDomain;

namespace Stratego.AppLogic.Contracts
{
    /// <summary>
    /// Service to manipulate all the games in the application.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Creates a new game for 2 human players (users).
        /// </summary>
        /// <param name="settings">The settings that should be used when creating the game.</param>
        /// <returns>The Guid that identifies the created game.</returns>
        Guid CreateGameForUsers(User user1, User user2, GameSettings settings);

        /// <summary>
        /// Retrieves the board of a game and transforms it to a dto (data transfer object) that can be send in json-format.
        /// </summary>
        /// <param name="gameId">The identifier of the game.</param>
        BoardDto GetBoardDto(Guid gameId);

        /// <summary>
        /// Finds a game and then transforms the game to a dto (data transfer object) that contains info about the game from the perspective of a certain player.
        /// </summary>
        /// <param name="gameId">The identifier of the game.</param>
        /// <param name="playerId">The identifier of the player (usually the identifier of a user).</param>
        PlayerGameDto GetPlayerGameDto(Guid gameId, Guid playerId);

        /// <summary>
        /// Finds a game and then tries to position a piece on the board.
        /// This method is used when the game is not started yet and the players are positioning their army pieces on the board.
        /// </summary>
        /// <param name="gameId">The identifier of the game.</param>
        /// <param name="pieceId">The identifier of the piece.</param>
        /// <param name="playerId">The identifier of the player.</param>
        /// <param name="targetCoordinate">The target coordinate on the board on which the piece should be positioned.</param>
        /// <returns>
        /// A success result if the positioning succeeded.
        /// A failure result (with a reason) if the positioning did not succeed.
        /// </returns>
        Result PositionPiece(Guid gameId, Guid pieceId, Guid playerId, BoardCoordinate targetCoordinate);

        /// <summary>
        /// Finds a game and then tries to mark one of the 2 players as 'ready'.
        /// </summary>
        /// <param name="gameId">The identifier of the game.</param>
        /// <param name="playerId">The identifier of the player.</param>
        void SetPlayerReady(Guid gameId, Guid playerId);

        /// <summary>
        /// Finds a game and then tries to move a piece on the board.
        /// This method is used when the game is started and a player wants to make a move.
        /// </summary>
        /// <param name="gameId">The identifier of the game.</param>
        /// <param name="pieceId">The identifier of the piece.</param>
        /// <param name="playerId">The identifier of the player.</param>
        /// <param name="targetCoordinate">The target coordinate on the board to which the piece should be moved.</param>
        /// <returns>
        /// A move data transfer object (dto) that contains details about the move (if the move succeeded).
        /// A failure result (with a reason) if the move did not succeed.
        /// </returns>
        Result<MoveDto> MovePiece(Guid gameId, Guid pieceId, Guid playerId, BoardCoordinate targetCoordinate);

        /// <summary>
        /// Finds a game and returns the last move (dto) that was made.
        /// </summary>
        /// <param name="gameId">The identifier of the game.</param>
        MoveDto GetLastMove(Guid gameId);

        #region EXTRA

        /// <summary>
        /// Returns a list of coordinates to which a certain piece is allowed to move.
        /// </summary>
        /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
        Result<IList<BoardCoordinate>> GetPossibleTargets(Guid gameId, Guid playerId, Guid pieceId);

        /// <summary>
        /// Takes a (file) stream and uses it to position the army of a player on the board.
        /// </summary>
        /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
        Result LoadArmyPositions(Guid gameId, Guid playerId, Stream fileStream);

        /// <summary>
        /// Finds a game and converts the positions of the army of a player to a byte array which can be downloaded.
        /// </summary>
        /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
        Result<DownloadDto> GetArmyPositionsDownload(Guid gameId, Guid playerId);

        #endregion
    }
}