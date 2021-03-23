using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.Domain.BoardDomain
{
    /// <inheritdoc />
    public class Board : IBoard
    {
        public IBoardSquare[,] Squares { get; }
        public int Size { get; }

        internal Board(bool isQuickGameBoard)
        {

        }

        public Result<Move> MovePiece(IPiece piece, BoardCoordinate targetCoordinate, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Result PositionPiece(IPiece piece, BoardCoordinate targetCoordinate, bool playerIsRed)
        {
            throw new NotImplementedException();
        }
    }
}