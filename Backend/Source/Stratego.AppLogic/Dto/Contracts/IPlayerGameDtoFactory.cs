using System;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic.Dto.Contracts
{
    /// <summary>
    /// Factory that can create a data transfer object (dto) that contains info about a game from the perspective of a player.
    /// </summary>
    internal interface IPlayerGameDtoFactory
    {
        /// <summary>
        /// Creates a dto from a game.
        /// The dto contains only information that a certain player may see.
        /// </summary>
        PlayerGameDto CreateFromGame(IGame game, Guid playerId);
    }
}