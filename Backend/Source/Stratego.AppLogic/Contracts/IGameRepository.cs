using System;
using Stratego.Domain.Contracts;

namespace Stratego.AppLogic.Contracts
{
    /// <summary>
    /// Repository that stores all the games of the application
    /// </summary>
    public interface IGameRepository
    {
        void Add(IGame newGame);
        IGame GetById(Guid id);
    }
}