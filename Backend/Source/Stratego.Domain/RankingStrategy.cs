using System.Collections.Generic;
using Stratego.Domain.Contracts;

namespace Stratego.Domain
{
    /// <inheritdoc />
    /// <remarks>This strategy is needed for an EXTRA. Not needed to implement the minimal requirements.</remarks>
    public class RankingStrategy : IRankingStrategy
    {
        public void AdjustRanking(IList<User> userSlice, User winner, User loser)
        {
            throw new System.NotImplementedException();
        }
    }
}