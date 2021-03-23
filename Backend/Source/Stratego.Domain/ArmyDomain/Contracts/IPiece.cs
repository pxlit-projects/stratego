using System;
using Stratego.Domain.BoardDomain;

namespace Stratego.Domain.ArmyDomain.Contracts
{
    /// <summary>
    /// A piece of an army on the board (like flag, miner, general, ...).
    /// </summary>
    public interface IPiece
    {
        /// <summary>
        /// Unique identifier of the piece. Should be set on construction.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Friendly name of the piece. E.g. Bomb, Marshal, ...
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Bomb: 11, Marshal: 10, ...., Scout: 2, Spy: 1, Flag: 0
        /// </summary>
        int Strength { get; }

        /// <summary>
        /// Position of the piece on the board.
        /// Should be null when the piece is not positioned on a board.
        /// </summary>
        BoardCoordinate Position { get; set; }

        bool IsAlive { get; set; }

        /// <summary>
        /// Indicates if the piece can make moves.
        /// Should be false for flags and bombs.
        /// Should be true for all other pieces.
        /// </summary>
        bool IsMoveable { get; }

        /// <summary>
        /// Indicates if a piece can move over multiple squares.
        /// Should only be true for scouts.
        /// </summary>
        bool CanMoveAnyDistance { get; }

        /// <summary>
        /// Attack another piece.
        /// After the attack the <see cref="IsAlive"/> properties of the piece and the attacked piece should reflect the outcome of the battle.
        /// </summary>
        /// <param name="opponent">The piece being attacked.</param>
        void Attack(IPiece opponent);
    }
}