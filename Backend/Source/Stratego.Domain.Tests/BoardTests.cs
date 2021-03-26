using Guts.Client.Shared;
using NUnit.Framework;
using Stratego.Common;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.Contracts;
using Stratego.TestTools;
using Stratego.TestTools.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using Guts.Client.Core;
using Moq;
using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "Board",
        @"Stratego.Domain\BoardDomain\Board.cs;
Stratego.Domain\BoardDomain\BoardSquare.cs;
Stratego.Domain\BoardDomain\Move.cs")]
    public class BoardTests
    {
        private static readonly Random RandomGenerator = new Random();
        private Board _board;
        private IPlayer _player;

        [SetUp]
        public void Setup()
        {
            _board = new Board(true);
            _player = new PlayerMockBuilder().Mock.Object;
        }

        [MonitoredTest("Constructor - Quick game board - Should initialize an 8x8 board")]
        public void Constructor_QuickGameBoard_ShouldInitializeAn8x8Board()
        {
            Assert.That(_board.Size, Is.EqualTo(8), "The 'Size' property should return 8.");
            Assert.That(_board.Squares, Is.Not.Null, "The 'Squares' property should return an 8x8 matrix of squares, but is null.");
            Assert.That(_board.Squares.GetLength(0), Is.EqualTo(8),
                "The 'Squares' property should return an 8x8 matrix of squares, but contains too many rows.");
            Assert.That(_board.Squares.GetLength(1), Is.EqualTo(8),
                "The 'Squares' property should return an 8x8 matrix of squares, but contains too many columns.");

            foreach (IBoardSquare square in _board.Squares)
            {
                Assert.That(square, Is.Not.Null, "One or more squares on the board do not contain an object that implements IBoardSquare.");
                Assert.That(square.Piece, Is.Null, "One or more squares on the board already contain a piece. The 'Piece' property should be null.");
            }
        }

        [MonitoredTest("Constructor - Quick game board - Should have 3 rows of home territory for both players")]
        public void Constructor_QuickGameBoard_ShouldHave3RowsOfHomeTerritoryForBothPlayers()
        {
            Constructor_QuickGameBoard_ShouldInitializeAn8x8Board();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _board.Size; j++)
                {
                    IBoardSquare redSquare = _board.Squares[i, j];
                    Assert.That(redSquare.IsRedHomeTerritory, Is.True,
                        $"The square [{i}][{j}] is not marked as red home territory.");
                    Assert.That(redSquare.IsBlueHomeTerritory, Is.False,
                        $"The square [{i}][{j}] should not be marked as blue home territory.");
                }
            }

            for (int i = 5; i < _board.Size; i++)
            {
                for (int j = 0; j < _board.Size; j++)
                {
                    IBoardSquare blueSquare = _board.Squares[i, j];
                    Assert.That(blueSquare.IsBlueHomeTerritory, Is.True,
                        $"The square [{i}][{j}] is not marked as blue home territory.");
                    Assert.That(blueSquare.IsRedHomeTerritory, Is.False,
                        $"The square [{i}][{j}] should not be marked as red home territory.");
                }
            }
        }

        [MonitoredTest("Constructor - Quick game board - Should have 2 obstacles of 1 by 2")]
        public void Constructor_QuickGameBoard_ShouldHave2ObstaclesOf1by2()
        {
            Constructor_QuickGameBoard_ShouldInitializeAn8x8Board();

            BoardCoordinate[] obstacleCoordinates =
            {
                new BoardCoordinate(3, 2),
                new BoardCoordinate(4, 2),
                new BoardCoordinate(3, 5),
                new BoardCoordinate(4, 5)
            };

            for (int i = 0; i < _board.Size; i++)
            {
                for (int j = 0; j < _board.Size; j++)
                {
                    IBoardSquare square = _board.Squares[i, j];

                    if (obstacleCoordinates.Contains(new BoardCoordinate(i, j)))
                    {
                        Assert.That(square.IsObstacle, Is.True, $"The square [{i}][{j}] should be an obstacle.");
                    }
                    else
                    {
                        Assert.That(square.IsObstacle, Is.False, $"The square [{i}][{j}] should not be an obstacle.");
                    }
                }
            }
        }

        [MonitoredTest("MovePiece - Should return failure when target coordinate is out of bounds")]
        public void MovePiece_ShouldReturnFailureWhenTargetCoordinateIsOutOfBounds()
        {
            IPiece piece = new PieceMockBuilder().WithRandomPositionOnBoard(_board).Mock.Object;

            BoardCoordinate[] outOfBoundCoordinates =
            {
                new BoardCoordinate(piece.Position.Row + _board.Size, piece.Position.Column),
                new BoardCoordinate(piece.Position.Row - _board.Size, piece.Position.Column),
                new BoardCoordinate(piece.Position.Row, piece.Position.Column + _board.Size),
                new BoardCoordinate(piece.Position.Row, piece.Position.Column - _board.Size)
            };

            foreach (BoardCoordinate outOfBoundCoordinate in outOfBoundCoordinates)
            {
                Result<Move> result = _board.MovePiece(piece, outOfBoundCoordinate, _player.Id);
                Assert.That(result.IsFailure, Is.True, $"No failure is returned for target {outOfBoundCoordinate}.");
                string expectedMessagePart = "out of bounds";
                Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                    $"The message of the result should contain '{expectedMessagePart}'.");
            }
        }

        [MonitoredTest("MovePiece - Should return failure when the piece is not moveable")]
        public void MovePiece_ShouldReturnFailureWhenThePieceIsNotMoveable()
        {
            //Arrange
            IPiece piece = new PieceMockBuilder().WithIsMoveable(false).WithRandomPositionOnBoard(_board).Mock.Object;
            BoardCoordinate targetCoordinate =
                new BoardCoordinateBuilder().AsMoveablePositionNearBy(piece.Position, _board).Build();

            //Act
            Result<Move> result = _board.MovePiece(piece, targetCoordinate, _player.Id);

            //Assert
            Assert.That(result.IsFailure, Is.True, "No failure is returned");
            string expectedMessagePart = "cannot be moved";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("MovePiece - Should return failure when the move is not in a straight line")]
        public void MovePiece_ShouldReturnFailureWhenTheMoveIsNotInAStraightLine()
        {
            //Arrange
            IPiece piece = new PieceMockBuilder().WithRandomPositionOnBoard(_board).Mock.Object;
            BoardCoordinate originalPosition = piece.Position;
            BoardCoordinate targetCoordinate =
                new BoardCoordinateBuilder().AsDiagonalPositionNearBy(piece.Position, _board).Build();

            //Act
            Result<Move> result = _board.MovePiece(piece, targetCoordinate, _player.Id);

            //Assert
            Assert.That(result.IsFailure, Is.True, $"No failure is returned for piece moving from {originalPosition} to {targetCoordinate}");
            string expectedMessagePart = "straight line";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("MovePiece - Should return failure when target is an obstacle")]
        public void MovePiece_ShouldReturnFailureWhenTargetIsAnObstacle()
        {
            //Arrange
            BoardCoordinate obstacleCoordinate = GetBoardCoordinates(square => square.IsObstacle).NextRandomElement();
            IPiece piece = new PieceMockBuilder().WithPositionOnBoardNextTo(obstacleCoordinate, _board).Mock.Object;
            BoardCoordinate originalPosition = piece.Position;

            //Act
            Result<Move> result = _board.MovePiece(piece, obstacleCoordinate, _player.Id);

            //Assert
            Assert.That(result.IsFailure, Is.True, $"No failure is returned for piece moving from {originalPosition} to {obstacleCoordinate}");
            string expectedMessagePart = "obstacle";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("MovePiece - Piece cannot move any distance - Should return failure when moving over multiple squares")]
        public void MovePiece_PieceCannotMoveAnyDistance_ShouldReturnFailureWhenMovingOverMultipleSquares()
        {
            //Arrange
            IPiece piece = new PieceMockBuilder()
                .WithRandomPositionOnBoard(_board)
                .WithCanMoveAnyDistance(false)
                .Mock.Object;
            BoardCoordinate originalPosition = piece.Position;

            BoardCoordinate targetCoordinate =
                new BoardCoordinateBuilder().AsMoveablePositionNearBy(piece.Position, _board, distance: 2).Build();

            //Act
            Result<Move> result = _board.MovePiece(piece, targetCoordinate, _player.Id);

            //Assert
            Assert.That(result.IsFailure, Is.True, $"No failure is returned for piece moving from {originalPosition} to {targetCoordinate}");
            string expectedMessagePart = "multiple squares";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("MovePiece - Piece can move any distance - Should return failure when jumping over an obstacle")]
        public void MovePiece_PieceCanMoveAnyDistance_ShouldReturnFailureWhenJumpingOverAnObstacle()
        {
            //Arrange
            BoardCoordinate obstacleCoordinate = GetBoardCoordinates(square => square.IsObstacle).NextRandomElement();
            BoardCoordinate originalPosition = new BoardCoordinate(0, obstacleCoordinate.Column);
            IPiece piece = new PieceMockBuilder()
                .WithCanMoveAnyDistance(true)
                .WithPosition(originalPosition).Mock.Object;
            _board.Squares[originalPosition.Row, originalPosition.Column].Piece = piece;
            BoardCoordinate targetPosition = new BoardCoordinate(_board.Size - 1, obstacleCoordinate.Column);

            //Act
            Result<Move> result = _board.MovePiece(piece, targetPosition, _player.Id);

            //Assert
            Assert.That(result.IsFailure, Is.True,
                $"No failure is returned for piece moving from {originalPosition} to {targetPosition} (over obstacle at {obstacleCoordinate})");
            string expectedMessagePart = "obstacle";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("MovePiece - Piece can move any distance - Should return failure when jumping over a piece")]
        public void MovePiece_PieceCanMoveAnyDistance_ShouldReturnFailureWhenJumpingOverAPiece()
        {
            //Arrange
            BoardCoordinate originalPosition = new BoardCoordinate(0, 0);
            IPiece piece = new PieceMockBuilder()
                .WithCanMoveAnyDistance(true)
                .WithPosition(originalPosition).Mock.Object;
            _board.Squares[originalPosition.Row, originalPosition.Column].Piece = piece;

            BoardCoordinate middlePosition = new BoardCoordinate(RandomGenerator.Next(1, 6), originalPosition.Column);
            IPiece middlePiece = new PieceMockBuilder().WithPosition(middlePosition).Mock.Object;
            _board.Squares[middlePosition.Row, middlePosition.Column].Piece = middlePiece;

            BoardCoordinate targetPosition = new BoardCoordinate(RandomGenerator.Next(middlePosition.Row + 1, _board.Size), originalPosition.Column);

            //Act
            Result<Move> result = _board.MovePiece(piece, targetPosition, _player.Id);

            //Assert
            Assert.That(result.IsFailure, Is.True,
                $"No failure is returned for piece moving from {originalPosition} to {targetPosition} (over piece at {middlePosition})");
            string expectedMessagePart = "jump over";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("MovePiece - To empty square - Should move the piece to the target position")]
        public void MovePiece_ToEmptySquare_ShouldMoveThePieceToTheTargetPosition()
        {
            //Arrange
            Mock<IPiece> pieceMock = new PieceMockBuilder().WithRandomPositionOnBoard(_board).Mock;
            IPiece piece = pieceMock.Object;
            BoardCoordinate originalPosition = piece.Position;
            BoardCoordinate targetPosition =
                new BoardCoordinateBuilder().AsMoveablePositionNearBy(piece.Position, _board).Build();

            //Act
            Result<Move> result = _board.MovePiece(piece, targetPosition, _player.Id);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned");
            AssertBasicMove(pieceMock, originalPosition, targetPosition);

            Move move = result.Value;
            Assert.That(move.From, Is.EqualTo(originalPosition),
                "The 'From' property of the returned 'Move' is not set correctly.");
            Assert.That(move.To, Is.EqualTo(targetPosition),
                "The 'To' property of the returned 'Move' is not set correctly.");
            Assert.That(move.Piece, Is.SameAs(piece),
                "The 'Piece' property of the returned 'Move' is not set correctly.");
            Assert.That(move.TargetPiece, Is.Null,
                "The 'TargetPiece' property of the returned 'Move' should be null.");
            Assert.That(move.PlayerId, Is.EqualTo(_player.Id),
                "The 'PlayerId' property of the returned 'Move' is not set correctly.");
        }

        [MonitoredTest("MovePiece - Attack and win - Should remove the attacked piece from the board")]
        public void MovePiece_AttackAndWin_ShouldRemoveTheAttackedPieceFromTheBoard()
        {
            //Arrange
            Mock<IPiece> pieceMock = new PieceMockBuilder()
                .WithRandomPositionOnBoard(_board).Mock;
            IPiece piece = pieceMock.Object;
            BoardCoordinate originalPosition = piece.Position;

            BoardCoordinate targetPosition =
                new BoardCoordinateBuilder().AsMoveablePositionNearBy(piece.Position, _board).Build();
            Mock<IPiece> targetPieceMock = new PieceMockBuilder()
                .WithPosition(targetPosition).Mock;
            IPiece targetPiece = targetPieceMock.Object;
            GetBoardSquareAt(targetPosition).Piece = targetPiece;

            pieceMock.Setup(p => p.Attack(It.IsAny<IPiece>())).Callback(() =>
            {
                targetPiece.IsAlive = false;
            });

            //Act
            Result<Move> result = _board.MovePiece(piece, targetPosition, _player.Id);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned");
            AssertBasicMove(pieceMock, originalPosition, targetPosition);

            pieceMock.Verify(p => p.Attack(targetPiece), Times.Once,
                "The 'Attack' method of the moving piece is not called correctly.");

            Assert.That(targetPiece.Position, Is.Null,
                "The attacked piece is not removed from the board ('Position' property still has a value)");

            Move move = result.Value;
            Assert.That(move.TargetPiece, Is.SameAs(targetPiece),
                "The 'TargetPiece' property of the returned 'Move' is not set correctly.");
        }

        [MonitoredTest("MovePiece - Attack and lose - Should remove the attacking piece from the board")]
        public void MovePiece_AttackAndLose_ShouldRemoveTheAttackingPieceFromTheBoard()
        {
            //Arrange
            Mock<IPiece> pieceMock = new PieceMockBuilder()
                .WithRandomPositionOnBoard(_board).Mock;
            IPiece piece = pieceMock.Object;
            BoardCoordinate originalPosition = piece.Position;

            BoardCoordinate targetPosition =
                new BoardCoordinateBuilder().AsMoveablePositionNearBy(piece.Position, _board).Build();
            Mock<IPiece> targetPieceMock = new PieceMockBuilder()
                .WithPosition(targetPosition).Mock;
            IPiece targetPiece = targetPieceMock.Object;
            GetBoardSquareAt(targetPosition).Piece = targetPiece;

            pieceMock.Setup(p => p.Attack(It.IsAny<IPiece>())).Callback(() =>
            {
                piece.IsAlive = false;
            });

            //Act
            Result<Move> result = _board.MovePiece(piece, targetPosition, _player.Id);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned");

            pieceMock.Verify(p => p.Attack(targetPiece), Times.Once,
                "The 'Attack' method of the moving piece is not called correctly.");

            var originalSquare = GetBoardSquareAt(originalPosition);
            string messageSuffix = $" after moving from {originalPosition} to {targetPosition}";
            Assert.That(originalSquare.Piece, Is.Null,
                $"The square at {originalSquare} should not contain a piece{messageSuffix}");
            var targetSquare = GetBoardSquareAt(targetPosition);
            Assert.That(targetSquare.Piece, Is.SameAs(targetPieceMock.Object),
                $"The square at {targetPosition} should contain the attacked piece{messageSuffix}");

            Assert.That(piece.Position, Is.Null,
                "The attacking piece is not removed from the board ('Position' property still has a value)");

            Assert.That(targetPiece.Position, Is.EqualTo(targetPosition),
                "The position of the attacked piece has changed.");

            Move move = result.Value;
            Assert.That(move.TargetPiece, Is.SameAs(targetPiece),
                "The 'TargetPiece' property of the returned 'Move' is not set correctly.");
        }

        [MonitoredTest("MovePiece - Attack with draw - Should remove both pieces from the board")]
        public void MovePiece_AttackWithDraw_ShouldRemoveBothPiecesFromTheBoard()
        {
            //Arrange
            Mock<IPiece> pieceMock = new PieceMockBuilder()
                .WithRandomPositionOnBoard(_board).Mock;
            IPiece piece = pieceMock.Object;
            BoardCoordinate originalPosition = piece.Position;

            BoardCoordinate targetPosition =
                new BoardCoordinateBuilder().AsMoveablePositionNearBy(piece.Position, _board).Build();
            Mock<IPiece> targetPieceMock = new PieceMockBuilder()
                .WithPosition(targetPosition).Mock;
            IPiece targetPiece = targetPieceMock.Object;
            GetBoardSquareAt(targetPosition).Piece = targetPiece;

            pieceMock.Setup(p => p.Attack(It.IsAny<IPiece>())).Callback(() =>
            {
                piece.IsAlive = false;
                targetPiece.IsAlive = false;
            });

            //Act
            Result<Move> result = _board.MovePiece(piece, targetPosition, _player.Id);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned");

            pieceMock.Verify(p => p.Attack(targetPiece), Times.Once,
                "The 'Attack' method of the moving piece is not called correctly.");

            var originalSquare = GetBoardSquareAt(originalPosition);
            string messageSuffix = $" after moving from {originalPosition} to {targetPosition}";
            Assert.That(originalSquare.Piece, Is.Null,
                $"The square at {originalSquare} should not contain a piece{messageSuffix}");
            var targetSquare = GetBoardSquareAt(targetPosition);
            Assert.That(targetSquare.Piece, Is.Null,
                $"The square at {targetPosition} should not contain a piece{messageSuffix}");

            Assert.That(piece.Position, Is.Null,
                "The attacking piece is not removed from the board ('Position' property still has a value)");

            Assert.That(targetPiece.Position, Is.Null,
                "The attacked piece is not removed from the board ('Position' property still has a value)");

            Move move = result.Value;
            Assert.That(move.TargetPiece, Is.SameAs(targetPiece),
                "The 'TargetPiece' property of the returned 'Move' is not set correctly.");
        }

        [MonitoredTest("PositionPiece - Should return failure when target coordinate is out of bounds")]
        public void PositionPiece_ShouldReturnFailureWhenTargetCoordinateIsOutOfBounds()
        {
            IPiece piece = new PieceMockBuilder().Mock.Object;

            BoardCoordinate[] outOfBoundCoordinates =
            {
                new BoardCoordinate(RandomGenerator.Next(_board.Size, _board.Size + 2), RandomGenerator.Next(0,_board.Size)),
                new BoardCoordinate(RandomGenerator.Next(-2, 0), RandomGenerator.Next(0,_board.Size)),
                new BoardCoordinate(RandomGenerator.Next(0,_board.Size), RandomGenerator.Next(_board.Size, _board.Size + 2)),
                new BoardCoordinate(RandomGenerator.Next(0,_board.Size), RandomGenerator.Next(-2, 0))
            };

            foreach (BoardCoordinate outOfBoundCoordinate in outOfBoundCoordinates)
            {
                Result result = _board.PositionPiece(piece, outOfBoundCoordinate, true);
                Assert.That(result.IsFailure, Is.True, $"No failure is returned for target {outOfBoundCoordinate}.");
                string expectedMessagePart = "out of bounds";
                Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                    $"The message of the result should contain '{expectedMessagePart}'.");
            }
        }

        [MonitoredTest("PositionPiece - Should return failure when target is an obstacle")]
        public void PositionPiece_ShouldReturnFailureWhenTargetIsAnObstacle()
        {
            //Arrange
            BoardCoordinate obstacleCoordinate = GetBoardCoordinates(square => square.IsObstacle).NextRandomElement();
            IPiece piece = new PieceMockBuilder().Mock.Object;

            //Act
            Result result = _board.PositionPiece(piece, obstacleCoordinate, false);

            //Assert
            Assert.That(result.IsFailure, Is.True, $"No failure is returned when positioning on {obstacleCoordinate}");
            string expectedMessagePart = "obstacle";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }


        [MonitoredTest("PositionPiece - Should return failure when a piece is not positioned in home territory")]
        public void PositionPiece_ShouldReturnFailureWhenAPieceIsNotPositionedInHomeTerritory()
        {
            TestNoneHomeTerritoryPositioning(true);
            TestNoneHomeTerritoryPositioning(false);
        }

        private void TestNoneHomeTerritoryPositioning(bool isRed)
        {
            IPiece piece = new PieceMockBuilder().Mock.Object;
            var invalidCoordinate = isRed
                ? GetBoardCoordinates(square => !square.IsRedHomeTerritory && !square.IsObstacle).NextRandomElement()
                : GetBoardCoordinates(square => !square.IsBlueHomeTerritory && !square.IsObstacle).NextRandomElement();
            
            Result result = _board.PositionPiece(piece, invalidCoordinate, isRed);
            string color = isRed ? "red" : "blue";
            Assert.That(result.IsFailure, Is.True, $"No failure is returned when positioning a {color} piece on {invalidCoordinate}");
            string expectedMessagePart = "territory";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("PositionPiece - Valid empty target position - Should position the piece")]
        public void PositionPiece_ValidEmptyTargetPosition_ShouldPositionThePiece()
        {
            //Arrange
            IPiece piece = new PieceMockBuilder().Mock.Object;
            BoardCoordinate targetPosition =
                GetBoardCoordinates(square => square.IsRedHomeTerritory).NextRandomElement();

            //Act
            Result result = _board.PositionPiece(piece, targetPosition, true);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned");

            var targetSquare = GetBoardSquareAt(targetPosition);
            Assert.That(targetSquare.Piece, Is.SameAs(piece),
                $"The square at {targetPosition} should contain the positioned piece");
            Assert.That(piece.Position, Is.EqualTo(targetPosition),
                "The 'Position' of the piece is not set correctly.");
        }

        [MonitoredTest("PositionPiece - Target position contains piece - Should take other piece of the board")]
        public void PositionPiece_TargetPositionContainsPiece_ShouldTakeOtherPieceOfTheBoard()
        {
            //Arrange
            BoardCoordinate targetPosition =
                GetBoardCoordinates(square => square.IsBlueHomeTerritory).NextRandomElement();
            IPiece piece = new PieceMockBuilder().Mock.Object;
            IPiece targetPiece = new PieceMockBuilder().WithPosition(targetPosition).Mock.Object;
            var targetSquare = GetBoardSquareAt(targetPosition);
            targetSquare.Piece = targetPiece;

            //Act
            Result result = _board.PositionPiece(piece, targetPosition, false);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned");
            Assert.That(targetSquare.Piece, Is.SameAs(piece),
                $"The square at {targetPosition} should contain the positioned piece");
            Assert.That(piece.Position, Is.EqualTo(targetPosition),
                "The 'Position' of the piece is not set correctly.");
            Assert.That(targetPiece.Position, Is.Null,
                "The piece being replaced is not removed from the board ('Position' property still has a value)");
        }

        [MonitoredTest("PositionPiece - Target and source piece already on the board - Should swap the pieces")]
        public void PositionPiece_TargetAndSourcePieceAlreadyOnTheBoard_ShouldSwapThePieces()
        {
            //Arrange
            IList<BoardCoordinate> redHomeCoordinates = GetBoardCoordinates(square => square.IsRedHomeTerritory);
            BoardCoordinate sourcePosition = redHomeCoordinates.NextRandomElement();
            BoardCoordinate targetPosition = redHomeCoordinates.NextRandomElement();
            while (targetPosition == sourcePosition)
            {
                targetPosition = redHomeCoordinates.NextRandomElement();
            }

            IPiece sourcePiece = new PieceMockBuilder().WithPosition(sourcePosition).Mock.Object;
            var sourceSquare = GetBoardSquareAt(sourcePosition);
            sourceSquare.Piece = sourcePiece;
            IPiece targetPiece = new PieceMockBuilder().WithPosition(targetPosition).Mock.Object;
            var targetSquare = GetBoardSquareAt(targetPosition);
            targetSquare.Piece = targetPiece;

            //Act
            Result result = _board.PositionPiece(sourcePiece, targetPosition, true);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned");
            Assert.That(targetSquare.Piece, Is.SameAs(sourcePiece),
                $"The square at {targetPosition} should contain the positioned piece");
            Assert.That(sourcePiece.Position, Is.EqualTo(targetPosition),
                "The 'Position' of the piece is not set correctly.");
            Assert.That(sourceSquare.Piece, Is.SameAs(targetPiece),
                $"The square at {sourcePosition} should contain the targeted piece");
            Assert.That(targetPiece.Position, Is.EqualTo(sourcePosition),
                "The 'Position' of the targeted piece is not set correctly.");
        }

        private IList<BoardCoordinate> GetBoardCoordinates(Predicate<IBoardSquare> predicate)
        {
            IList<BoardCoordinate> coordinates = new List<BoardCoordinate>();
            for (int i = 0; i < _board.Size; i++)
            {
                for (int j = 0; j < _board.Size; j++)
                {
                    if (predicate(_board.Squares[i, j]))
                    {
                        coordinates.Add(new BoardCoordinate(i, j));
                    }
                }
            }
            return coordinates;
        }

        private IBoardSquare GetBoardSquareAt(BoardCoordinate coordinate)
        {
            return _board.Squares[coordinate.Row, coordinate.Column];
        }

        private void AssertBasicMove(Mock<IPiece> pieceMock, BoardCoordinate originalPosition, BoardCoordinate targetPosition)
        {
            var originalSquare = GetBoardSquareAt(originalPosition);
            string messageSuffix = $" after moving from {originalPosition} to {targetPosition}";
            Assert.That(originalSquare.Piece, Is.Null,
                $"The square at {originalSquare} should not contain a piece{messageSuffix}");
            var targetSquare = GetBoardSquareAt(targetPosition);
            Assert.That(targetSquare.Piece, Is.SameAs(pieceMock.Object),
                $"The square at {targetPosition} should contain the moved piece{messageSuffix}");
            pieceMock.VerifySet(p => p.Position = targetPosition, Times.Once,
                $"The position op the moved piece should be set to {targetPosition}{messageSuffix}");
        }
    }
}
