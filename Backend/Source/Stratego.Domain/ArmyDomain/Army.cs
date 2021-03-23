using System;
using System.Collections.Generic;
using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.Domain.ArmyDomain
{
    /// <inheritdoc />
    public class Army : IArmy
    {
        public bool IsPositionedOnBoard { get; }
        public bool IsDefeated { get; }

        /// <summary>
        /// Creates the pieces of the army.
        /// 10 pieces for a quick game.
        /// EXTRA: 40 pieces for a normal game.
        /// </summary>
        internal Army(bool isQuickGame)
        {
            
        }

        public IPiece GetPieceById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IList<IPiece> FindPieces(bool alive)
        {
            throw new NotImplementedException();
        }
    }
}