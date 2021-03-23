using System;
using Stratego.Common;
using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.Domain.BoardDomain.Contracts
{
    /// <summary>
    /// Board that contains a matrix of squares.
    /// Can position and move pieces.
    /// </summary>
    public interface IBoard
    {
        IBoardSquare[,] Squares { get; }

        int Size { get; }

        /// <summary>
        /// Tries to position a piece on the board.
        /// </summary>
        /// <param name="piece">he piece that should be positioned.</param>
        /// <param name="targetCoordinate">The target coordinate on the board on which the piece should be positioned.</param>
        /// <param name="playerIsRed">Indicates if it is the red or blue player that is positioning the piece.</param>
        /// <returns>
        /// A success result if the positioning succeeded.
        /// A failure result (with a reason) if the positioning did not succeed.
        /// </returns>
        Result PositionPiece(IPiece piece, BoardCoordinate targetCoordinate, bool playerIsRed);

        /// <summary>
        /// Tries to move a piece on the board.
        /// </summary>
        /// <param name="piece">The piece that should be moved.</param>
        /// <param name="targetCoordinate">The target position of the piece.</param>
        /// <param name="playerId">The identifier of the player that is moving the piece.</param>
        /// <returns>
        /// A move object that contains details about the move (if the move succeeded).
        /// A failure result (with a reason) if the move did not succeed.
        /// </returns>
        Result<Move> MovePiece(IPiece piece, BoardCoordinate targetCoordinate, Guid playerId);
    }
}