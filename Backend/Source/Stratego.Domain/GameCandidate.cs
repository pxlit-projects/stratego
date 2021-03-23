using System;
using Stratego.Common;
using Stratego.Domain.Contracts;

namespace Stratego.Domain
{
    /// <inheritdoc />
    internal class GameCandidate : IGameCandidate
    {
        public User User { get; }
        public GameSettings GameSettings { get; }
        public Guid ProposedOpponentUserId { get; }
        public Guid GameId { get; set; }

        internal GameCandidate(User user, GameSettings gameSettings)
        {
           
        }

        public Result CanChallenge(IGameCandidate targetCandidate)
        {
            throw new NotImplementedException();
        }

        public Result Challenge(IGameCandidate targetCandidate)
        {
            throw new NotImplementedException();
        }

        public Result AcceptChallenge(IGameCandidate challenger)
        {
            throw new NotImplementedException();
        }

        public void WithdrawChallenge()
        {
            throw new NotImplementedException();
        }
    }
}