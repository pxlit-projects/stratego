using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.AppLogic.Dto
{
    /// <summary>
    /// Information about a square on the game board
    /// </summary>
    public class BoardSquareDto
    {
        /// <summary>
        /// If true, the square is not accessible for pieces.
        /// </summary>
        public bool IsObstacle { get; set; }

        /// <summary>
        /// If true, a red piece can be placed on this square during game setup (the positioning of pieces).
        /// </summary>
        public bool IsRedHomeTerritory { get; set; }

        /// <summary>
        /// If true, a blue piece can be placed on this square during game setup (the positioning of pieces).
        /// </summary>
        public bool IsBlueHomeTerritory { get; set; }

        public BoardSquareDto(IBoardSquare square)
        {
            IsObstacle = square.IsObstacle;
            IsRedHomeTerritory = square.IsRedHomeTerritory;
            IsBlueHomeTerritory = square.IsBlueHomeTerritory;
        }
    }
}