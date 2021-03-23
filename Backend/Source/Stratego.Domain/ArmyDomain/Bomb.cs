using System;
using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.Domain.ArmyDomain
{
    public class Bomb : Piece
    {
        public override void Attack(IPiece opponent)
        {
            throw new NotImplementedException();
        }
    }
}