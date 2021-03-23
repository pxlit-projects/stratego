using System;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;
using Stratego.AppLogic.Dto;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.AppLogic.Tests
{
    public class BoardDtoFactoryTests
    {
        private static readonly Random RandomGenerator = new Random();

        private BoardDtoFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new BoardDtoFactory();
        }

        [MonitoredTest("CreateFromBoard - Converts the square matrix of the grid to a jagged array")]
        public void CreateFromBoard_ConvertsTheSquareMatrixOfTheGridToAJaggedArray()
        {
            //Arrange
            IBoard board = ArrangeBoard();

            //Act
            BoardDto boardDto = _factory.CreateFromBoard(board);

            //Assert
            Assert.That(boardDto, Is.Not.Null, "No instance of a class that implements BoardDto is returned.");
            Assert.That(boardDto.Size, Is.EqualTo(board.Size), "The Size of the dto should be the same as the Size of the board.");

            Assert.That(boardDto.Squares, Is.Not.Null, "The Squares property of the BoardDto does not contain an instance of a jagged array.");
            Assert.That(boardDto.Squares.Length, Is.EqualTo(board.Size), "The Squares should have a length equal to the size of the board.");
            foreach (var squareDtoRow in boardDto.Squares)
            {
                Assert.That(squareDtoRow, Is.Not.Null,
                    "The value for each element in the array of the 1ste dimension (row) should in turn be an instance of an array (columns).");
                Assert.That(squareDtoRow.Length, Is.EqualTo(board.Size),
                    "Each row of squares should have a length equal to the size of the board.");
            }
        }

        [MonitoredTest("CreateFromBoard - Generates the correct BoardSquareDtos")]
        public void CreateFromBoard_GeneratesTheCorrectBoardSquareDtos()
        {
            //Arrange
            IBoard board = ArrangeBoard();

            //Act
            BoardDto boardDto = _factory.CreateFromBoard(board);

            //Assert
            Assert.That(boardDto, Is.Not.Null, "No instance of a class that implements BoardDto is returned.");

            for (var i = 0; i < boardDto.Squares.Length; i++)
            {
                BoardSquareDto[] squareDtoRow = boardDto.Squares[i];
                for (var j = 0; j < squareDtoRow.Length; j++)
                {
                    BoardSquareDto squareDto = squareDtoRow[j];
                    IBoardSquare matchingSquare = board.Squares[i, j];

                    Assert.That(squareDto.IsBlueHomeTerritory, Is.EqualTo(matchingSquare.IsBlueHomeTerritory),
                        $"'IsBlueHomeTerritory' for square dto at [{i}][{j}] is not correct.");
                    Assert.That(squareDto.IsRedHomeTerritory, Is.EqualTo(matchingSquare.IsRedHomeTerritory),
                        $"'IsRedHomeTerritory' for square dto at [{i}][{j}] is not correct.");
                    Assert.That(squareDto.IsObstacle, Is.EqualTo(matchingSquare.IsObstacle),
                        $"'IsObstacle' for square dto at [{i}][{j}] is not correct.");
                }
            }
        }

        private IBoard ArrangeBoard()
        {
            var boardMock = new Mock<IBoard>();
            int size = 8;
            boardMock.SetupGet(b => b.Size).Returns(size);
            var squares = new IBoardSquare[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    bool isRed = false;
                    bool isBlue = false;
                    bool isObstacle = false;
                    double randomDouble = RandomGenerator.NextDouble();
                    if (randomDouble < 0.3)
                    {
                        isRed = true;
                    }
                    else if(randomDouble > 0.6)
                    {
                        isBlue = true;
                    }
                    else
                    {
                        isObstacle = RandomGenerator.NextDouble() < 0.25;
                    }

                    var square = new BoardSquare(isRed, isBlue) {IsObstacle = isObstacle};
                    squares[i, j] = square;
                }
            }
            boardMock.SetupGet(b => b.Squares).Returns(squares);
            return boardMock.Object;
        }
    }
}