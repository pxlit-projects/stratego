using System;
using Stratego.Domain.Contracts;

namespace Stratego.Domain
{
    /// <inheritdoc />
    public class GameCandidateFactory : IGameCandidateFactory
    {
        public IGameCandidate CreateNewForUser(User user, GameSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}