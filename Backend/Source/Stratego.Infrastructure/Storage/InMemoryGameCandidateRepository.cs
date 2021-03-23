using System;
using System.Collections.Generic;
using Stratego.AppLogic.Contracts;
using Stratego.Common;
using Stratego.Domain.Contracts;

namespace Stratego.Infrastructure.Storage
{
    /// <inheritdoc />
    /// <summary>
    /// Stores all the game candidates in an in-memory dictionary.
    /// Candidates are removed from the dictionary automatically after 10-minutes.
    /// </summary>
    internal class InMemoryGameCandidateRepository : IGameCandidateRepository
    {
        private readonly ExpiringDictionary<Guid, IGameCandidate> _candidateDictionary;

        public InMemoryGameCandidateRepository()
        {
            _candidateDictionary = new ExpiringDictionary<Guid, IGameCandidate>(TimeSpan.FromMinutes(10));
        }

        public void AddOrReplace(IGameCandidate candidate)
        {
            _candidateDictionary.AddOrReplace(candidate.User.Id, candidate);
        }

        public void RemoveCandidate(Guid userId)
        {
            _candidateDictionary.TryRemove(userId, out IGameCandidate _);
        }

        public IGameCandidate GetCandidate(Guid userId)
        {
            if (_candidateDictionary.TryGetValue(userId, out IGameCandidate candidate))
            {
                return candidate;
            }
            throw new DataNotFoundException();
        }

        public IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId)
        {
            //TODO: retrieve the candidate with userId as key in the _candidateDictionary (use the TryGetValue method)

            //TODO: loop over all candidates (user the Values property of _candidateDictionary)
            //and check if those candidates can be challenged by the candidate you retrieved in the first step (use the CanChallenge method of IGameCandidate).
            //Put the candidates that can be challenged in a list and return that list.

            throw new NotImplementedException(
                $"The {nameof(FindCandidatesThatCanBeChallengedBy)} method of the {nameof(InMemoryGameCandidateRepository)} class is not implemented.");
        }

        #region EXTRA

        public IList<IGameCandidate> FindChallengesFor(Guid challengedUserId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}