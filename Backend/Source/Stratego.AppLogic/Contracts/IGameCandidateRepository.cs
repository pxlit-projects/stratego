using System;
using System.Collections.Generic;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic.Contracts
{
    /// <summary>
    /// Repository that stores all game candidates of the application
    /// </summary>
    public interface IGameCandidateRepository
    {
        /// <summary>
        /// Adds a candidate.
        /// If the candidate is already present in de repository, it is overwritten.
        /// </summary>
        void AddOrReplace(IGameCandidate candidate);

        /// <summary>
        /// Removes a candidate from the repository.
        /// </summary>
        /// <param name="userId">The user identifier of the candidate.</param>
        void RemoveCandidate(Guid userId);

        /// <summary>
        /// Retrieves a candidate from the repository.
        /// </summary>
        /// <param name="userId">The user identifier of the candidate.</param>
        IGameCandidate GetCandidate(Guid userId);

        /// <summary>
        /// Retrieves all the candidates that can be challenged by a certain candidate.
        /// </summary>
        /// <param name="userId">The user identifier of the candidate that wants to challenge somebody.</param>
        IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId);

        #region EXTRA

        /// <summary>
        /// Retrieves all the candidates that challenged a certain candidate.
        /// </summary>
        /// <param name="challengedUserId">The user identifier of the candidate that was challenged.</param>
        IList<IGameCandidate> FindChallengesFor(Guid challengedUserId);

        #endregion
    }
}