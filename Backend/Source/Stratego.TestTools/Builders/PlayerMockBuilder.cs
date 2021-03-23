using System;
using Moq;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.Contracts;

namespace Stratego.TestTools.Builders
{
    public class PlayerMockBuilder : MockBuilder<IPlayer>
    {
        private static readonly Random RandomGenerator = new Random();

        public Mock<IArmy> ArmyMock { get; }

        public PlayerMockBuilder()
        {
            Mock.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            Mock.SetupGet(p => p.IsRed).Returns(RandomGenerator.NextBool());
            Mock.SetupProperty(p => p.IsReady, RandomGenerator.NextBool());
            ArmyMock = new Mock<IArmy>();
            Mock.SetupGet(p => p.Army).Returns(ArmyMock.Object);
        }

        public PlayerMockBuilder WithIsRed(bool isRed)
        {
            Mock.SetupGet(p => p.IsRed).Returns(isRed);
            return this;
        }

        public PlayerMockBuilder WithIsReady(bool isReady)
        {
            Mock.SetupProperty(p => p.IsReady, isReady);
            return this;
        }
    }
}