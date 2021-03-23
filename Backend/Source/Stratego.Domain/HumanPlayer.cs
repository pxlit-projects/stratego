using System;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.Domain.Contracts;

namespace Stratego.Domain
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a human player.
    /// </summary>
    public class HumanPlayer : IPlayer
    {
        /// <summary>
        /// Id of the player. This should be the identifier if the user for which the player is constructed.
        /// </summary>
        public Guid Id { get; }

        public string NickName { get; }
        public IArmy Army { get; }
        public bool IsRed { get; }
        public bool IsReady { get; set; }

        public HumanPlayer(Guid id, string nickName, bool isRed, IArmy army)
        {
            
        }
    }
}