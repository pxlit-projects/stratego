using System.Collections.Generic;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic.Contracts
{
    /// <summary>
    /// Service that can choose an opponent from a list of possible candidates.
    /// </summary>
    public interface IGameCandidateMatcher
    {
        /// <summary>
        /// Selects an opponent from a list of possible candidates.
        /// </summary>
        IGameCandidate SelectOpponentToChallenge(IList<IGameCandidate> possibleOpponents);
    }
}