using System;
using System.Collections.Generic;
using System.IO;
using Stratego.AppLogic.Contracts;
using Stratego.AppLogic.Dto;
using Stratego.AppLogic.Dto.Contracts;
using Stratego.Common;
using Stratego.Domain;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic
{
    /// <inheritdoc />
    internal class GameService : IGameService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameFactory">Factory that can be used to create a game.</param>
        /// <param name="gameRepository">
        /// Repository that can be used to store and retrieve games.
        /// An instance of InMemoryGameRepository (Stratego.Infrastructure/Storage) will be injected here.
        /// </param>
        /// <param name="boardDtoFactory">Factory that can transform a game board to a board data transfer object (dto).</param>
        /// <param name="playerGameDtoFactory">Factory that can create info about the game from the perspective of a certain player.</param>
        /// <param name="userRepository">
        /// Repository that can be used to rank users or manage friends of users.
        /// You only need this repository if you are implementing certain extra's.
        /// </param>
        public GameService(
            IGameFactory gameFactory,
            IGameRepository gameRepository,
            IBoardDtoFactory boardDtoFactory,
            IPlayerGameDtoFactory playerGameDtoFactory,
            IUserRepository userRepository)
        {
        }

        public Guid CreateGameForUsers(User user1, User user2, GameSettings settings)
        {
            throw new NotImplementedException();
        }

        public BoardDto GetBoardDto(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public PlayerGameDto GetPlayerGameDto(Guid gameId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Result PositionPiece(Guid gameId, Guid pieceId, Guid playerId, BoardCoordinate targetCoordinate)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerReady(Guid gameId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Result<MoveDto> MovePiece(Guid gameId, Guid pieceId, Guid playerId, BoardCoordinate targetCoordinate)
        {
            throw new NotImplementedException();
        }

        public MoveDto GetLastMove(Guid gameId)
        {
            throw new NotImplementedException();
        }

        #region EXTRA

        public Result<IList<BoardCoordinate>> GetPossibleTargets(Guid gameId, Guid playerId, Guid pieceId)
        {
            throw new NotImplementedException();
        }

        public Result LoadArmyPositions(Guid gameId, Guid playerId, Stream fileStream)
        {
            throw new NotImplementedException();
        }

        public Result<DownloadDto> GetArmyPositionsDownload(Guid gameId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}