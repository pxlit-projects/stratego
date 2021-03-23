using System;
using Stratego.AppLogic.Dto.Contracts;
using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.AppLogic.Dto
{
    /// <inheritdoc />
    internal class BoardDtoFactory : IBoardDtoFactory
    {
        public BoardDto CreateFromBoard(IBoard board)
        {
            throw new NotImplementedException();
        }
    }
}