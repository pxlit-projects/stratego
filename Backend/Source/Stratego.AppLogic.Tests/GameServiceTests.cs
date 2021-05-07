using System;
using Guts.Client.Core;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;
using Stratego.AppLogic.Contracts;
using Stratego.AppLogic.Dto;
using Stratego.AppLogic.Dto.Contracts;
using Stratego.Common;
using Stratego.Domain;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.BoardDomain.Contracts;
using Stratego.Domain.Contracts;
using Stratego.TestTools.Builders;

namespace Stratego.AppLogic.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "GameService", @"Stratego.AppLogic\GameService.cs")]
    public class GameServiceTests
    {
        private Mock<IGameFactory> _gameFactoryMock;
        private Mock<IGameRepository> _gameRepositoryMock;
        private Mock<IBoardDtoFactory> _boardDtoFactoryMock;
        private Mock<IPlayerGameDtoFactory> _playerGameDtoFactoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private GameService _service;
        private GameSettings _gameSettings;
        private IGame _game;
        private Mock<IGame> _gameMock;

        [SetUp]
        public void Setup()
        {
            _gameFactoryMock = new Mock<IGameFactory>();
            _gameRepositoryMock = new Mock<IGameRepository>();
            _boardDtoFactoryMock = new Mock<IBoardDtoFactory>();
            _playerGameDtoFactoryMock = new Mock<IPlayerGameDtoFactory>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new GameService(
                _gameFactoryMock.Object,
                _gameRepositoryMock.Object,
                _boardDtoFactoryMock.Object,
                _playerGameDtoFactoryMock.Object,
                _userRepositoryMock.Object);

            _gameSettings = new GameSettingsBuilder().Build();

            _gameMock = new GameMockBuilder().Mock;
            _game = _gameMock.Object;


            _gameFactoryMock.Setup(factory =>
                factory.CreateNewForUsers(It.IsAny<User>(), It.IsAny<User>(), It.IsAny<GameSettings>())).Returns(_game);
            _gameRepositoryMock.Setup(repo => repo.GetById(It.IsAny<Guid>())).Returns(_game);
        }

        [MonitoredTest("CreateGameForUsers - Should create a game and add it to the repository")]
        public void CreateGameForUsers_ShouldCreateAGameAndAddItToTheRepository()
        {
            //Arrange
            User user1 = new UserBuilder().Build();
            User user2 = new UserBuilder().Build();
            _gameFactoryMock.Setup(factory => factory.CreateNewForUsers(user1, user2, _gameSettings)).Returns(_game);

            //Act
            Guid gameId = _service.CreateGameForUsers(user1, user2, _gameSettings);

            //Assert
            _gameFactoryMock.Verify(factory => factory.CreateNewForUsers(user1, user2, _gameSettings), Times.Once,
                "The 'CreateNewForUsers' method of the 'IGameFactory' is not called correctly.");

            Assert.That(gameId, Is.EqualTo(_game.Id), "The game id returned is not the same as the id of the game returned by the factory.");

            _gameRepositoryMock.Verify(repo => repo.Add(_game), Times.Once,
                "The 'Add' method of the 'IGameRepository' is not called correctly.");
        }

        [MonitoredTest("GetBoardDto - Should retrieve the game from the repository and extract a dto from it")]
        public void GetBoardDto_Should_RetrieveTheGameFromTheRepositoryAndExtractADtoFromIt()
        {
            //Arrange
            var boardDto = new BoardDto();
            _boardDtoFactoryMock.Setup(factory => factory.CreateFromBoard(It.IsAny<IBoard>())).Returns(boardDto);

            //Act
            BoardDto dto = _service.GetBoardDto(_game.Id);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            _boardDtoFactoryMock.Verify(factory => factory.CreateFromBoard(_game.Board), Times.Once,
                "The 'CreateFromBoard' method of the 'IBoardDtoFactory' is not called correctly.");

            Assert.That(dto, Is.SameAs(boardDto), "The dto returned is not the same dto created by the 'IBoardDtoFactory'.");
        }

        [MonitoredTest("GetPlayerGameDto - Should retrieve the game from the repository and extract a dto from it")]
        public void GetPlayerGameDto_Should_RetrieveTheGameFromTheRepositoryAndExtractADtoFromIt()
        {
            //Arrange
            Guid playerId = _game.RedPlayer.Id;
            var playerGameDto = new PlayerGameDto();
            _playerGameDtoFactoryMock.Setup(factory => factory.CreateFromGame(It.IsAny<IGame>(), It.IsAny<Guid>())).Returns(playerGameDto);

            //Act
            PlayerGameDto dto = _service.GetPlayerGameDto(_game.Id, playerId);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            _playerGameDtoFactoryMock.Verify(factory => factory.CreateFromGame(_game, playerId), Times.Once,
                "The 'CreateFromGame' method of the 'IPlayerGameDtoFactory' is not called correctly.");

            Assert.That(dto, Is.SameAs(playerGameDto), "The dto returned is not the same dto created by the 'IPlayerGameDtoFactory'.");
        }

        [MonitoredTest("PositionPiece - Should retrieve the game from the repository and position the piece")]
        public void PositionPiece_Should_RetrieveTheGameFromTheRepositoryAndPositionThePiece()
        {
            //Arrange
            Guid pieceId = Guid.NewGuid();
            Guid playerId = _game.BluePlayer.Id;
            var targetCoordinate = new BoardCoordinateBuilder().Build();

            Result expectedResult = Result.CreateSuccessResult();
            _gameMock.Setup(g => g.PositionPiece(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BoardCoordinate>())).Returns(expectedResult);

            //Act
            Result result = _service.PositionPiece(_game.Id, pieceId, playerId, targetCoordinate);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            _gameMock.Verify(g => g.PositionPiece(playerId, pieceId, targetCoordinate), Times.Once,
                "The 'PositionPiece' method of the game is not called correctly.");

            Assert.That(result, Is.SameAs(expectedResult),
                "The result returned is not the same result instance returned by the 'PositionPiece' method of the game.");
        }

        [MonitoredTest("SetPlayerReady - Should retrieve the game from the repository and mark the player as ready")]
        public void SetPlayerReady_Should_RetrieveTheGameFromTheRepositoryAndMarkThePlayerAsReady()
        {
            //Arrange
            Guid playerId = _game.RedPlayer.Id;

            //Act
            _service.SetPlayerReady(_game.Id, playerId);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            _gameMock.Verify(g => g.SetPlayerReady(playerId), Times.Once,
                "The 'SetPlayerReady' method of the game is not called correctly.");
        }

        [MonitoredTest("MovePiece - Should retrieve the game from the repository and move the piece")]
        public void MovePiece_Should_RetrieveTheGameFromTheRepositoryAndMoveThePiece()
        {
            //Arrange
            Guid pieceId = Guid.NewGuid();
            Guid playerId = _game.BluePlayer.Id;
            var targetCoordinate = new BoardCoordinateBuilder().Build();

            Result<Move> gameMoveResult = Result<Move>.CreateSuccessResult(new MoveBuilder().Build());
            _gameMock.Setup(g => g.MovePiece(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BoardCoordinate>())).Returns(gameMoveResult);

            //Act
            Result<MoveDto> result = _service.MovePiece(_game.Id, pieceId, playerId, targetCoordinate);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            _gameMock.Verify(g => g.MovePiece(playerId, pieceId, targetCoordinate), Times.Once,
                "The 'MovePiece' method of the game is not called correctly.");

            _gameMock.VerifyGet(g => g.LastMove, Times.Never,
                "Is is not necessary to read the 'LastMove' property of the game. The last move is returned by the 'MovePiece' method of the game.");

            Assert.That(result.IsSuccess, Is.True, "A success result should be returned.");

            AssertMoveDtoEquality(result.Value, gameMoveResult.Value);
        }

        [MonitoredTest("GetLastMove - Should retrieve the game from the repository and return the last move")]
        public void GetLastMove_Should_RetrieveTheGameFromTheRepositoryAndReturnTheLastMove()
        {
            //Arrange
            Move lastMove = new MoveBuilder().WithTargetPiece().Build();
            _gameMock.SetupGet(g => g.LastMove).Returns(lastMove);

            //Act
            MoveDto result = _service.GetLastMove(_game.Id);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            AssertMoveDtoEquality(result, lastMove);
        }

        private static void AssertMoveDtoEquality(MoveDto dto, Move move)
        {
            Assert.That(dto, Is.Not.Null, "The move dto returned should not be null.");
            Assert.That(dto.To, Is.EqualTo(move.To),
                "The move dto returned is not derived properly from the 'LastMove' of the game. Unexpected 'To' value.");
            Assert.That(dto.From, Is.EqualTo(move.From),
                "The move dto returned is not derived properly from the 'LastMove' of the game. Unexpected 'From' value.");
            Assert.That(dto.PlayerId, Is.EqualTo(move.PlayerId),
                "The move dto returned is not derived properly from the 'LastMove' of the game. Unexpected 'PlayerId' value.");
            Assert.That(dto.Piece.Id, Is.EqualTo(move.Piece.Id),
                "The move dto returned is not derived properly from the 'LastMove' of the game. Unexpected 'Piece' value.");
            Assert.That(dto.TargetPiece?.Id, Is.EqualTo(move.TargetPiece?.Id),
                "The move dto returned is not derived properly from the 'LastMove' of the game. Unexpected 'TargetPiece' value.");
        }
    }
}