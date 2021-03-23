using System;
using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.Domain.BoardDomain
{
    /// <summary>
    /// Info about a move that was made in a game
    /// </summary>
    public class Move 
    {
        public BoardCoordinate From { get; }

        public BoardCoordinate To { get; }

        /// <summary>
        /// Id of the player (user) that made the move.
        /// </summary>
        public Guid PlayerId { get; }

        /// <summary>
        /// The piece that has been moved.
        /// </summary>
        public IPiece Piece { get; }

        /// <summary>
        /// The piece that was targeted with this move.
        /// If no piece was targeted, this will be null.
        /// </summary>
        public IPiece TargetPiece { get; internal set; }

        public Move(BoardCoordinate from, BoardCoordinate to, Guid playerId, IPiece piece)
        {
            
        }
    }
}