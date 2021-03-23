using System;
using System.Collections.Generic;
using Stratego.Common;
using Stratego.Domain;

namespace Stratego.AppLogic.Contracts
{
    /// <summary>
    /// EXTRA: Service to manage friendships between users
    /// </summary>
    /// <remarks>This service is needed for an EXTRA. Not needed to implement the minimal requirements.</remarks>
    public interface IFriendshipService
    {
        IList<User> GetFriends(Guid userId);
        IList<User> GetFriendRequests(Guid userId);
        Result DoFriendRequest(Guid fromUserId, Guid toUserId);
        Result ConfirmFriendship(Guid confirmingUserId, Guid otherUserId);
        Result RemoveFriendship(Guid user1Id, Guid user2Id);
    }
}