using Castle.DynamicProxy.Internal;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;
using Stratego.Domain.Contracts;
using Stratego.TestTools.Builders;
using Stratego.Common;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.ArmyDomain.Contracts;
using System;
using System.Linq;
using System.Reflection;
using Guts.Client.Core;
using Stratego.Domain.BoardDomain.Contracts;
using Stratego.TestTools;

namespace Stratego.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "Game", @"Stratego.Domain\Game.cs;Stratego.Domain\GameSettings.cs")]
    public class GameTests
    {
        private static readonly Random RandomGenerator = new Random();

        private Mock<IBoard> _boardMock;
        private PlayerMockBuilder _redPlayerMockBuilder;
        private PlayerMockBuilder _bluePlayerMockBuilder;
        private IPiece _piece;

        [SetUp]
        public void Setup()
        {
            //mock players and their armies to be ready and not defeated
            _redPlayerMockBuilder = new PlayerMockBuilder().WithIsRed(true).WithIsReady(true);
            _bluePlayerMockBuilder = new PlayerMockBuilder().WithIsRed(false).WithIsReady(true);
            _redPlayerMockBuilder.ArmyMock.SetupGet(a => a.IsDefeated).Returns(false);
            _bluePlayerMockBuilder.ArmyMock.SetupGet(a => a.IsDefeated).Returns(false);
            _boardMock = new Mock<IBoard>();
        }

        [MonitoredTest("Constructor - Should initialize properly")]
        public void Constructor_ShouldInitializeProperly()
        {
            IPlayer redPlayer = _redPlayerMockBuilder.Object;
            IPlayer bluePlayer = _bluePlayerMockBuilder.Object;

            //Act
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);

            //Assert
            Assert.That(game.Id, Is.Not.EqualTo(Guid.Empty), "The 'Id' property is not set correctly.");
            Assert.That(game.RedPlayer, Is.SameAs(redPlayer), "The 'RedPlayer' property is not set correctly.");
            Assert.That(game.BluePlayer, Is.SameAs(bluePlayer), "The 'BluePlayer' property is not set correctly.");
            Assert.That(game.Board, Is.SameAs(_boardMock.Object), "The 'Board' property is not set correctly.");
            Assert.That(game.LastMove, Is.Null, "The 'LastMove' property should be null.");
        }

        [MonitoredTest("IsStarted - Should be true when both players are ready")]
        public void IsStarted_ShouldBeTrueWhenBothPlayersAreReady()
        {
           //Arrange
           var redPlayer = _redPlayerMockBuilder.Object;
           var bluePlayer = _bluePlayerMockBuilder.Object;
           var game = new Game(redPlayer, bluePlayer, _boardMock.Object);

           //Act
           bool isStarted = game.IsStarted;

           //Assert
           Assert.That(isStarted, Is.True);
        }

        [MonitoredTest("IsStarted - Should be false when a player is not ready")]
        public void IsStarted_ShouldBeFalseWhenAPlayerIsNotReady()
        {
            //Arrange
            bool redIsReady = RandomGenerator.NextBool();
            var redPlayer = _redPlayerMockBuilder.WithIsReady(redIsReady).Object;
            var bluePlayer = _bluePlayerMockBuilder.WithIsReady(!redIsReady).Object;
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);

            //Act
            bool isStarted = game.IsStarted;

            //Assert
            Assert.That(isStarted, Is.False);
        }

        [MonitoredTest("IsOver - Should be true when one of the armies is defeated")]
        public void IsOver_ShouldBeTrueWhenOneOfTheArmiesIsDefeated()
        {
            //Arrange
            bool isRedDefeated = RandomGenerator.NextBool();
            _redPlayerMockBuilder.ArmyMock.SetupGet(a => a.IsDefeated).Returns(isRedDefeated);
            _bluePlayerMockBuilder.ArmyMock.SetupGet(a => a.IsDefeated).Returns(!isRedDefeated);
            var redPlayer = _redPlayerMockBuilder.Object;
            var bluePlayer = _bluePlayerMockBuilder.Object;
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);

            //Act
            bool isOver = game.IsOver;

            //Assert
            Assert.That(isOver, Is.True);
        }

        [MonitoredTest("IsOver - Should be false when no army is defeated")]
        public void IsOver_ShouldBeFalseWhenNoArmyIsIsDefeated()
        {
            //Arrange
            _redPlayerMockBuilder.ArmyMock.SetupGet(a => a.IsDefeated).Returns(false);
            _bluePlayerMockBuilder.ArmyMock.SetupGet(a => a.IsDefeated).Returns(false);
            var redPlayer = _redPlayerMockBuilder.Object;
            var bluePlayer = _bluePlayerMockBuilder.Object;
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);

            //Act
            bool isOver = game.IsOver;

            //Assert
            Assert.That(isOver, Is.False);
        }

        [MonitoredTest("PositionPiece - Should return failure when game is already started")]
        public void PositionPiece_ShouldReturnFailureWhenGameIsAlreadyStarted()
        {
            //Arrange
            IPlayer redPlayer = _redPlayerMockBuilder.Object;
            IPlayer bluePlayer = _bluePlayerMockBuilder.Object;
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);
            IPiece piece = new PieceMockBuilder().Object;
            BoardCoordinate boardCoordinate = new BoardCoordinateBuilder().Build();

            //Act
            Result result = game.PositionPiece(redPlayer.Id, piece.Id, boardCoordinate);

            //Assert
            Assert.That(result.IsFailure, Is.True, "Pieces can not be positioned when game has already started.");
            string expectedMessagePart = "started";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("PositionPiece - Should retrieve the piece from the army and position it on the board")]
        public void PositionPiece_ShouldRetrieveThePieceFromTheArmyAndPositionItOnTheBoard()
        {
            //Arrange
            IPlayer redPlayer = _redPlayerMockBuilder.WithIsReady(false).Object;
            IPlayer bluePlayer = _bluePlayerMockBuilder.WithIsReady(false).Object;
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);
            IPiece piece = new PieceMockBuilder().Object;
            _bluePlayerMockBuilder.ArmyMock.Setup(a => a.GetPieceById(It.IsAny<Guid>())).Returns(piece);

            BoardCoordinate targetPosition = new BoardCoordinateBuilder().Build();

            Result expectedResult = Result.CreateSuccessResult();
            _boardMock.Setup(b => b.PositionPiece(It.IsAny<IPiece>(), It.IsAny<BoardCoordinate>(), It.IsAny<bool>()))
                .Returns(expectedResult);

            //Act
            Result result = game.PositionPiece(bluePlayer.Id, piece.Id, targetPosition);

            //Assert
            _bluePlayerMockBuilder.ArmyMock.Verify(a => a.GetPieceById(piece.Id), Times.Once,
                "When the blue positions a piece, the piece should be retrieved from the army of the blue player (using 'GetPieceById').");
            _boardMock.Verify(b => b.PositionPiece(piece, targetPosition, false),
                "The 'PositionPiece' method of the board is not called correctly.");
            Assert.That(result, Is.SameAs(expectedResult),
                "The result should be the same object returned by the 'PositionPiece' method of the board.");
        }

        [MonitoredTest("MovePiece - Should return failure when game is not started yet")]
        public void MovePiece_ShouldReturnFailureWhenGameIsNotStartedYet()
        {
            //Arrange
            var game = CreateStartedGameForMove();
            _redPlayerMockBuilder.WithIsReady(false);
            _bluePlayerMockBuilder.WithIsReady(false);
            BoardCoordinate targetPosition = new BoardCoordinateBuilder().Build();

            //Act
            var result = game.MovePiece(_redPlayerMockBuilder.Object.Id, _piece.Id, targetPosition);

            //Assert
            Assert.That(result.IsFailure, Is.True, "Pieces can not be moved when game is not started.");
            string expectedMessagePart = "started";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("MovePiece - Should return failure when it's not the player's turn")]
        public void MovePiece_ShouldReturnFailureWhenItsNotThePlayersTurn()
        {
            //Arrange
            var game = CreateStartedGameForMove();
            BoardCoordinate targetPosition = new BoardCoordinateBuilder().Build();

            Move lastMove = new MoveBuilder(_redPlayerMockBuilder.Object.Id).Build();

            FieldInfo lastMoveBackingField = GetLastMoveBackingField();
            lastMoveBackingField.SetValue(game, lastMove);

            //Act
            Result<Move> result = game.MovePiece(_redPlayerMockBuilder.Object.Id, _piece.Id, targetPosition);

            //Assert
            Assert.That(result.IsFailure, Is.True, "A failure result should be returned");
            string expectedMessagePart = "turn";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("MovePiece - Should retrieve the piece from the army, move it on the board and remember the move")]
        public void MovePiece_ShouldRetrieveThePieceFromTheArmy_MoveItOnTheBoard_AndRememberTheMove()
        {
            //Arrange
            var game = CreateStartedGameForMove();
            BoardCoordinate targetPosition = new BoardCoordinateBuilder().Build();

            Move move = new MoveBuilder(_redPlayerMockBuilder.Object.Id).Build();
            Result<Move> expectedResult = Result<Move>.CreateSuccessResult(move);
            _boardMock.Setup(b => b.MovePiece(It.IsAny<IPiece>(), It.IsAny<BoardCoordinate>(), It.IsAny<Guid>()))
                .Returns(expectedResult);

            //Act
            Result<Move> result = game.MovePiece(_redPlayerMockBuilder.Object.Id, _piece.Id, targetPosition);

            //Assert
            _redPlayerMockBuilder.ArmyMock.Verify(a => a.GetPieceById(_piece.Id), Times.Once,
                "When red moves a piece, the piece should be retrieved from the army of the red player (using 'GetPieceById').");
            _boardMock.Verify(b => b.MovePiece(_piece, targetPosition, _redPlayerMockBuilder.Object.Id),
                "The 'MovePiece' method of the board is not called correctly.");
            Assert.That(result, Is.SameAs(expectedResult),
                "The result should be the same object returned by the 'MovePiece' method of the board.");
            Assert.That(game.LastMove, Is.SameAs(move), "The 'LastMove' of the game should be the same move returned by the 'MovePiece' method of the board.");
        }

        [MonitoredTest("IsPlayersTurn - Should return true for the red player when no move has been made yet")]
        public void IsPlayersTurn_ShouldReturnTrueForTheRedPlayerWhenNoMoveHasBeenMadeYet()
        {
            //Arrange
            Game game = CreateStartedGameForMove();
            FieldInfo lastMoveBackingField = GetLastMoveBackingField();
            lastMoveBackingField.SetValue(game, null);

            //Act
            bool isRedPlayersTurn = game.IsPlayersTurn(_redPlayerMockBuilder.Object.Id);
            bool isBluePlayersTurn = game.IsPlayersTurn(_bluePlayerMockBuilder.Object.Id);

            //Assert
            Assert.That(isRedPlayersTurn, Is.True, "It should be the red player's turn.");
            Assert.That(isBluePlayersTurn, Is.False, "It should not be the blue player's turn.");
        }

        [MonitoredTest("IsPlayersTurn - Should be true if the opponent made the last move")]
        public void IsPlayersTurn_ShouldBeTrueIfTheOpponentMadeTheLastMove()
        {
            //Arrange
            Game game = CreateStartedGameForMove();
            Move lastMove = new MoveBuilder(_redPlayerMockBuilder.Object.Id).Build();
            FieldInfo lastMoveBackingField = GetLastMoveBackingField();
            lastMoveBackingField.SetValue(game, lastMove);

            //Act
            bool isRedPlayersTurn = game.IsPlayersTurn(_redPlayerMockBuilder.Object.Id);
            bool isBluePlayersTurn = game.IsPlayersTurn(_bluePlayerMockBuilder.Object.Id);

            //Assert
            Assert.That(isRedPlayersTurn, Is.False, "It should not be the red player's turn when red made the last move.");
            Assert.That(isBluePlayersTurn, Is.True, "It should be the blue player's turn when red made the last move.");
        }

        [MonitoredTest("IsPlayersTurn - Should always return false when the game is not started yet")]
        public void IsPlayersTurn_ShouldAlwaysReturnFalseWhenTheGameIsNotStartedYet()
        {
            //Arrange
            var redPlayer = _redPlayerMockBuilder.WithIsReady(false).Object;
            var bluePlayer = _bluePlayerMockBuilder.WithIsReady(false).Object;
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);

            //Act
            bool isRedPlayersTurn = game.IsPlayersTurn(_redPlayerMockBuilder.Object.Id);
            bool isBluePlayersTurn = game.IsPlayersTurn(_bluePlayerMockBuilder.Object.Id);

            //Assert
            Assert.That(isRedPlayersTurn, Is.False, "It should not be the red player's turn.");
            Assert.That(isBluePlayersTurn, Is.False, "It should not be the blue player's turn.");
        }

        [MonitoredTest("GetPlayerById - Should return the matching player")]
        public void GetPlayerById_ShouldReturnTheMatchingPlayer()
        {
            //Arrange
            Game game = CreateStartedGameForMove();
            
            //Act
            var redPlayer = game.GetPlayerById(_redPlayerMockBuilder.Object.Id);
            var bluePlayer = game.GetPlayerById(_bluePlayerMockBuilder.Object.Id);

            //Assert
            Assert.That(redPlayer, Is.SameAs(game.RedPlayer), "Passing the id of the red player does not return the red player.");
            Assert.That(bluePlayer, Is.SameAs(game.BluePlayer), "Passing the id of the blue player does not return the blue player.");
        }

        [MonitoredTest("GetPlayerById - Non existing id - Should throw ApplicationException")]
        public void GetPlayerById_NonExistingId_ShouldThrowApplicationException()
        {
            //Arrange
            Game game = CreateStartedGameForMove();
            Guid invalidPlayerId = Guid.NewGuid();

            //Act + Assert
            Assert.That(() => game.GetPlayerById(invalidPlayerId), Throws.InstanceOf<ApplicationException>());
        }

        [MonitoredTest("SetPlayerReady - Should set the 'IsReady' property of the player")]
        public void SetPlayerReady_ShouldSetTheIsReadyPropertyOfThePlayer()
        {
            //Arrange
            var redPlayer = _redPlayerMockBuilder.WithIsReady(false).Object;
            var bluePlayer = _bluePlayerMockBuilder.WithIsReady(false).Object;
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);

            //Act
            game.SetPlayerReady(bluePlayer.Id);

            //Assert
            Assert.That(bluePlayer.IsReady, Is.True);
        }

        [MonitoredTest("GetOpponent - Should return the other player")]
        public void GetOpponent_ShouldReturnTheOtherPlayer()
        {
            //Arrange
            Game game = CreateStartedGameForMove();

            //Act
            var bluePlayer = game.GetOpponent(game.RedPlayer);
            var redPlayer = game.GetOpponent(game.BluePlayer);

            //Assert
            Assert.That(redPlayer, Is.SameAs(game.RedPlayer), "The opponent of the blue player should be the red player.");
            Assert.That(bluePlayer, Is.SameAs(game.BluePlayer), "The opponent of the red player should be the blue player.");
        }

        private FieldInfo GetLastMoveBackingField()
        {
            var lastMoveBackingField =
                typeof(Game).GetAllFields().FirstOrDefault(f => f.IsPrivate && f.FieldType == typeof(Move));
            Assert.That(lastMoveBackingField, Is.Not.Null, "Make sure the 'LastMove' property can be privately set.");
            return lastMoveBackingField;
        }

        private Game CreateStartedGameForMove()
        {
            IPlayer redPlayer = _redPlayerMockBuilder.WithIsReady(true).Object;
            IPlayer bluePlayer = _bluePlayerMockBuilder.WithIsReady(true).Object;
            var game = new Game(redPlayer, bluePlayer, _boardMock.Object);
            _piece = new PieceMockBuilder().Object;
            _redPlayerMockBuilder.ArmyMock.Setup(a => a.GetPieceById(It.IsAny<Guid>())).Returns(_piece);
            Result<Move> result = Result<Move>.CreateFailureResult("wrong result message");
            _boardMock.Setup(b => b.MovePiece(It.IsAny<IPiece>(), It.IsAny<BoardCoordinate>(), It.IsAny<Guid>())).Returns(result);
            return game;
        }
    }
}
