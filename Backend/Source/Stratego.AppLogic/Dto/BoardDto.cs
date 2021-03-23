namespace Stratego.AppLogic.Dto
{
    /// <summary>
    /// Information about the game board.
    /// </summary>
    public class BoardDto
    {
        /// <summary>
        /// Matrix of the board squares (top-left = (0,0), bottom-right = (Size - 1, Size - 1)).
        /// </summary>
        public BoardSquareDto[][] Squares { get; set; }

        /// <summary>
        /// Size of the board (10 for a normal game, 8 for a quick game)
        /// </summary>
        public int Size { get; set; }
    }
}