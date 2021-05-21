using Stratego.Domain;

namespace Stratego.TestTools.Builders
{
    public class GameSettingsBuilder
    {
        private readonly GameSettings _settings;

        public GameSettingsBuilder()
        {
            _settings = new GameSettings
            {
                AutoMatchCandidates = true,
                IsQuickGame = true
            };
        }

        public GameSettingsBuilder WithAutoMatching(bool value)
        {
            _settings.AutoMatchCandidates = value;
            return this;
        }

        public GameSettingsBuilder WithIsQuickGame(bool value)
        {
            _settings.IsQuickGame = value;
            return this;
        }

        public GameSettingsBuilder AsCopyOf(GameSettings otherSettings)
        {
            _settings.IsQuickGame = otherSettings.IsQuickGame;
            _settings.AutoMatchCandidates = otherSettings.AutoMatchCandidates;
            return this;
        }

        public GameSettings Build()
        {
            return _settings;
        }
    }
}