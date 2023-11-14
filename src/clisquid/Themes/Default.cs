namespace CliSquid.Themes
{
    using System.Drawing;
    using CliSquid.Interfaces;

    public class DefaultTheme : IPromptTheme
    {
        public Color Background => throw new System.NotImplementedException();
        public Color ErrorForeground => Color.Crimson;
        public Color IntroForeground => Color.Black;
        public Color IntroBackground => Color.DodgerBlue;
        public Color CompleteForeground => Color.GreenYellow;
        public Color CompleteBackground => throw new System.NotImplementedException();
        public Color PrefixActive => Color.DodgerBlue;
        public Color PrefixActiveIcon => Color.DodgerBlue;
        public Color PrefixComplete => Color.DimGray;
        public Color PrefixCompleteIcon => Color.GreenYellow;
        public Color PrefixWarning => Color.Goldenrod;
        public Color PrefixWarningIcon => Color.Goldenrod;
        public Color PrefixError => Color.Crimson;
        public Color PrefixErrorIcon => Color.Crimson;
        public Color PrefixMuted => Color.DimGray;
        public Color Spinner => Color.DodgerBlue;
        public Color SpinnerForeground => Color.WhiteSmoke;
        public Color SpinnerBackground => throw new System.NotImplementedException();
        public Color SpinnerStatusForeground => Color.DimGray;
        public Color SpinnerStatusBackground => throw new System.NotImplementedException();
        public Color SpinnerResultForeground => Color.DimGray;
        public Color SpinnerResultBackground => throw new System.NotImplementedException();
        public Color SelectForeground => Color.WhiteSmoke;
        public Color SelectBackground => throw new System.NotImplementedException();
        public Color SelectResultForeground => Color.DimGray;
        public Color SelectResultBackground => throw new System.NotImplementedException();
        public Color SelectOptionActive => Color.DodgerBlue;
        public Color SelectOptionInactive => Color.DimGray;
        public Color SelectOptionSelected => Color.DodgerBlue;
        public Color SelectOptionTextActive => Color.DodgerBlue;
        public Color SelectOptionTextInactive => Color.DimGray;
        public Color SelectOptionHint => Color.DimGray;
        public Color UserInputForeground => Color.WhiteSmoke;
        public Color UserInputBackground => throw new System.NotImplementedException();
        public Color UserInputActiveForeground => Color.WhiteSmoke;
        public Color UserInputActiveBackground => throw new System.NotImplementedException();
        public Color UserInputResultForeground => Color.DimGray;
        public Color UserInputResultBackground => throw new System.NotImplementedException();
        public Color UserInputPlaceholderForeground => Color.DimGray;
        public Color UserInputPlaceholderBackground => throw new System.NotImplementedException();
    }
}
