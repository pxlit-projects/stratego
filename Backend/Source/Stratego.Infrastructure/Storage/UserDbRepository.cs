using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.AppLogic.Contracts;
using Stratego.Domain;
using Stratego.Domain.Contracts;

namespace Stratego.Infrastructure.Storage
{
    /// <summary>
    /// EXTRA: Repository to retrieve and manipulate users in the database
    /// </summary>
    /// <remarks>
    /// This repository is needed for some EXTRAS. Not needed to implement the minimal requirements.
    /// This repository is also fully implemented. Normally you do not need to change code here.
    /// </remarks>
    internal class UserDbRepository : IUserRepository
    {
        private readonly StrategoDbContext _context;
        private readonly IRankingStrategy _rankingStrategy;

        public UserDbRepository(StrategoDbContext context, IRankingStrategy rankingStrategy)
        {
            _context = context;
            _rankingStrategy = rankingStrategy;
        }

        public void AdjustRankingAfterGame(Guid winnerUserId, Guid loserUserId)
        {
            //DO NOT CHANGE THIS METHOD!! Implement the necessary logic in the RankingStrategy class.
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                User winner = _context.Users.Find(winnerUserId);
                User loser = _context.Users.Find(winnerUserId);

                int fromRank = Math.Min(loser.Rank, winner.Rank) - 1;
                int toRank = Math.Max(loser.Rank, winner.Rank) + 1;

                IList<User> userSlice =
                    _context.Users.Where(u => u.Rank >= fromRank && u.Rank <= toRank).OrderByDescending(u => u.Rank)
                        .ToList();

                _rankingStrategy.AdjustRanking(userSlice, winner, loser);

                _context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }

        public IList<User> GetFriends(Guid userId)
        {
            var query = (from friendShip in _context.Friendships
                         where friendShip.Friend1Id == userId && friendShip.ConfirmedByFriend1 && friendShip.ConfirmedByFriend2
                         select friendShip.Friend2)
                .Union(
                    from friendShip in _context.Friendships
                    where friendShip.Friend2Id == userId && friendShip.ConfirmedByFriend1 && friendShip.ConfirmedByFriend2
                    select friendShip.Friend1
                );

            return query.ToList();
        }

        public IList<User> GetUnconfirmedFriends(Guid userId)
        {
            var query = (from friendShip in _context.Friendships
                    where friendShip.Friend1Id == userId && !friendShip.ConfirmedByFriend1
                    select friendShip.Friend2)
                .Union(
                    from friendShip in _context.Friendships
                    where friendShip.Friend2Id == userId && !friendShip.ConfirmedByFriend2
                    select friendShip.Friend1
                );

            return query.ToList();
        }

        public void AddNewFriendRequest(Guid initiatorUserId, Guid targetUserid)
        {
            if (GetFriendship(initiatorUserId, targetUserid) != null)
            {
                throw new ApplicationException("A friendship between these users already exist.");
            }

            var friendship = new Friendship
            {
                Friend1Id = initiatorUserId, 
                Friend2Id = targetUserid, 
                ConfirmedByFriend1 = true,
                ConfirmedByFriend2 = false
            };
            _context.Friendships.Add(friendship);
            _context.SaveChanges();
        }

        public void ConfirmFriendship(Guid confirmingUserId, Guid otherUserId)
        {
            Friendship friendship = GetFriendship(confirmingUserId, otherUserId);
            if (friendship == null)
            {
                throw new ApplicationException("No friendship can be found between these users.");
            }

            if (confirmingUserId == friendship.Friend1Id)
            {
                friendship.ConfirmedByFriend1 = true;
            }
            if (confirmingUserId == friendship.Friend2Id)
            {
                friendship.ConfirmedByFriend2 = true;
            }
            _context.SaveChanges();
        }

        public void RemoveFriendship(Guid user1Id, Guid user2Id)
        {
            Friendship friendshipToRemove = GetFriendship(user1Id, user2Id);
            if (friendshipToRemove != null)
            {
                _context.Friendships.Remove(friendshipToRemove);
                _context.SaveChanges();
            }
        }

        private Friendship GetFriendship(Guid user1Id, Guid user2Id)
        {
            return _context.Friendships.FirstOrDefault(f => (f.Friend1Id == user1Id && f.Friend2Id == user2Id) ||
                                                            (f.Friend1Id == user2Id && f.Friend2Id == user1Id));
        }
    }
}