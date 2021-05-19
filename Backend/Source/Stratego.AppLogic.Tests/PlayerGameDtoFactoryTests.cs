using System;
using System.Collections.Generic;
using System.Linq;
using Guts.Client.Core;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;
using Stratego.AppLogic.Dto;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.Contracts;
using Stratego.TestTools;
using Stratego.TestTools.Builders;

namespace Stratego.AppLogic.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "PlayerGameDtoFactory", @"Stratego.AppLogic\Dto\PlayerGameDtoFactory.cs;Stratego.AppLogic\Dto\PlayerGameDto.cs")]
    public class PlayerGameDtoFactoryTests
    {
        private static readonly Random RandomGenerator = new Random();

        private PlayerGameDtoFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new PlayerGameDtoFactory();
        }

        [MonitoredTest("CreateFromGame - Uses a game to get the info from the perspective of the player")]
        public void CreateFromGame_UsesAGameToGetTheInfoFromThePerspectiveOfThePlayer()
        {
            //Arrange
            GameMockBuilder gameMockBuilder = new GameMockBuilder();
            Mock<IGame> gameMock = gameMockBuilder.Mock;
            IGame game = gameMock.Object;

            bool redIsPositioned = RandomGenerator.NextBool();
            gameMockBuilder.RedPlayerMockBuilder.ArmyMock.SetupGet(a => a.IsPositionedOnBoard).Returns(redIsPositioned);
            bool blueIsPositioned = RandomGenerator.NextBool();
            gameMockBuilder.BluePlayerMockBuilder.ArmyMock.SetupGet(a => a.IsPositionedOnBoard).Returns(blueIsPositioned);

            gameMock.Setup(g => g.IsPlayersTurn(game.RedPlayer.Id)).Returns(true);
            gameMock.Setup(g => g.IsPlayersTurn(game.BluePlayer.Id)).Returns(false);

            var ownLivingPieces = new List<IPiece>
            {
                new PieceMockBuilder().Mock.Object,
                new PieceMockBuilder().Mock.Object
            };
            gameMockBuilder.RedPlayerMockBuilder.ArmyMock.Setup(a => a.FindPieces(true)).Returns(ownLivingPieces);

            var ownFallenPieces = new List<IPiece>
            {
                new PieceMockBuilder().Mock.Object,
                new PieceMockBuilder().Mock.Object
            };
            gameMockBuilder.RedPlayerMockBuilder.ArmyMock.Setup(a => a.FindPieces(false)).Returns(ownFallenPieces);

            var opponentLivingPieces = new List<IPiece>
            {
                new PieceMockBuilder().Mock.Object,
                new PieceMockBuilder().Mock.Object
            };
            gameMockBuilder.BluePlayerMockBuilder.ArmyMock.Setup(a => a.FindPieces(true)).Returns(opponentLivingPieces);

            var opponentFallenPieces = new List<IPiece>
            {
                new PieceMockBuilder().Mock.Object,
                new PieceMockBuilder().Mock.Object
            };
            gameMockBuilder.BluePlayerMockBuilder.ArmyMock.Setup(a => a.FindPieces(false)).Returns(opponentFallenPieces);

            bool isRedPlayerReady = RandomGenerator.NextBool();
            gameMockBuilder.RedPlayerMockBuilder.Mock.SetupGet(p => p.IsReady).Returns(isRedPlayerReady);

            bool isBluePlayerReady = RandomGenerator.NextBool();
            gameMockBuilder.BluePlayerMockBuilder.Mock.SetupGet(p => p.IsReady).Returns(isBluePlayerReady);

            bool redIsDefeated = RandomGenerator.NextBool();
            bool blueIsDefeated = RandomGenerator.NextBool();
            gameMockBuilder.RedPlayerMockBuilder.ArmyMock.SetupGet(a => a.IsDefeated).Returns(redIsDefeated);
            gameMockBuilder.BluePlayerMockBuilder.ArmyMock.SetupGet(a => a.IsDefeated).Returns(blueIsDefeated);
            bool isOver = RandomGenerator.NextBool();
            gameMock.SetupGet(g => g.IsOver).Returns(isOver);
            bool isStarted = RandomGenerator.NextBool();
            gameMock.SetupGet(g => g.IsStarted).Returns(isStarted);

            gameMockBuilder.RedPlayerMockBuilder.Mock.SetupGet(p => p.IsRed).Returns(true);

            //Act
            PlayerGameDto dto = _factory.CreateFromGame(game, game.RedPlayer.Id);

            //Assert
            Assert.That(dto, Is.Not.Null, "No instance of a class that implements PlayerGameDto is returned.");
            Assert.That(dto.Id, Is.EqualTo(game.Id), "The Id should be the Id of the game.");

            gameMock.Verify(g => g.IsPlayersTurn(game.RedPlayer.Id), Times.Once, "The 'IsPlayersTurn' of the game is not called correctly.");
            Assert.That(dto.IsYourTurn, Is.True, $"The 'IsYourTurn' property should be true for the player with id '{game.RedPlayer.Id}'");

            gameMock.VerifyGet(g => g.IsOver, Times.Once, "The 'IsOver' of the game is not called correctly.");
            Assert.That(dto.IsOver, Is.EqualTo(isOver),
                $"'IsOver' should be {isOver} when 'IsOver' property of the game is {isOver}.");

            gameMock.VerifyGet(g => g.IsStarted, Times.Once, "The 'IsStarted' of the game is not called correctly.");
            Assert.That(dto.IsStarted, Is.EqualTo(isStarted),
                $"'IsStarted' should be {isStarted} when 'IsStarted' property of the game is {isStarted}.");

            AssertOwnPlayerProperties(gameMockBuilder, dto, isRedPlayerReady, ownLivingPieces, ownFallenPieces, redIsDefeated);
            AssertOpponentProperties(gameMockBuilder, dto, isBluePlayerReady, opponentLivingPieces, opponentFallenPieces, blueIsDefeated);
        }

        private void AssertOwnPlayerProperties(GameMockBuilder gameMockBuilder, PlayerGameDto dto, bool isRedPlayerReady,
            List<IPiece> ownLivingPieces, List<IPiece> ownFallenPieces, bool redIsDefeated)
        {
            Mock<IGame> gameMock = gameMockBuilder.Mock;
            IGame game = gameMock.Object;

            gameMock.Verify(g => g.GetPlayerById(game.RedPlayer.Id), Times.Once,
                "The player should be retrieved using the GetPlayerById method of the game correctly.");

            gameMockBuilder.RedPlayerMockBuilder.ArmyMock.Verify(a => a.FindPieces(true), Times.Once,
                "'FindPieces(true)' should have been called on the army of the player.");

            gameMockBuilder.RedPlayerMockBuilder.Mock.VerifyGet(p => p.IsReady, Times.Once,
                "The 'IsReady' property of the player is not called correctly.");
            Assert.That(dto.OwnPlayerIsReady, Is.EqualTo(isRedPlayerReady),
                $"'OwnPlayerIsReady' should be {isRedPlayerReady} when 'IsReady' property of the player is {isRedPlayerReady}.");

            Assert.That(dto.OwnLivingPieces, Is.Not.Null, "'OwnLivingPieces' should not be null.");

            foreach (PieceDto pieceDto in dto.OwnLivingPieces)
            {
                IPiece matchingPiece = ownLivingPieces.FirstOrDefault(p => p.Id == pieceDto.Id);
                Assert.That(matchingPiece, Is.Not.Null,
                    "Not all living pieces of the player army can be found in the 'OwnLivingPieces' property of the dto.");
                Assert.That(pieceDto.Name, Is.EqualTo(matchingPiece.Name),
                    "Invalid 'Name' for a piece dto in the 'OwnLivingPieces' property of the dto");
                Assert.That(pieceDto.Position, Is.EqualTo(matchingPiece.Position),
                    "Invalid 'Position' for a piece dto in the 'OwnLivingPieces' property of the dto");
                Assert.That(pieceDto.Strength, Is.EqualTo(matchingPiece.Strength),
                    "Invalid 'Strength' for a piece dto in the 'OwnLivingPieces' property of the dto");
            }

            gameMockBuilder.RedPlayerMockBuilder.ArmyMock.Verify(a => a.FindPieces(false), Times.Once,
                "'FindPieces(false)' should have been called on the army of the player.");

            Assert.That(dto.OwnFallenPieces, Is.Not.Null, "'OwnFallenPieces' should not be null.");

            foreach (PieceDto pieceDto in dto.OwnFallenPieces)
            {
                IPiece matchingPiece = ownFallenPieces.FirstOrDefault(p => p.Id == pieceDto.Id);
                Assert.That(matchingPiece, Is.Not.Null,
                    "Not all fallen pieces of the player army can be found in the 'OwnFallenPieces' property of the dto.");
                Assert.That(pieceDto.Name, Is.EqualTo(matchingPiece.Name),
                    "Invalid 'Name' for a piece dto in the 'OwnFallenPieces' property of the dto");
                Assert.That(pieceDto.Position, Is.EqualTo(matchingPiece.Position),
                    "Invalid 'Position' for a piece dto in the 'OwnFallenPieces' property of the dto");
                Assert.That(pieceDto.Strength, Is.EqualTo(matchingPiece.Strength),
                    "Invalid 'Strength' for a piece dto in the 'OwnFallenPieces' property of the dto");
            }

            gameMockBuilder.RedPlayerMockBuilder.Mock.VerifyGet(p => p.IsRed, Times.Once,
                "The 'IsRed' of the player is not called.");
            Assert.That(dto.OwnColorIsRed, Is.True, "'OwnColorIsRed' should be true.");

            gameMockBuilder.RedPlayerMockBuilder.ArmyMock.VerifyGet(a => a.IsDefeated, Times.Once,
                "The 'IsDefeated' of the player army is not called correctly.");
            Assert.That(dto.OwnArmyIsDefeated, Is.EqualTo(redIsDefeated),
                $"'OwnArmyIsDefeated' should be {redIsDefeated} when 'IsDefeated' property of the player army is {redIsDefeated}.");
        }

        private void AssertOpponentProperties(GameMockBuilder gameMockBuilder, PlayerGameDto dto, bool isBluePlayerReady,
            List<IPiece> opponentLivingPieces, List<IPiece> opponentFallenPieces, bool blueIsDefeated)
        {
            Mock<IGame> gameMock = gameMockBuilder.Mock;
            IGame game = gameMock.Object;

            gameMock.Verify(g => g.GetOpponent(game.RedPlayer), Times.Once,
                "The opponent should be retrieved using the GetOpponent method of the game correctly.");

            gameMockBuilder.BluePlayerMockBuilder.Mock.VerifyGet(p => p.IsReady, Times.Once,
                "The 'IsReady' property of the opponent is not called correctly.");
            Assert.That(dto.OpponentIsReady, Is.EqualTo(isBluePlayerReady),
                $"'OpponentIsReady' should be {isBluePlayerReady} when 'IsReady' property of the opponent is {isBluePlayerReady}.");

            gameMockBuilder.BluePlayerMockBuilder.ArmyMock.Verify(a => a.FindPieces(true), Times.Once,
                "'FindPieces(true)' should have been called on the army of the opponent.");
            Assert.That(dto.OpponentLivingPieceCoordinates,
                Has.All.Matches((BoardCoordinate coordinate) => opponentLivingPieces.Any(p => p.Position == coordinate)),
                "The 'OpponentLivingPieceCoordinates' are not correct.");

            gameMockBuilder.BluePlayerMockBuilder.ArmyMock.Verify(a => a.FindPieces(false), Times.Once,
                "'FindPieces(false)' should have been called on the army of the opponent.");

            Assert.That(dto.OwnFallenPieces, Is.Not.Null, "'OpponentFallenPieces' should not be null.");

            foreach (PieceDto pieceDto in dto.OpponentFallenPieces)
            {
                IPiece matchingPiece = opponentFallenPieces.FirstOrDefault(p => p.Id == pieceDto.Id);
                Assert.That(matchingPiece, Is.Not.Null,
                    "Not all fallen pieces of the opponent army can be found in the 'OpponentFallenPieces' property of the dto.");
                Assert.That(pieceDto.Name, Is.EqualTo(matchingPiece.Name),
                    "Invalid 'Name' for a piece dto in the 'OpponentFallenPieces' property of the dto");
                Assert.That(pieceDto.Position, Is.EqualTo(matchingPiece.Position),
                    "Invalid 'Position' for a piece dto in the 'OpponentFallenPieces' property of the dto");
                Assert.That(pieceDto.Strength, Is.EqualTo(matchingPiece.Strength),
                    "Invalid 'Strength' for a piece dto in the 'OpponentFallenPieces' property of the dto");
            }

            gameMockBuilder.BluePlayerMockBuilder.ArmyMock.VerifyGet(a => a.IsDefeated, Times.Once,
                "The 'IsDefeated' of the opponent army is not called correctly.");
            Assert.That(dto.OpponentArmyIsDefeated, Is.EqualTo(blueIsDefeated),
                $"'OpponentArmyIsDefeated' should be {blueIsDefeated} when 'IsDefeated' property of the opponent army is {blueIsDefeated}.");
        }
    }
}