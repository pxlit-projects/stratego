using System;
using System.Collections.Generic;

namespace Stratego.Domain.ArmyDomain.Contracts
{
    /// <summary>
    /// An army of pieces of a player.
    /// </summary>
    public interface IArmy
    {
        /// <summary>
        /// Indicates that all pieces of the army have a board position set.
        /// </summary>
        bool IsPositionedOnBoard { get; }

        /// <summary>
        /// True if the flag is captured (not alive) or if there are no more pieces alive that can move.
        /// False otherwise.
        /// </summary>
        bool IsDefeated { get; }

        IPiece GetPieceById(Guid id);

        /// <summary>
        /// Get all pieces that are alive or all pieces that are dead.
        /// </summary>
        IList<IPiece> FindPieces(bool alive);
    }
}