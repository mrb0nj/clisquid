namespace CliSquid.Interfaces
{
    using System.Drawing;

    public interface IPromptTheme
    {
        // Intro Prompt
        public Color IntroForeground { get; }
        public Color IntroBackground { get; }

        // Complete Prompt
        public Color CompleteForeground { get; }
        public Color CompleteBackground { get; }

        // Prefix Generic
        public Color PrefixActive { get; }
        public Color PrefixActiveIcon { get; }
        public Color PrefixComplete { get; }
        public Color PrefixCompleteIcon { get; }
        public Color PrefixWarning { get; }
        public Color PrefixWarningIcon { get; }
        public Color PrefixError { get; }
        public Color PrefixErrorIcon { get; }
        public Color PrefixMuted { get; }

        // Spinner Prompt
        public Color Spinner { get; }
        public Color SpinnerForeground { get; }
        public Color SpinnerBackground { get; }
        public Color SpinnerStatusForeground { get; }
        public Color SpinnerStatusBackground { get; }
        public Color SpinnerResultForeground { get; }
        public Color SpinnerResultBackground { get; }

        // Select Prompt
        public Color SelectForeground { get; }
        public Color SelectBackground { get; }
        public Color SelectResultForeground { get; }
        public Color SelectResultBackground { get; }
        public Color SelectOptionActive { get; }
        public Color SelectOptionInactive { get; }
        public Color SelectOptionSelected { get; }
        public Color SelectOptionTextActive { get; }
        public Color SelectOptionTextInactive { get; }
        public Color SelectOptionHint { get; }

        // User input prompt
        public Color UserInputForeground { get; }
        public Color UserInputBackground { get; }
        public Color UserInputActiveForeground { get; }
        public Color UserInputActiveBackground { get; }
        public Color UserInputResultForeground { get; }
        public Color UserInputResultBackground { get; }
        public Color UserInputPlaceholderForeground { get; }
        public Color UserInputPlaceholderBackground { get; }

        // Generic
        public Color Background { get; }
        public Color ErrorForeground { get; }
    }
}
