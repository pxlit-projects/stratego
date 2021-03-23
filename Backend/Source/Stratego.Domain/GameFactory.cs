using System;
using Stratego.Domain.Contracts;

namespace Stratego.Domain
{
    /// <inheritdoc />
    public class GameFactory : IGameFactory
    {
        public IGame CreateNewForUsers(User user1, User user2, GameSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}