using System;
using Moq;
using Stratego.Common;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.Contracts;

namespace Stratego.TestTools.Builders
{
    public class GameMockBuilder : MockBuilder<IGame>
    {
        public PlayerMockBuilder RedPlayerMockBuilder { get; }
        public PlayerMockBuilder BluePlayerMockBuilder { get; }

        public GameMockBuilder()
        {
            Mock.SetupGet(g => g.Id).Returns(Guid.NewGuid());

            RedPlayerMockBuilder = new PlayerMockBuilder().WithIsRed(true);
            BluePlayerMockBuilder = new PlayerMockBuilder().WithIsRed(false);
            IPlayer redPlayer = RedPlayerMockBuilder.Mock.Object;
            IPlayer bluePlayer = BluePlayerMockBuilder.Mock.Object;
            WithPlayers(redPlayer, bluePlayer);
        }

        public GameMockBuilder WithPlayers(IPlayer redPlayer, IPlayer bluePlayer)
        {
            Mock.SetupGet(g => g.RedPlayer).Returns(redPlayer);
            Mock.SetupGet(g => g.BluePlayer).Returns(bluePlayer);
            Mock.Setup(g => g.GetPlayerById(redPlayer.Id)).Returns(redPlayer);
            Mock.Setup(g => g.GetPlayerById(bluePlayer.Id)).Returns(bluePlayer);
            Mock.Setup(g => g.GetOpponent(redPlayer)).Returns(bluePlayer);
            Mock.Setup(g => g.GetOpponent(bluePlayer)).Returns(redPlayer);
            Mock.Setup(g => g.PositionPiece(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BoardCoordinate>())).Returns(Result.CreateSuccessResult());
            return this;
        }
    }
}