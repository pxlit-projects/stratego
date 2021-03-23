using System;
using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.TestTools.Builders
{
    public class ArmyMockBuilder : MockBuilder<IArmy>
    {
        private static readonly Random RandomGenerator = new Random();

        public ArmyMockBuilder()
        {
            Mock.SetupGet(a => a.IsPositionedOnBoard).Returns(RandomGenerator.NextBool());
            Mock.SetupGet(a => a.IsDefeated).Returns(RandomGenerator.NextBool());
        }

        public ArmyMockBuilder WithArmyPositioned(bool isPositioned)
        {
            Mock.SetupGet(a => a.IsPositionedOnBoard).Returns(isPositioned);
            return this;
        }
    }
}