using System;
using Stratego.Domain.BoardDomain;

namespace Stratego.Api.Models
{
    /// <summary>
    /// Encapsulates the movement of a piece
    /// </summary>
    public class MoveModel
    {
        /// <summary>
        /// Id (guid) of the piece.
        /// </summary>
        public Guid PieceId { get; set; }

        /// <summary>
        /// The coordinate where the piece should be moved to.
        /// </summary>
        public BoardCoordinate TargetCoordinate { get; set; }
    }
}