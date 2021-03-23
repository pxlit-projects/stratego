using System;
using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.Domain.ArmyDomain
{
    /// <inheritdoc />
    public class MoveablePiece : Piece
    {
        public override void Attack(IPiece opponent)
        {
            throw new NotImplementedException();
        }
    }
}