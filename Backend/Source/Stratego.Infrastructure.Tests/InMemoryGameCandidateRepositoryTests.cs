using System;
using System.Collections.Generic;
using Guts.Client.Core;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;
using Stratego.Common;
using Stratego.Domain.Contracts;
using Stratego.Infrastructure.Storage;
using Stratego.TestTools.Builders;

namespace Stratego.Infrastructure.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "GameCandidateRepo", @"Stratego.Infrastructure\Storage\InMemoryGameCandidateRepository.cs")]
    public class InMemoryGameCandidateRepositoryTests
    {
        private static readonly Random RandomGenerator = new Random();
        private InMemoryGameCandidateRepository _repo;

        [SetUp]
        public void Setup()
        {
            _repo = new InMemoryGameCandidateRepository();
        }

        [MonitoredTest("FindCandidatesThatCanBeChallengedBy - Repo contains candidates that can be challenged - Should return them")]
        public void FindCandidatesThatCanBeChallengedBy_RepoContainsCandidatesThatCanBeChallenged_ShouldReturnThem()
        {
            //Arrange
            var challengerBuilder = new GameCandidateMockBuilder();
            Mock<IGameCandidate> challengerMock = challengerBuilder.Mock;
            challengerMock.Setup(c => c.CanChallenge(It.IsAny<IGameCandidate>()))
                .Returns(Result.CreateFailureResult("This candidate cannot be challenged"));
            IGameCandidate challenger = challengerMock.Object;
            _repo.AddOrReplace(challenger);

            var unChallengeables = new List<IGameCandidate>();
            for (int i = 0; i < RandomGenerator.Next(1, 11); i++)
            {
                var candidate = new GameCandidateMockBuilder().Mock.Object;
                unChallengeables.Add(candidate);
                _repo.AddOrReplace(candidate);
            }
            var challengeables = new List<IGameCandidate>();
            for (int i = 0; i < RandomGenerator.Next(1, 11); i++)
            {
                var candidate = new GameCandidateMockBuilder().Mock.Object;
                challengerMock.Setup(c => c.CanChallenge(candidate))
                    .Returns(Result.CreateSuccessResult());
                challengeables.Add(candidate);
                _repo.AddOrReplace(candidate);
            }

            //Act
            IList<IGameCandidate> result = _repo.FindCandidatesThatCanBeChallengedBy(challenger.User.Id);

            //Assert
            Assert.That(result, Is.Not.Null, "No list was returned.");
            challengerMock.Verify(c => c.CanChallenge(It.IsAny<IGameCandidate>()),
                Times.AtLeast(challengeables.Count + unChallengeables.Count),
                "The 'CanChallenge' method of the challenger should have been called multiple times.");
            foreach (IGameCandidate candidate in result)
            {
                Assert.That(challengeables.Contains(candidate), "At least one candidate that could be challenged is not returned.");
            }
        }

        [MonitoredTest("FindCandidatesThatCanBeChallengedBy - Repo contains no candidates that can be challenged - Should return empty list")]
        public void FindCandidatesThatCanBeChallengedBy_RepoContainsNoCandidatesThatCanBeChallenged_ShouldReturnEmptyList()
        {
            //Arrange
            var challengerBuilder = new GameCandidateMockBuilder();
            Mock<IGameCandidate> challengerMock = challengerBuilder.Mock;
            challengerMock.Setup(c => c.CanChallenge(It.IsAny<IGameCandidate>()))
                .Returns(Result.CreateFailureResult("This candidate cannot be challenged"));
            IGameCandidate challenger = challengerMock.Object;

            _repo.AddOrReplace(challenger);

            var unChallengeables = new List<IGameCandidate>();
            for (int i = 0; i < RandomGenerator.Next(1, 11); i++)
            {
                IGameCandidate candidate = new GameCandidateMockBuilder().Mock.Object;
                unChallengeables.Add(candidate);
                _repo.AddOrReplace(candidate);
            }
            
            //Act
            IList<IGameCandidate> result = _repo.FindCandidatesThatCanBeChallengedBy(challenger.User.Id);

            //Assert
            Assert.That(result, Is.Not.Null, "No list was returned.");
            Assert.That(result, Is.Empty, "The list returned is not empty.");
        }
    }
}