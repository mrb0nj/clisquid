namespace CliSquid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using CliSquid.Enums;
    using CliSquid.Interfaces;
    using CliSquid.Prompts;
    using Pastel;

    public static class Prompt
    {
        internal const string GUTTER_PROMPT = "\u25C6"; // ◆
        internal const string GUTTER_PROMPT_COMPLETE = "\u25C7"; // ◇
        internal const string GUTTER_PROMPT_WARNING = "\u25B2"; // ▲
        internal const string GUTTER_PROMPT_ERROR = "\u25A0"; // ■
        internal const string GUTTER_BAR_START = "\u250C"; // ┌
        internal const string GUTTER_BAR = "\u2502"; // │
        internal const string GUTTER_BAR_END = "\u2514"; // └
        internal const int GUTTER_PAD_RIGHT = 3;
        internal const string RADIO_ACTIVE = "\u25CF"; // ●
        internal const string RADIO_INACTIVE = "\u25CB"; // ○
        internal const string CHECK_ACTIVE = "\u25FB"; // ◻
        internal const string CHECK_INACTIVE = "\u25FB"; // ◻
        internal const string CHECK_SELECTED = "\u25FC"; // ◼

        internal static CancellationTokenSource TokenSource = new CancellationTokenSource();
        internal static CancellationToken Token;
        private static TextWriter output = Console.Out;
        private static Configuration configuration = new Configuration();

        public static void Configure()
        {
            Token = TokenSource.Token;

            Console.Clear();
            output = Console.Out;
            Console.SetOut(new PrefixWriter());
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs args)
            {
                args.Cancel = true;
                TokenSource.Cancel();
            };
        }

        public static void Configure(Action<Configuration> configure)
        {
            Prompt.Configure();
            configure(configuration);
        }

        public static IUserInputPrompt<T> FromUserInput<T>() =>
            new UserInputPrompt<T>(configuration);

        public static ISelectPrompt<T> FromList<T>() => new SelectPrompt<T>(configuration);

        public static ISpinner<T> Spinner<T>() => new SpinnerPrompt<T>(configuration);

        public static void Intro(string text)
        {
            WriteGutter(GUTTER_BAR_START.Pastel(configuration.Theme.PrefixMuted));
            WriteText(
                string.Format(
                    " {0} "
                        .Pastel(configuration.Theme.IntroForeground)
                        .PastelBg(configuration.Theme.IntroBackground),
                    text
                )
            );
            WriteGutter(GUTTER_BAR.Pastel(configuration.Theme.PrefixMuted), newLine: true);
        }

        public static void Complete(string text)
        {
            WriteGutter(GUTTER_BAR_END.Pastel(configuration.Theme.PrefixMuted));
            WriteText(string.Format("{0}".Pastel(configuration.Theme.CompleteForeground), text));
            WriteText(string.Empty);
        }

        public static bool Confirm(string text)
        {
            var options = new List<PromptOption<bool>>
            {
                new PromptOption<bool>(true) { Display = "Yes" },
                new PromptOption<bool>(false) { Display = "No" }
            };
            return FromList<bool>().Title(text).Inline().Options(options).SelectOne();
        }

        internal static void CancelToken() => TokenSource.Cancel();

        internal static void ExitGracefully(string title)
        {
            var abortedText = "Aborted...";
            var interruptedText = "Program execution interrupted";
            WriteGutter(Prompt.GetGutterPrompt(PromptStatus.Error));
            WriteText(title.PadRight(Console.WindowWidth - title.Length - Prompt.GUTTER_PAD_RIGHT));
            WriteGutter(Prompt.GetGutterBar(PromptStatus.Error));
            WriteText(
                abortedText.PadRight(Console.WindowWidth - abortedText.Length - GUTTER_PAD_RIGHT)
            );
            WriteGutter(
                Prompt
                    .GetGutterBar(PromptStatus.Error)
                    .PadRight(Console.WindowWidth - GUTTER_PAD_RIGHT),
                newLine: true
            );
            WriteGutter(Prompt.GetGutterEnd(PromptStatus.Error));
            WriteText(
                interruptedText
                    .PadRight(Console.WindowWidth - interruptedText.Length - GUTTER_PAD_RIGHT)
                    .Pastel(configuration.Theme.ErrorForeground)
            );

            var pos = CursorPosition.GetCursorPosition();
            var lines = Console.BufferHeight - pos.Top - 1;
            for (var i = 0; i < lines; i++)
                output.WriteLine("".PadRight(Console.WindowWidth));
            Console.SetCursorPosition(pos.Left, pos.Top);
            Environment.Exit(-1);
        }

        internal static void WriteGutter(string gutter, bool newLine = false)
        {
            var suffix = newLine ? "\n" : "  ";
            output.Write("{0}{1}", gutter, suffix);
        }

        internal static void WriteText(string text, bool newLine = true)
        {
            if (newLine)
                output.WriteLine(text);
            else
                output.Write(text);
        }

        internal static string GetGutterPrompt(PromptStatus status)
        {
            string r = string.Empty;
            switch (status)
            {
                case PromptStatus.Active:
                    r = Prompt.GUTTER_PROMPT.Pastel(configuration.Theme.PrefixActiveIcon);
                    break;
                case PromptStatus.Complete:
                    r = Prompt
                        .GUTTER_PROMPT_COMPLETE
                        .Pastel(configuration.Theme.PrefixCompleteIcon);
                    break;
                case PromptStatus.Warning:
                    r = Prompt.GUTTER_PROMPT_WARNING.Pastel(configuration.Theme.PrefixWarningIcon);
                    break;
                case PromptStatus.Error:
                    r = Prompt.GUTTER_PROMPT_ERROR.Pastel(configuration.Theme.PrefixErrorIcon);
                    break;
            }

            return r;
        }

        internal static string GetGutterBar(PromptStatus status)
        {
            string r = string.Empty;
            switch (status)
            {
                case PromptStatus.Active:
                    r = Prompt.GUTTER_BAR.Pastel(configuration.Theme.PrefixActive);
                    break;
                case PromptStatus.Complete:
                    r = Prompt.GUTTER_BAR.Pastel(configuration.Theme.PrefixComplete);
                    break;
                case PromptStatus.Warning:
                    r = Prompt.GUTTER_BAR.Pastel(configuration.Theme.PrefixWarning);
                    break;
                case PromptStatus.Error:
                    r = Prompt.GUTTER_BAR.Pastel(configuration.Theme.PrefixError);
                    break;
            }

            return r;
        }

        internal static string GetGutterEndNote(PromptStatus status, string note)
        {
            if (string.IsNullOrWhiteSpace(note))
                return note;
            string r = string.Empty;
            switch (status)
            {
                case PromptStatus.Active:
                    r = note.Pastel(configuration.Theme.PrefixActive);
                    break;
                case PromptStatus.Complete:
                    r = note.Pastel(configuration.Theme.PrefixComplete);
                    break;
                case PromptStatus.Warning:
                    r = note.Pastel(configuration.Theme.PrefixWarning);
                    break;
                case PromptStatus.Error:
                    r = note.Pastel(configuration.Theme.PrefixError);
                    break;
            }

            return r;
        }

        internal static string GetGutterEnd(PromptStatus status)
        {
            string r = string.Empty;
            switch (status)
            {
                case PromptStatus.Active:
                    r = Prompt.GUTTER_BAR_END.Pastel(configuration.Theme.PrefixActive);
                    break;
                case PromptStatus.Complete:
                    r = Prompt.GUTTER_BAR.Pastel(configuration.Theme.PrefixMuted);
                    break;
                case PromptStatus.Warning:
                    r = Prompt.GUTTER_BAR_END.Pastel(configuration.Theme.PrefixWarning);
                    break;
                case PromptStatus.Error:
                    r = Prompt.GUTTER_BAR_END.Pastel(configuration.Theme.PrefixError);
                    break;
            }

            return r;
        }
    }
}
