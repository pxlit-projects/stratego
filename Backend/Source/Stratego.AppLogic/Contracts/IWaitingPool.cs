using System;
using System.Collections.Generic;
using Stratego.Common;
using Stratego.Domain;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic.Contracts
{
    /// <summary>
    /// Service that maintains a pool of candidates waiting for a game.
    /// </summary>
    public interface IWaitingPool
    {
        /// <summary>
        /// Adds a user as a candidate to the waiting pool
        /// </summary>
        /// <param name="user">The user to be added</param>
        /// <param name="gameSettings">
        /// The settings the user desires for a game
        /// (only candidates with similar settings should be matched with this user).</param>
        void Join(User user, GameSettings gameSettings);

        /// <summary>
        /// Removes a user from the waiting pool
        /// </summary>
        /// <param name="userId">The identifier of the user that was used to created the candidate.</param>
        void Leave(Guid userId);

        /// <summary>
        /// Gets a candidate from the waiting pool
        /// </summary>
        /// <param name="userId">The identifier of the user that was used to create the candidate.</param>
        IGameCandidate GetCandidate(Guid userId);

        #region Extra

        /// <summary>
        /// EXTRA: Challenge another candidate in the waiting pool.
        /// This method should be used for candidates that don't have auto-matching enabled (in their game settings). 
        /// </summary>
        /// <param name="challengerUserId">Identifier of the user (candidate) that is challenging (the challenger).</param>
        /// <param name="targetUserId">Identifier of the user (candidate) that is challenged.</param>
        /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
        Result Challenge(Guid challengerUserId, Guid targetUserId);

        /// <summary>
        /// EXTRA: Withdraw the challenge a user has made.
        /// </summary>
        /// <param name="userId">The identifier of the user that has challenged a candidate.</param>
        /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
        void WithdrawChallenge(Guid userId);

        /// <summary>
        /// EXTRA: Retrieves all the candidates in the pool that are waiting for a game and have matching game settings.
        /// </summary>
        /// <param name="userId">The identifier of the user that wants to challenge other candidates.</param>
        /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
        IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId);

        /// <summary>
        /// EXTRA: Retrieves a list of candidates that challenged a particular user (candidate).
        /// </summary>
        /// <param name="challengedUserId">The identifier of the user that is challenged by other candidates.</param>
        /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
        IList<IGameCandidate> FindChallengesFor(Guid challengedUserId);

        #endregion
    }
}
