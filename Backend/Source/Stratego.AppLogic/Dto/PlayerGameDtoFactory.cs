using System;
using Stratego.AppLogic.Dto.Contracts;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic.Dto
{
    /// <inheritdoc />
    public class PlayerGameDtoFactory : IPlayerGameDtoFactory
    {
        public PlayerGameDto CreateFromGame(IGame game, Guid playerId)
        {
            throw new NotImplementedException();
        }
    }
}