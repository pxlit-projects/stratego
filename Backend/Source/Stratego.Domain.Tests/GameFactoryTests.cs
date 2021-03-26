using Guts.Client.Core;
using Guts.Client.Shared;
using NUnit.Framework;
using Stratego.Domain.ArmyDomain;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.Contracts;
using Stratego.TestTools.Builders;

namespace Stratego.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "GameFactory", @"Stratego.Domain\GameFactory.cs")]
    public class GameFactoryTests
    {
        private GameFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new GameFactory();
        }

        [MonitoredTest("CreateNewForUsers - Should create game with 2 human players")]
        public void CreateNewForUsers_ShouldCreateGameWith2HumanPlayers()
        {
            //Arrange
            User user1 = new UserBuilder().Build();
            User user2 = new UserBuilder().Build();
            GameSettings settings = new GameSettingsBuilder().Build();

            //Act
            IGame game = _factory.CreateNewForUsers(user1, user2, settings);

            //Assert
            Assert.That(game.RedPlayer, Is.InstanceOf<HumanPlayer>(), "The red player should be an instance of 'HumanPlayer'.");
            Assert.That(game.RedPlayer.Id, Is.EqualTo(user1.Id), "The 'Id' of the red player be the id of user1.");
            Assert.That(game.RedPlayer.NickName, Is.EqualTo(user1.NickName), "The 'NickName' of the red player should be the nickname of user1.");
            Assert.That(game.RedPlayer.IsRed, Is.True, "The 'IsRed' of the red player should be true.");
            Assert.That(game.RedPlayer.Army, Is.InstanceOf<Army>(), "The army of the red player should be an instance of 'Army'.");

            Assert.That(game.BluePlayer, Is.InstanceOf<HumanPlayer>(), "The blue player should be an instance of 'HumanPlayer'.");
            Assert.That(game.BluePlayer.Id, Is.EqualTo(user2.Id), "The 'Id' of the blue player be the id of user2.");
            Assert.That(game.BluePlayer.NickName, Is.EqualTo(user2.NickName), "The 'NickName' of the blue player should be the nickname of user2.");
            Assert.That(game.BluePlayer.IsRed, Is.False, "The 'IsRed' of the blue player should be false.");
            Assert.That(game.BluePlayer.Army, Is.InstanceOf<Army>(), "The army of the blue player should be an instance of 'Army'.");

            Assert.That(game.Board, Is.InstanceOf<Board>(), "The board should be an instance of 'Board'.");
        }
    }
}