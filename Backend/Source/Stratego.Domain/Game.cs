using System;
using Stratego.Common;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.BoardDomain.Contracts;
using Stratego.Domain.Contracts;

namespace Stratego.Domain
{
    /// <inheritdoc />
    internal class Game : IGame
    {
        public Guid Id { get; }
        public IPlayer RedPlayer { get; }
        public IPlayer BluePlayer { get; }
        public IBoard Board { get; }
        public bool IsStarted { get; }
        public bool IsOver { get; }
        public Move LastMove { get; }

        internal Game(IPlayer redPlayer, IPlayer bluePlayer, IBoard board)
        {

        }

        public Result PositionPiece(Guid playerId, Guid pieceId, BoardCoordinate targetCoordinate)
        {
            throw new NotImplementedException();
        }

        public Result<Move> MovePiece(Guid playerId, Guid pieceId, BoardCoordinate targetCoordinate)
        {
            throw new NotImplementedException();
        }

        public bool IsPlayersTurn(Guid playerId)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerReady(Guid playerId)
        {
            throw new NotImplementedException();
        }

        public IPlayer GetPlayerById(Guid playerId)
        {
            throw new NotImplementedException();
        }

        public IPlayer GetOpponent(IPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}