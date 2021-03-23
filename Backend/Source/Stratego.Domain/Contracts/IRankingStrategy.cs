using System.Collections.Generic;

namespace Stratego.Domain.Contracts
{
    /// <summary>
    /// EXTRA: Can adjust the ranking of users based on the result of a played game.
    /// </summary>
    /// <remarks>This strategy is needed for an EXTRA. Not needed to implement the minimal requirements.</remarks>
    public interface IRankingStrategy
    {
        /// <summary>
        /// Adjusts the rank (and score) of the users ranked near the winner and loser of a game.
        /// </summary>
        /// <param name="userSlice">
        /// List of users that are ranked near the winner and loser. The ranking (and score) of these users may be impacted.
        /// All users ranked between the winner and loser are included,
        /// but also the user ranked one higher than the highest ranked user and the user ranked one lower than the lowest ranked user.
        /// The list of users is sorted by rank (descending)
        /// </param>
        /// <param name="winner">User in the <paramref name="userSlice"/> that has won.</param>
        /// <param name="loser">User in the <paramref name="userSlice"/> that has lost.</param>
        void AdjustRanking(IList<User> userSlice, User winner, User loser);
    }
}