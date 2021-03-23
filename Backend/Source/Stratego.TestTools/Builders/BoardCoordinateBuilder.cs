using Stratego.Domain.BoardDomain;
using System;
using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.TestTools.Builders
{
    public class BoardCoordinateBuilder
    {
        private readonly int _boardSize;
        private static readonly Random RandomGenerator = new Random();
        private BoardCoordinate _coordinate;

        public BoardCoordinateBuilder(int boardSize = 8)
        {
            _boardSize = boardSize;
            int row = RandomGenerator.Next(0, _boardSize);
            int column = RandomGenerator.Next(0, _boardSize);
            _coordinate = new BoardCoordinate(row, column);
        }

        public BoardCoordinateBuilder AsMoveablePositionNearBy(BoardCoordinate coordinate, IBoard board, int distance = 1)
        {
            BoardCoordinate[] neighborCoordinates =
            {
                new BoardCoordinate(coordinate.Row - distance, coordinate.Column),
                new BoardCoordinate(coordinate.Row + distance, coordinate.Column),
                new BoardCoordinate(coordinate.Row, coordinate.Column - distance),
                new BoardCoordinate(coordinate.Row, coordinate.Column + distance)
            };
            int index = RandomGenerator.Next(0, neighborCoordinates.Length);

            BoardCoordinate candidate = neighborCoordinates[index];
            int tries = 1;
            while (tries <= neighborCoordinates.Length && 
                   (candidate.IsOutOfBounds(_boardSize) || board.Squares[candidate.Row, candidate.Column].IsObstacle))
            {
                index = (index + 1) % neighborCoordinates.Length;
                candidate = neighborCoordinates[index];
                tries++;
            }

            _coordinate = candidate;

            return this;
        }

        public BoardCoordinateBuilder AsDiagonalPositionNearBy(BoardCoordinate coordinate, IBoard board)
        {
            BoardCoordinate[] diagonalCoordinates =
            {
                new BoardCoordinate(coordinate.Row - 1, coordinate.Column - 1),
                new BoardCoordinate(coordinate.Row + 1, coordinate.Column - 1),
                new BoardCoordinate(coordinate.Row + 1, coordinate.Column + 1),
                new BoardCoordinate(coordinate.Row - 1, coordinate.Column + 1)
            };
            int index = RandomGenerator.Next(0, diagonalCoordinates.Length);

            BoardCoordinate candidate = diagonalCoordinates[index];
            int tries = 1;
            while (tries <= diagonalCoordinates.Length &&
                   (candidate.IsOutOfBounds(_boardSize) || board.Squares[candidate.Row, candidate.Column].IsObstacle))
            {
                index = (index + 1) % diagonalCoordinates.Length;
                candidate = diagonalCoordinates[index];
                tries++;
            }

            _coordinate = candidate;

            return this;
        }

        public BoardCoordinate Build()
        {
            return _coordinate;
        }
    }
}
