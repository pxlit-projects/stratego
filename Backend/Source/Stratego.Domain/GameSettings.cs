namespace Stratego.Domain
{
    public class GameSettings
    {
        /// <summary>
        /// If true, you will be automatically matched with another candidate in the waiting pool.
        /// </summary>
        public bool AutoMatchCandidates { get; set; }

        /// <summary>
        /// If true, a game with a 10x10 board and 40 pieces per army will be created.
        /// If false, a game with a 8x8 board and 10 pieces per army will be created.
        /// </summary>
        public bool IsQuickGame { get; set; }

        public GameSettings()
        {
            AutoMatchCandidates = true;
            IsQuickGame = false;
        }
    }
}