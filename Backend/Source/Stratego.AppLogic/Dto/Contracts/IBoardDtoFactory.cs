using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.AppLogic.Dto.Contracts
{
    /// <summary>
    /// Factory that can create a data transfer object (dto) that contains info about a board that can be converted to json.
    /// </summary>
    /// <remarks>
    /// This is needed in this project because an <see cref="IBoard"/> contains a 2-dimensional array of squares.
    /// 2-dimensional arrays cannot be converted to json (and be sent to a browser), but a jagged array can.
    /// </remarks>
    internal interface IBoardDtoFactory
    {
        /// <summary>
        /// Creates a dto from a board.
        /// </summary>
        BoardDto CreateFromBoard(IBoard board);
    }
}