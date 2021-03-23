using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.Domain.BoardDomain.Contracts
{
    /// <summary>
    /// A square on a <see cref="Board"/>.
    /// </summary>
    public interface IBoardSquare
    {
        /// <summary>
        /// Indicates if the square is an obstacle through which pieces cannot move.
        /// </summary>
        bool IsObstacle { get; set; }

        /// <summary>
        /// Indicates if the square may be used to position a red player piece on (before the game is started)
        /// </summary>
        bool IsRedHomeTerritory { get; }

        /// <summary>
        /// Indicates if the square may be used to position a blue player piece on (before the game is started)
        /// </summary>
        bool IsBlueHomeTerritory { get; }

        /// <summary>
        /// The piece that is currently on the square.
        /// Null if the square is empty.
        /// </summary>
        IPiece Piece { get; set; }
    }
}