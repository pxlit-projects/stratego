using System;
using System.Collections.Generic;
using Stratego.Domain;

namespace Stratego.AppLogic.Contracts
{
    /// <summary>
    /// EXTRA: Repository to retrieve and manipulate users in the storage medium (e.g. database)
    /// </summary>
    /// <remarks>This repository is needed for some EXTRAS. Not needed to implement the minimal requirements.</remarks>
    public interface IUserRepository
    {
        void AdjustRankingAfterGame(Guid winnerUserId, Guid loserUserId);
        IList<User> GetFriends(Guid userId);
        IList<User> GetUnconfirmedFriends(Guid userId);
        void AddNewFriendRequest(Guid initiatorUserId, Guid targetUserid);
        void ConfirmFriendship(Guid confirmingUserId, Guid otherUserId);
        void RemoveFriendship(Guid user1Id, Guid user2Id);
    }
}