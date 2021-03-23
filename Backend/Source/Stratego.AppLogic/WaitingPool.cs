using System;
using System.Collections.Generic;
using Stratego.AppLogic.Contracts;
using Stratego.Common;
using Stratego.Domain;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic
{
    /// <inheritdoc />
    internal class WaitingPool : IWaitingPool
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameCandidateFactory">
        /// Factory that can be used to create a game candidate</param>
        /// <param name="gameCandidateRepository">
        /// Repository that can be used to store and retrieve game candidates.
        /// An instance of InMemoryGameCandidateRepository (Stratego.Infrastructure/Storage) will be injected here.
        /// </param>
        /// <param name="gameCandidateMatcher">
        /// Service that can be used to select a possible opponent for a candidate from the candidates that are waiting in the pool.
        /// </param>
        /// <param name="gameService">
        /// Service that can be used to create a game when 2 candidates in the pool found each other.
        /// </param>
        public WaitingPool(
            IGameCandidateFactory gameCandidateFactory,
            IGameCandidateRepository gameCandidateRepository, 
            IGameCandidateMatcher gameCandidateMatcher, 
            IGameService gameService)
        {
            //TODO: store the factory, repository, matcher and service in field variables so that they can be used in other methods of this class.
        }

        public void Join(User user, GameSettings gameSettings)
        {
            //TODO: create a candidate for the user
            //TODO: add the candidate to the repository
            //TODO: try to automatically match the candidate with another candidate waiting in the pool
            //TODO: if a match is made -> create a game
        }

        public void Leave(Guid userId)
        {
            //TODO: remove the candidate from the repository
        }

        public IGameCandidate GetCandidate(Guid userId)
        {
            //TODO: retrieve the candidate from the repository
            throw new NotImplementedException();
        }

        #region EXTRA

        public Result Challenge(Guid challengerUserId, Guid targetUserId)
        {
            throw new NotImplementedException();
        }

        public void WithdrawChallenge(Guid userId)
        {
            throw new NotImplementedException();
        }

        public IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId)
        {
            throw new NotImplementedException();
        }

        public IList<IGameCandidate> FindChallengesFor(Guid challengedUserId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}