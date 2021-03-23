using System;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.BoardDomain;

namespace Stratego.AppLogic.Dto
{
    /// <summary>
    /// Information about a piece
    /// </summary>
    public class PieceDto
    {
        public Guid Id { get; set; }

        //Name of the piece (Spy, Flag, General, ...)
        public string Name { get; set; }

        /// <summary>
        /// Strength of the piece (Bomb: 11, Marshal 10, General: 9, ... , Scout: 2, Spy: 1, Flag: 0
        /// </summary>
        public int Strength { get; set; }

        /// <summary>
        /// Position of the piece on the board.
        /// When the piece is not on the board, the position is null.
        /// </summary>
        public BoardCoordinate Position { get; set; }

        /// <summary>
        /// Indicates if the piece is alive
        /// </summary>
        public bool IsAlive { get; set; }

        public static PieceDto FromPiece(IPiece piece)
        {
            if (piece == null) return null;
            return new PieceDto
            {
                Id = piece.Id,
                Name = piece.Name,
                Strength = piece.Strength,
                Position = piece.Position,
                IsAlive = piece.IsAlive
            };
        }
    }
}