using System;
using Stratego.Common;

namespace Stratego.Domain.Contracts
{
    /// <summary>
    /// Represents a user that wants to play a game.
    /// </summary>
    public interface IGameCandidate
    {
        User User { get; }

        /// <summary>
        /// The desired game settings.
        /// A candidate will be matched with other candidates that have similar settings.
        /// </summary>
        GameSettings GameSettings { get; }

        /// <summary>
        /// If a matching candidate is found and a game is created for the 2 candidates, then this property will contain the id of the created game.
        /// If no game is created yet, this property should be an empty Guid.
        /// </summary>
        Guid GameId { get; set; }

        /// <summary>
        /// When the candidate challenges another candidate, this property will contain the user identifier of the challenged candidate.
        /// </summary>
        Guid ProposedOpponentUserId { get; }

        /// <summary>
        /// Indicates if the candidate may challenge a target candidate.
        /// Challenging a candidate should only be possible if:
        /// <list type="bullet">
        /// <item><description>No game is created for the candidate</description></item>
        /// <item><description>No game is created for the target</description></item>
        /// <item><description>The target is not the candidate himself</description></item>
        /// <item><description>The settings match (e.g. both must have the same IsQuickGame property</description></item>
        /// </list>
        /// </summary>
        /// <param name="targetCandidate">The candidate that would be challenged.</param>
        /// <returns>
        /// A success result if challenging is allowed.
        /// A failure result (with a reason) if challenging is not allowed.
        /// </returns>
        Result CanChallenge(IGameCandidate targetCandidate);

        /// <summary>
        /// Challenge a target candidate.
        /// Should set the <see cref="ProposedOpponentUserId"/> property if it is allowed (<see cref="CanChallenge"/>) to challenge the target.
        /// </summary>
        /// <param name="targetCandidate">The candidate that is being challenged.</param>
        /// <returns>
        /// A success result if the challenge succeeded.
        /// A failure result (with a reason) if challenging is not allowed.
        /// </returns>
        Result Challenge(IGameCandidate targetCandidate);

        /// <summary>
        /// Accepts a challenge from another candidate.
        /// Should set the <see cref="ProposedOpponentUserId"/> property to the identifier of the challenger.
        /// </summary>
        /// <param name="challenger">The candidate that challenged this candidate.</param>
        /// <returns>
        /// A failure result (with a reason) if the candidate is already in a game or the proposed opponent id of the challenger is not the identifier of this candidate.
        /// Otherwise a success result is returned.
        /// </returns>
        Result AcceptChallenge(IGameCandidate challenger);

        /// <summary>
        /// Clears the <see cref="ProposedOpponentUserId"/> of the candidate.
        /// </summary>
        void WithdrawChallenge();
    }
}