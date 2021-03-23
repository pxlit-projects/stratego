using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.Domain.BoardDomain
{
    /// <inheritdoc />
    public class BoardSquare : IBoardSquare
    {
        public bool IsObstacle { get; set; }
        public bool IsRedHomeTerritory { get; }
        public bool IsBlueHomeTerritory { get; }
        public IPiece Piece { get; set; }

        public BoardSquare(bool isRedHomeTerritory, bool isBlueHomeTerritory)
        {
            
        }
    }
}