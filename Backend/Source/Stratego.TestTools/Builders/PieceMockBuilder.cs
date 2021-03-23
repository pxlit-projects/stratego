using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.BoardDomain;
using System;
using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.TestTools.Builders
{
    public class PieceMockBuilder : MockBuilder<IPiece>
    {
        private static readonly Random RandomGenerator = new Random();

        public PieceMockBuilder()
        {
            Mock.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            Mock.SetupGet(p => p.Name).Returns(Guid.NewGuid().ToString());
            Mock.SetupGet(p => p.Strength).Returns(RandomGenerator.Next(0, 12));
            Mock.SetupProperty(p => p.Position, null);
            Mock.SetupProperty(p => p.IsAlive, true);
            Mock.SetupGet(p => p.IsMoveable).Returns(true);
            Mock.SetupGet(p => p.CanMoveAnyDistance).Returns(false);
        }

        public PieceMockBuilder WithStrength(int strength)
        {
            Mock.SetupGet(p => p.Strength).Returns(strength);
            return this;
        }

        public PieceMockBuilder WithName(string name)
        {
            Mock.SetupGet(p => p.Name).Returns(name);
            return this;
        }

        public PieceMockBuilder WithAlive(bool isAlive)
        {
            Mock.SetupProperty(p => p.IsAlive, isAlive);
            return this;
        }

        public PieceMockBuilder WithIsMoveable(bool isMoveable)
        {
            Mock.SetupGet(p => p.IsMoveable).Returns(isMoveable);
            return this;
        }

        public PieceMockBuilder WithCanMoveAnyDistance(bool canMoveAnyDistance)
        {
            Mock.SetupGet(p => p.CanMoveAnyDistance).Returns(canMoveAnyDistance);
            return this;
        }

        public PieceMockBuilder WithPosition(BoardCoordinate position)
        {
            Mock.SetupProperty(p => p.Position, position);
            return this;
        }

        public PieceMockBuilder WithRandomPositionOnBoard(IBoard board)
        {
            var position = new BoardCoordinateBuilder().Build();
            while (board.Squares[position.Row, position.Column].IsObstacle ||
                   board.Squares[position.Row, position.Column].Piece != null)
            {
                position = new BoardCoordinateBuilder().Build();
            }

            WithPosition(position);
            board.Squares[position.Row, position.Column].Piece = Mock.Object;

            return this;
        }

        public PieceMockBuilder WithPositionOnBoardNextTo(BoardCoordinate coordinate, IBoard board)
        {
            BoardCoordinate[] neighborCoordinates =
            {
                new BoardCoordinate(coordinate.Row - 1, coordinate.Column),
                new BoardCoordinate(coordinate.Row + 1, coordinate.Column),
                new BoardCoordinate(coordinate.Row, coordinate.Column - 1),
                new BoardCoordinate(coordinate.Row, coordinate.Column + 1)
            };
            int index = RandomGenerator.Next(0, neighborCoordinates.Length);

            BoardCoordinate candidate = neighborCoordinates[index];
            int tries = 1;
            while (tries <= neighborCoordinates.Length &&
                   (candidate.IsOutOfBounds(board.Size) || board.Squares[candidate.Row, candidate.Column].IsObstacle))
            {
                index = (index + 1) % neighborCoordinates.Length;
                candidate = neighborCoordinates[index];
                tries++;
            }

            WithPosition(candidate);
            board.Squares[candidate.Row, candidate.Column].Piece = Mock.Object;

            return this;
        }
    }
}
