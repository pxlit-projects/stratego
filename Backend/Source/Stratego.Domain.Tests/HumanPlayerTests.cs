using System;
using Guts.Client.Core;
using Guts.Client.Shared;
using NUnit.Framework;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.TestTools;
using Stratego.TestTools.Builders;

namespace Stratego.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "HumanPlayer", @"Stratego.Domain\HumanPlayer.cs")]
    public class HumanPlayerTests
    {
        private static readonly Random RandomGenerator = new Random();
        private ArmyMockBuilder _armyMockBuilder;
        private HumanPlayer _humanPlayer;

        [SetUp]
        public void Setup()
        {
            _armyMockBuilder = new ArmyMockBuilder();
            _humanPlayer = new HumanPlayer(Guid.NewGuid(), Guid.NewGuid().ToString(), RandomGenerator.NextBool(), _armyMockBuilder.Object);
        }

        [MonitoredTest("Constructor - Should initialize properly")]
        public void Constructor_ShouldInitializeProperly()
        {
            //Arrange
            Guid playerId = Guid.NewGuid();
            string nickName = Guid.NewGuid().ToString();
            bool isRed = RandomGenerator.NextBool();
            IArmy army = _armyMockBuilder.Object;

            //Act
            var humanPlayer = new HumanPlayer(playerId, nickName, isRed, army);

            //Assert
            Assert.That(humanPlayer.Id, Is.EqualTo(playerId), "The 'Id' property is not set correctly.");
            Assert.That(humanPlayer.NickName, Is.EqualTo(nickName), "The 'NickName' property is not set correctly.");
            Assert.That(humanPlayer.IsRed, Is.EqualTo(isRed), "The 'IsRed' property is not set correctly.");
            Assert.That(humanPlayer.Army, Is.SameAs(army), "The 'Army' property is not set correctly.");
            Assert.That(humanPlayer.IsReady, Is.False, "The 'IsReady' property should be false.");
        }

        [MonitoredTest("IsReady - Set true - Army not positioned yet - Should throw ApplicationException")]
        public void IsReady_SetTrue_ArmyNotPositionedYet_ShouldThrowApplicationException()
        {
            //Arrange
            _armyMockBuilder.WithArmyPositioned(false);

            //Act + Assert
            Assert.That(() => _humanPlayer.IsReady = true, Throws.InstanceOf<ApplicationException>());
        }

        [MonitoredTest("IsReady - Set true - Army not positioned - Should succeed")]
        public void IsReady_SetTrue_ArmyPositioned_ShouldSucceed()
        {
            //Arrange
            _armyMockBuilder.WithArmyPositioned(true);

            //Act
            _humanPlayer.IsReady = true;

            Assert.That(_humanPlayer.IsReady, Is.True);
        }
    }
}