using System;
using System.Linq;
using Guts.Client.Core;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Stratego.Common;
using Stratego.Domain.Contracts;
using Stratego.TestTools.Builders;

namespace Stratego.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "GameCandidate", @"Stratego.Domain\GameCandidate.cs")]
    public class GameCandidateTests
    {
        private User _user;
        private GameSettings _gameSettings;
        private GameCandidate _candidate;

        [SetUp]
        public void Setup()
        {
            _user = new UserBuilder().Build();
            _gameSettings = new GameSettingsBuilder().WithAutoMatching(true).Build();
            _candidate = new GameCandidate(_user, _gameSettings);
        }

        [MonitoredTest("Constructor - Should initialize properly")]
        public void Constructor_ShouldInitializeProperly()
        {
            //Assert
            Assert.That(_candidate.User, Is.SameAs(_user), "The 'User' property is not set correctly.");
            Assert.That(_candidate.GameSettings, Is.SameAs(_gameSettings), "The 'GameSettings' property is not set correctly.");
            Assert.That(_candidate.GameId, Is.EqualTo(Guid.Empty), "The 'GameId' should be an empty Guid.");
            Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(Guid.Empty), "The 'ProposedOpponentUserId' should be an empty Guid.");
        }

        [MonitoredTest("CanChallenge - Challenger already in game - Should return failure")]
        public void CanChallenge_ChallengerAlreadyInGame_ShouldReturnFailure()
        {
            //Arrange
            IGameCandidate target = new GameCandidateMockBuilder().WithSettings(_gameSettings).Object;
            _candidate.GameId = Guid.NewGuid();

            //Act
            Result result = _candidate.CanChallenge(target);

            //Assert
            Assert.That(result.IsFailure, Is.True, "A candidate cannot challenge when its 'GameId' has a non-empty value.");
            string expectedMessagePart = "game";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase.And.Contain(_user.NickName),
                $"The message of the result should contain '{expectedMessagePart}' and the nickname of the candidate.");
        }

        [MonitoredTest("CanChallenge - Challenged candidate already in game - Should return failure")]
        public void CanChallenge_ChallengedCandidateAlreadyInGame_ShouldReturnFailure()
        {
            //Arrange
            IGameCandidate target = new GameCandidateMockBuilder().WithSettings(_gameSettings).WithGameId().Object;
            
            //Act
            Result result = _candidate.CanChallenge(target);

            //Assert
            Assert.That(result.IsFailure, Is.True, "A candidate cannot be challenged when its 'GameId' has a non-empty value.");
            string expectedMessagePart = "game";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase.And.Contain(target.User.NickName),
                $"The message of the result should contain '{expectedMessagePart}' and the nickname of the challenged candidate.");
        }

        [MonitoredTest("CanChallenge - Challenge yourself - Should return failure")]
        public void CanChallenge_ChallengeYourself_ShouldReturnFailure()
        {
            //Act
            Result result = _candidate.CanChallenge(_candidate);

            //Assert
            Assert.That(result.IsFailure, Is.True, "A candidate cannot challenge himself.");
            string expectedMessagePart = "yourself";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("CanChallenge - Different game modes - Should return failure")]
        public void CanChallenge_DifferentGameModes_ShouldReturnFailure()
        {
            //Arrange
            GameSettings nonMatchingSettings =
                new GameSettingsBuilder().WithIsQuickGame(!_gameSettings.IsQuickGame).Build();
            IGameCandidate target = new GameCandidateMockBuilder().WithSettings(nonMatchingSettings).Object;

            //Act
            Result result = _candidate.CanChallenge(target);

            //Assert
            Assert.That(result.IsFailure, Is.True,
                "A candidate cannot be challenged when the 'IsQuickGame' property of the settings does not match.");
            string expectedMessagePart = "settings";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");
        }

        [MonitoredTest("CanChallenge - All validations pass - Should return success")]
        public void CanChallenge_AllValidationsPass_ShouldReturnSuccess()
        {
            //Arrange
            IGameCandidate target = new GameCandidateMockBuilder().WithSettings(_gameSettings).Object;

            //Act
            Result result = _candidate.CanChallenge(target);

            //Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [MonitoredTest("Challenge - Should use CanChallenge method")]
        public void Challenge_ShouldUseCanChallengeMethod()
        {
            var code = Solution.Current.GetFileContent(@"Stratego.Domain\GameCandidate.cs");
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText == "Challenge");

            Assert.That(method, Is.Not.Null, "Could not find the 'Challenge' method.");
            
            var body = CodeCleaner.StripComments(method.Body.ToString());

            Assert.That(body, Contains.Substring("CanChallenge("),
                "You must use the 'CanChallenge' method to check if the challenge can be made.");
        }

        [MonitoredTest("Challenge - Should set the proposed opponent and return success")]
        public void Challenge_ShouldSetTheProposedOpponentAndReturnSuccess()
        {
            //Arrange
            IGameCandidate target = new GameCandidateMockBuilder().WithSettings(_gameSettings).Object;

            //Act
            Result result = _candidate.Challenge(target);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned.");
            Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(target.User.Id),
                "The 'ProposedOpponentUserId' of the challenger should the user id of the challenged candidate.");
        }

        [MonitoredTest("Challenge - Invalid challenge - Should return failure")]
        public void Challenge_InvalidChallenge_ShouldReturnFailure()
        {
            //Arrange
            GameSettings nonMatchingSettings =
                new GameSettingsBuilder().WithIsQuickGame(!_gameSettings.IsQuickGame).Build();
            IGameCandidate target = new GameCandidateMockBuilder().WithSettings(nonMatchingSettings).WithGameId().Object;
            _candidate.GameId = Guid.NewGuid();

            //Act
            Result result = _candidate.Challenge(target);

            //Assert
            Assert.That(result.IsFailure, Is.True, "A failure result should be returned.");
            Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(Guid.Empty),
                "The 'ProposedOpponentUserId' of the challenger should be empty.");
        }

        [MonitoredTest("AcceptChallenge - Already in game - Should return failure")]
        public void AcceptChallenge_AlreadyInGame_ShouldReturnFailure()
        {
            //Arrange
            IGameCandidate challenger = new GameCandidateMockBuilder()
                .WithSettings(_gameSettings)
                .WithProposedOpponentUserId(_user.Id)
                .Object;
            _candidate.GameId = Guid.NewGuid();

            //Act
            Result result = _candidate.AcceptChallenge(challenger);

            //Assert
            Assert.That(result.IsFailure, Is.True, "A failure result should be returned.");
            string expectedMessagePart = "game";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");

            Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(Guid.Empty),
                "The 'ProposedOpponentUserId' of the candidate should remain empty.");
        }

        [MonitoredTest("AcceptChallenge - Challenge was for other candidate - Should return failure")]
        public void AcceptChallenge_ChallengeWasForOtherCandidate_ShouldReturnFailure()
        {
            //Arrange
            Guid otherUserId = Guid.NewGuid();
            IGameCandidate challenger = new GameCandidateMockBuilder()
                .WithSettings(_gameSettings)
                .WithProposedOpponentUserId(otherUserId)
                .Object;

            //Act
            Result result = _candidate.AcceptChallenge(challenger);

            //Assert
            Assert.That(result.IsFailure, Is.True, "A failure result should be returned.");
            string expectedMessagePart = "accept challenge";
            Assert.That(result.Message, Does.Contain(expectedMessagePart).IgnoreCase,
                $"The message of the result should contain '{expectedMessagePart}'.");

            Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(Guid.Empty),
                "The 'ProposedOpponentUserId' of the candidate should remain empty.");
        }

        [MonitoredTest("AcceptChallenge - Valid challenge - Should set proposed opponent and return success")]
        public void AcceptChallenge_ValidChallenge_ShouldSetProposedOpponentAndReturnSuccess()
        {
            //Arrange
            IGameCandidate challenger = new GameCandidateMockBuilder()
                .WithSettings(_gameSettings)
                .WithProposedOpponentUserId(_user.Id)
                .Object;

            //Act
            Result result = _candidate.AcceptChallenge(challenger);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A success result should be returned.");

            Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(challenger.User.Id),
                "The 'ProposedOpponentUserId' of the candidate should be the user id of the challenger.");
        }
    }
}