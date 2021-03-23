using System;
using Stratego.Domain.BoardDomain;

namespace Stratego.AppLogic.Dto
{
    /// <summary>
    /// Information about a move
    /// </summary>
    public class MoveDto
    {
        public BoardCoordinate From { get; set; }

        public BoardCoordinate To { get; set; }

        /// <summary>
        /// Id of the player (user) that made the move.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The piece that has been moved.
        /// </summary>
        public PieceDto Piece { get; set; }

        /// <summary>
        /// The piece that was targeted with this move.
        /// If no piece was targeted, this will be null.
        /// </summary>
        public PieceDto TargetPiece { get; set; }

        public static MoveDto FromMove(Move move)
        {
            if (move == null) return null;
            return new MoveDto
            {
                From = move.From,
                To = move.To,
                PlayerId = move.PlayerId,
                Piece = PieceDto.FromPiece(move.Piece),
                TargetPiece = PieceDto.FromPiece(move.TargetPiece)
            };
        }
    }
}