using System;
using System.Collections.Generic;
using Stratego.AppLogic.Contracts;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic
{
    /// <summary>
    /// Basic implementation if <see cref="IGameCandidateMatcher"/>.
    /// </summary>
    public class BasicGameCandidateMatcher : IGameCandidateMatcher
    {
        /// <summary>
        /// Chooses the first candidate in a list of possible opponents.
        /// </summary>
        /// <returns>
        /// null, if there are no possible opponents.
        /// Otherwise the first candidate in the list is returned.
        /// </returns>
        public IGameCandidate SelectOpponentToChallenge(IList<IGameCandidate> possibleOpponents)
        {
            throw new NotImplementedException();
        }
    }
}