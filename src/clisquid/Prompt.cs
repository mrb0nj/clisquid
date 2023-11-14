namespace CliSquid
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
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

        private static TextWriter output = Console.Out;

        public static void Init()
        {
            Console.Clear();
            output = Console.Out;
            Console.SetOut(new PrefixWriter());
        }

        public static IUserInputPrompt<T> FromUserInput<T>() => new UserInputPrompt<T>();

        public static ISelectPrompt<T> FromList<T>() => new SelectPrompt<T>();

        public static ISpinner<T> Spinner<T>() => new SpinnerPrompt<T>();

        public static void Intro(string text)
        {
            WriteGutter(GUTTER_BAR_START.Pastel(Color.DimGray));
            WriteText(string.Format(" {0} ".Pastel(Color.Black).PastelBg(Color.DodgerBlue), text));
            WriteGutter(GUTTER_BAR.Pastel(Color.DimGray), newLine: true);
        }

        public static void Complete(string text)
        {
            WriteGutter(GUTTER_BAR_END.Pastel(Color.DimGray));
            WriteText(string.Format("{0}".Pastel(Color.GreenYellow), text));
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
                    r = Prompt.GUTTER_PROMPT.Pastel(Color.DodgerBlue);
                    break;
                case PromptStatus.Complete:
                    r = Prompt.GUTTER_PROMPT_COMPLETE.Pastel(Color.GreenYellow);
                    break;
                case PromptStatus.Warning:
                    r = Prompt.GUTTER_PROMPT_WARNING.Pastel(Color.Goldenrod);
                    break;
                case PromptStatus.Error:
                    r = Prompt.GUTTER_PROMPT_ERROR.Pastel(Color.Crimson);
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
                    r = Prompt.GUTTER_BAR.Pastel(Color.DodgerBlue);
                    break;
                case PromptStatus.Complete:
                    r = Prompt.GUTTER_BAR.Pastel(Color.DimGray);
                    break;
                case PromptStatus.Warning:
                    r = Prompt.GUTTER_BAR.Pastel(Color.Goldenrod);
                    break;
                case PromptStatus.Error:
                    r = Prompt.GUTTER_BAR.Pastel(Color.Crimson);
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
                    r = note.Pastel(Color.DodgerBlue);
                    break;
                case PromptStatus.Complete:
                    r = note.Pastel(Color.DimGray);
                    break;
                case PromptStatus.Warning:
                    r = note.Pastel(Color.Goldenrod);
                    break;
                case PromptStatus.Error:
                    r = note.Pastel(Color.Crimson);
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
                    r = Prompt.GUTTER_BAR_END.Pastel(Color.DodgerBlue);
                    break;
                case PromptStatus.Complete:
                    r = Prompt.GUTTER_BAR.Pastel(Color.DimGray);
                    break;
                case PromptStatus.Warning:
                    r = Prompt.GUTTER_BAR_END.Pastel(Color.Goldenrod);
                    break;
                case PromptStatus.Error:
                    r = Prompt.GUTTER_BAR_END.Pastel(Color.Crimson);
                    break;
            }

            return r;
        }
    }
}
