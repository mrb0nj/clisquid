namespace CliSquid.Themes
{
    using CliSquid.Enums;
    using CliSquid.Interfaces;

    public static class Theme
    {
        public static IPromptTheme GetTheme(PromptTheme theme)
        {
            IPromptTheme activeTheme = new DefaultTheme();
            switch (theme)
            {
                case PromptTheme.Dracula:
                    activeTheme = new DraculaTheme();
                    break;
            }

            return activeTheme;
        }
    }
}
