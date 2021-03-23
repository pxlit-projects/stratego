using System;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.BoardDomain;

namespace Stratego.TestTools.Builders
{
    public class MoveBuilder
    {
        private readonly Move _move;

        public MoveBuilder(Guid? playerId = null)
        {
            BoardCoordinate from = new BoardCoordinateBuilder().Build();
            BoardCoordinate to = new BoardCoordinateBuilder().Build();
            playerId ??= Guid.NewGuid();
            IPiece piece = new PieceMockBuilder().Mock.Object;
            _move = new Move(from, to, playerId.Value, piece);
        }

        public MoveBuilder WithTargetPiece()
        {
            IPiece targetPiece = new PieceMockBuilder().Mock.Object;
            _move.TargetPiece = targetPiece;
            return this;
        }

        public Move Build()
        {
            return _move;
        }
    }
}