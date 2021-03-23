namespace Stratego.Domain.Contracts
{
    /// <summary>
    /// Factory that creates game candidate instances.
    /// </summary>
    public interface IGameCandidateFactory
    {
        /// <summary>
        /// Creates a game candidate from a user and his / her desired game settings.
        /// </summary>
        IGameCandidate CreateNewForUser(User user, GameSettings settings);
    }
}