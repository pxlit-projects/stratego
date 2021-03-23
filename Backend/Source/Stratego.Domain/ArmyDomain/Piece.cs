using System;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.BoardDomain;

namespace Stratego.Domain.ArmyDomain
{
    /// <inheritdoc />
    /// <remarks>
    /// This abstract class should contain the logic for generating the <see cref="Id"/> of the piece.
    /// It also can have an implementation for the <see cref="IsMoveable"/> property (In the <see cref="Bomb"/> and <see cref="Flag"/> classes you will have to override this implementation)
    /// It also can have an implementation for the <see cref="CanMoveAnyDistance"/> property (In the <see cref="Scout"/> class you will have to override this implementation)
    /// </remarks>
    public abstract class Piece : IPiece
    {
        public Guid Id { get; }
        public string Name { get; protected set; }
        public int Strength { get; protected set; }
        public BoardCoordinate Position { get; set; }
        public bool IsAlive { get; set; }
        public bool IsMoveable { get; }
        public bool CanMoveAnyDistance { get; }

        public abstract void Attack(IPiece opponent);
    }
}