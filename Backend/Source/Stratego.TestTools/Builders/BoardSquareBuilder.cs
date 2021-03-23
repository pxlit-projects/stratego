using Stratego.Domain.BoardDomain;
using System;

namespace Stratego.TestTools.Builders
{
    public class BoardSquareBuilder
    {
        private readonly BoardSquare _boardSquare;

        public BoardSquareBuilder()
        {
            bool isRedHomeTerritory = new Random().Next(0, 2) > 0;
            _boardSquare = new BoardSquare(isRedHomeTerritory, !isRedHomeTerritory);
        }
    }
}
