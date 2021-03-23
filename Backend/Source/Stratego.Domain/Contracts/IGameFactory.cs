namespace Stratego.Domain.Contracts
{
    /// <summary>
    /// Factory that creates game instances.
    /// </summary>
    public interface IGameFactory
    {
        /// <summary>
        /// Creates a game for 2 human players.
        /// </summary>
        IGame CreateNewForUsers(User user1, User user2, GameSettings settings);
    }
}