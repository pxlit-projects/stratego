using System;
using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.Domain.Contracts
{
    public interface IPlayer
    {
        Guid Id { get; }
        string NickName { get; }
        IArmy Army { get; }
        bool IsRed { get; }
        bool IsReady { get; set; }
    }
}