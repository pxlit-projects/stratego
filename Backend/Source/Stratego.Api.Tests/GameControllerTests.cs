using System;
using System.Collections.Generic;
using System.Security.Claims;
using Guts.Client.Core;
using Guts.Client.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Stratego.Api.Controllers;
using Stratego.Api.Models;
using Stratego.Api.Tests.Builders;
using Stratego.AppLogic.Contracts;
using Stratego.AppLogic.Dto;
using Stratego.Common;
using Stratego.Domain;
using Stratego.Domain.BoardDomain;
using Stratego.TestTools.Builders;

namespace Stratego.Api.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "GameCtlr", @"Stratego.Api\Controllers\GameController.cs")]
    public class GameControllerTests
    {
        private GameController _controller;
        private Mock<IGameService> _gameServiceMock;
        private User _loggedInUser;

        [SetUp]
        public void Setup()
        {
            _gameServiceMock = new Mock<IGameService>();
            _controller = new GameController(_gameServiceMock.Object);

            _loggedInUser = new UserBuilder().Build();
            var userClaimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, _loggedInUser.Id.ToString())
                })
            );
            var context = new ControllerContext { HttpContext = new DefaultHttpContext() };
            context.HttpContext.User = userClaimsPrincipal;
            _controller.ControllerContext = context;
        }

        [MonitoredTest("GetPlayerGame - Should retrieve a PlayerGameDto from the game service")]
        public void GetPlayerGame_ShouldRetrieveAPlayerGameDtoFromTheGameService()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();
            var dto = new PlayerGameDto();
            _gameServiceMock.Setup(service => service.GetPlayerGameDto(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(dto);

            //Act
            var result = _controller.GetPlayerGame(gameId) as OkObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.GetPlayerGameDto(gameId, _loggedInUser.Id), Times.Once,
                "The 'GetPlayerGameDto' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            Assert.That(result.Value, Is.SameAs(dto), "The result should contain the dto returned by the game service");
        }

        [MonitoredTest("GetBoard - Should retrieve a BoardDto from the game service")]
        public void GetBoard_ShouldRetrieveABoardDtoFromTheGameService()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();
            var dto = new BoardDto();
            _gameServiceMock.Setup(service => service.GetBoardDto(It.IsAny<Guid>()))
                .Returns(dto);

            //Act
            var result = _controller.GetBoard(gameId) as OkObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.GetBoardDto(gameId), Times.Once,
                "The 'GetBoardDto' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            Assert.That(result.Value, Is.SameAs(dto), "The result should contain the dto returned by the game service");
        }

        [MonitoredTest("PositionPiece - Valid move - Should use the game service and return Ok")]
        public void PositionPiece_ValidMove_ShouldUseTheGameServiceAndReturnOk()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();
            MoveModel validMove = new MoveModelBuilder().Build();
            var successResult = Result.CreateSuccessResult();
            _gameServiceMock.Setup(service => service.PositionPiece(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BoardCoordinate>()))
                .Returns(successResult);

            //Act
            var result = _controller.PositionPiece(gameId, validMove) as OkResult;

            //Assert
            _gameServiceMock.Verify(service => service.PositionPiece(gameId, validMove.PieceId, _loggedInUser.Id, validMove.TargetCoordinate), Times.Once,
                "The 'PositionPiece' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");
        }

        [MonitoredTest("PositionPiece - Invalid move - Should use the game service and return bad request")]
        public void PositionPiece_InvalidMove_ShouldUseTheGameServiceAndReturnBadRequest()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();
            MoveModel invalidMove = new MoveModelBuilder().Build();
            string errorMessage = $"Error from game service: {Guid.NewGuid()}";
            var failureResult = Result.CreateFailureResult(errorMessage);
            _gameServiceMock.Setup(service => service.PositionPiece(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BoardCoordinate>()))
                .Returns(failureResult);

            //Act
            var result = _controller.PositionPiece(gameId, invalidMove) as BadRequestObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.PositionPiece(gameId, invalidMove.PieceId, _loggedInUser.Id, invalidMove.TargetCoordinate), Times.Once,
                "The 'PositionPiece' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'BadRequestObjectResult' should be returned.");
            var errorModel = result.Value as ErrorModel;
            Assert.That(errorModel, Is.Not.Null, "The bad request result should contain an instance of ErrorModel.");
            Assert.That(errorModel.Message, Is.EqualTo(errorMessage),
                "The error model in the bad request result should contain the error message returned from the game service");
        }

        [MonitoredTest("SetPlayerReady - Should use the game service and return Ok")]
        public void SetPlayerReady_ShouldUseTheGameServiceAndReturnOk()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();

            //Act
            var result = _controller.SetPlayerReady(gameId) as OkResult;

            //Assert
            _gameServiceMock.Verify(service => service.SetPlayerReady(gameId, _loggedInUser.Id), Times.Once,
                "The 'SetPlayerReady' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");
        }

        [MonitoredTest("MovePiece - Valid move - Should use the game service and return the move")]
        public void MovePiece_ValidMove_ShouldUseTheGameServiceAndReturnTheMove()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();
            MoveModel validMoveModel = new MoveModelBuilder().Build();
            MoveDto resultingMove = new MoveDto();
            var successResult = Result<MoveDto>.CreateSuccessResult(resultingMove);
            _gameServiceMock.Setup(service => service.MovePiece(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BoardCoordinate>()))
                .Returns(successResult);

            //Act
            var result = _controller.MovePiece(gameId, validMoveModel) as OkObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.MovePiece(gameId, validMoveModel.PieceId, _loggedInUser.Id, validMoveModel.TargetCoordinate), Times.Once,
                "The 'MovePiece' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            Assert.That(result.Value, Is.SameAs(resultingMove), "The result should contain the move returned by the game service");
        }

        [MonitoredTest("MovePiece - Invalid move - Should use the game service and return bad request")]
        public void MovePiece_InValidMove_ShouldUseTheGameServiceAndReturnBadRequest()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();
            MoveModel invalidMoveModel = new MoveModelBuilder().Build();
            string errorMessage = $"Error from game service: {Guid.NewGuid()}";
            var failureResult = Result<MoveDto>.CreateFailureResult(errorMessage);
            _gameServiceMock.Setup(service => service.MovePiece(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BoardCoordinate>()))
                .Returns(failureResult);

            //Act
            var result = _controller.MovePiece(gameId, invalidMoveModel) as BadRequestObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.MovePiece(gameId, invalidMoveModel.PieceId, _loggedInUser.Id, invalidMoveModel.TargetCoordinate), Times.Once,
                "The 'MovePiece' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'BadRequestObjectResult' should be returned.");
            var errorModel = result.Value as ErrorModel;
            Assert.That(errorModel, Is.Not.Null, "The bad request result should contain an instance of ErrorModel.");
            Assert.That(errorModel.Message, Is.EqualTo(errorMessage),
                "The error model in the bad request result should contain the error message returned from the game service");
        }

        [MonitoredTest("GetLastMove - Move was made - Should use the game service and return the last move")]
        public void GetLastMove_MoveWasMade_ShouldUseTheGameServiceAndReturnTheLastMove()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();
            MoveDto lastMove = new MoveDto();
            _gameServiceMock.Setup(service => service.GetLastMove(It.IsAny<Guid>()))
                .Returns(lastMove);

            //Act
            var result = _controller.GetLastMove(gameId) as OkObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.GetLastMove(gameId), Times.Once,
                "The 'GetLastMove' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            Assert.That(result.Value, Is.SameAs(lastMove), "The result should contain the move returned by the game service");
        }

        [MonitoredTest("GetLastMove - No move made yet - Should use the game service and return not found")]
        public void GetLastMove_NoMoveMadeYet_ShouldUseTheGameServiceAndReturnNotFound()
        {
            //Arrange
            Guid gameId = Guid.NewGuid();
            _gameServiceMock.Setup(service => service.GetLastMove(It.IsAny<Guid>()))
                .Returns(() => null);

            //Act
            var result = _controller.GetLastMove(gameId) as NotFoundResult;

            //Assert
            _gameServiceMock.Verify(service => service.GetLastMove(gameId), Times.Once,
                "The 'GetLastMove' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'NotFoundResult' should be returned.");
        }
    }
}