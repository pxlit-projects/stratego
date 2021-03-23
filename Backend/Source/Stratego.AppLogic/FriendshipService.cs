using System;
using System.Collections.Generic;
using Stratego.AppLogic.Contracts;
using Stratego.Common;
using Stratego.Domain;

namespace Stratego.AppLogic
{
    /// <inheritdoc cref="IFriendshipService"/>
    /// <remarks>This service is needed for an EXTRA. Not needed to implement the minimal requirements.</remarks>
    internal class FriendshipService : IFriendshipService
    {
        public FriendshipService(IUserRepository userRepository)
        {
        }

        public IList<User> GetFriends(Guid userId)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetFriendRequests(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Result DoFriendRequest(Guid fromUserId, Guid toUserId)
        {
            throw new NotImplementedException();
        }

        public Result ConfirmFriendship(Guid confirmingUserId, Guid otherUserId)
        {
            throw new NotImplementedException();
        }

        public Result RemoveFriendship(Guid user1Id, Guid user2Id)
        {
            throw new NotImplementedException();
        }
    }
}