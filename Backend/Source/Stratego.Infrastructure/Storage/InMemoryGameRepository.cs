using System;
using Stratego.AppLogic.Contracts;
using Stratego.Common;
using Stratego.Domain.Contracts;

namespace Stratego.Infrastructure.Storage
{
    /// <inheritdoc />
    /// <summary>
    /// Stores all the games in an in-memory dictionary.
    /// Games are removed from the dictionary automatically after 5 hours.
    /// </summary>
    /// <remarks>There should be no need to alter any code in this class.</remarks>
    internal class InMemoryGameRepository : IGameRepository
    {
        private readonly ExpiringDictionary<Guid, IGame> _gameDictionary;

        public InMemoryGameRepository()
        {
            _gameDictionary = new ExpiringDictionary<Guid, IGame>(TimeSpan.FromHours(5));
        }

        public void Add(IGame newGame)
        {
            _gameDictionary.AddOrReplace(newGame.Id, newGame);
        }

        public IGame GetById(Guid id)
        {
            if (_gameDictionary.TryGetValue(id, out IGame game))
            {
                return game;
            }
            throw new DataNotFoundException();
        }
    }
}