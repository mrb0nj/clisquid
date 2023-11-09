namespace CliSquid
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
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

        public static IUserInputPrompt<T> FromUserInput<T>() => new Prompt<T>();

        public static ISelectPrompt<T> FromList<T>() => new Prompt<T>();

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
            Console.Write("{0}{1}", gutter, suffix);
        }

        internal static void WriteText(string text, bool newLine = true)
        {
            if (newLine)
                Console.WriteLine(text);
            else
                Console.Write(text);
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

    public class Prompt<T> : IUserInputPrompt<T>, ISelectPrompt<T>
    {
        private string _text = "";
        private string _placeholder = "";
        private IList<PromptOption<T>>? _options;
        private bool _optionsInline;
        private int _limit;
        private Func<string, Tuple<bool, string>>? _validator;

        public Prompt() { }

        public IUserInputPrompt<T> InitialValue(string initialValue)
        {
            throw new NotImplementedException();
        }

        public ISelectPrompt<T> Inline()
        {
            _optionsInline = true;
            return this;
        }

        public ISelectPrompt<T> Limit(int limit)
        {
            _limit = limit;
            return this;
        }

        public ISelectPrompt<T> Options(IList<PromptOption<T>> options)
        {
            _options = options;
            return this;
        }

        public IUserInputPrompt<T> Placeholder(string placeholder)
        {
            _placeholder = placeholder;
            return this;
        }

        public T Read()
        {
            Action<string, string, PromptStatus, string> render = (
                string prompt,
                string response,
                PromptStatus status,
                string note
            ) =>
            {
                Prompt.WriteGutter(Prompt.GetGutterPrompt(status));
                Prompt.WriteText(prompt.Pastel(Color.WhiteSmoke));
                Prompt.WriteGutter(Prompt.GetGutterBar(status));

                Prompt.WriteText(
                    response.Pastel(
                        status == PromptStatus.Complete ? Color.DimGray : Color.WhiteSmoke
                    )
                );

                var gutterEnd = Prompt.GetGutterEnd(status);
                var gutterEndNote = Prompt.GetGutterEndNote(status, note);
                gutterEndNote = gutterEndNote.PadRight(
                    Console.WindowWidth - gutterEndNote.Length - Prompt.GUTTER_PAD_RIGHT
                );
                gutterEnd = string.Format("{0}  {1}", gutterEnd, gutterEndNote);
                Prompt.WriteGutter(gutterEnd, newLine: true);
            };

            Action<string, string, PromptStatus, string> reRender = (
                string prompt,
                string response,
                PromptStatus status,
                string warning
            ) =>
            {
                var pos = CursorPosition.GetCursorPosition();
                Console.SetCursorPosition(0, pos.Top - 1);
                render(prompt, response, status, warning);
            };

            render(_text, _placeholder.Pastel(Color.DimGray), PromptStatus.Active, string.Empty);

            Console.Out.Flush();
            var pos = CursorPosition.GetCursorPosition();
            Console.SetCursorPosition(Prompt.GUTTER_PAD_RIGHT, pos.Top - 2);

            var sb = new StringBuilder();
            ConsoleKeyInfo cki;
            var valid = Tuple.Create(true, string.Empty);

            do
            {
                var currentInput = sb.ToString();
                cki = Console.ReadKey(true);
                pos = CursorPosition.GetCursorPosition();

                switch (cki.Key)
                {
                    case ConsoleKey.Delete:
                        var del = pos.Left - Prompt.GUTTER_PAD_RIGHT;
                        if (del >= 0 && sb.Length > del)
                            sb.Remove(del, 1);
                        break;
                    case ConsoleKey.Backspace:
                        var back = pos.Left - Prompt.GUTTER_PAD_RIGHT - 1;
                        if (back >= 0 && sb.Length > back)
                            sb.Remove(back, 1);

                        if (pos.Left > Prompt.GUTTER_PAD_RIGHT)
                            pos.Left--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (pos.Left < sb.Length + Prompt.GUTTER_PAD_RIGHT)
                            pos.Left++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (pos.Left > Prompt.GUTTER_PAD_RIGHT)
                            pos.Left--;
                        break;
                    case ConsoleKey.Enter:
                        // ignore this key
                        break;
                    default:
                        var ins = pos.Left - Prompt.GUTTER_PAD_RIGHT;
                        if (ins >= 0 && sb.Length > ins)
                            sb.Insert(ins, cki.KeyChar);
                        else
                            sb.Append(new char[] { cki.KeyChar });

                        pos.Left++;
                        break;
                }

                var newInput = sb.ToString();
                var padLength =
                    currentInput.Length > _placeholder.Length
                        ? currentInput.Length
                        : _placeholder.Length;

                if (_validator != null && !string.IsNullOrWhiteSpace(newInput))
                    valid = _validator(newInput);
                else
                    valid = Tuple.Create(_validator == null, string.Empty);

                var r = string.IsNullOrWhiteSpace(newInput)
                    ? _placeholder.Pastel(Color.DimGray)
                    : newInput;
                reRender(
                    _text,
                    r.PadRight(padLength),
                    valid.Item1 ? PromptStatus.Active : PromptStatus.Warning,
                    valid.Item2
                );

                Console.SetCursorPosition(pos.Left, pos.Top);
            } while (cki.Key != ConsoleKey.Enter || !valid.Item1);

            var response = sb.ToString();
            reRender(_text, response, PromptStatus.Complete, string.Empty);

            Console.SetCursorPosition(0, pos.Top + 2);
            T output;
            try
            {
                output = (T)Convert.ChangeType(response, typeof(T));
            }
            catch
            {
                output = default(T);
            } // eeeugh
            return output;
        }

        public IList<T> SelectMany()
        {
            if (_options == null || !_options.Any())
                throw new Exception("No options provided");

            Action<
                string,
                IList<PromptOption<T>>,
                IList<PromptOption<T>>,
                PromptOption<T>,
                PromptStatus
            > render = (
                string prompt,
                IList<PromptOption<T>> options,
                IList<PromptOption<T>> selected,
                PromptOption<T>? active,
                PromptStatus status
            ) =>
            {
                Console.CursorVisible = false;
                Prompt.WriteGutter(Prompt.GetGutterPrompt(status));
                Prompt.WriteText(prompt.Pastel(Color.WhiteSmoke));

                if (status == PromptStatus.Complete)
                {
                    Prompt.WriteGutter(Prompt.GetGutterBar(status));
                    var selectedOptions = String.Join(
                        ", ",
                        selected.Select(s => s.Display).ToArray<string>()
                    );
                    var display = selectedOptions.PadRight(
                        Console.WindowWidth - selectedOptions.Length - Prompt.GUTTER_PAD_RIGHT
                    );
                    Prompt.WriteText(display.Pastel(Color.DimGray));
                    for (var i = 0; i < options.Count; i++)
                        Prompt.WriteText("".PadRight(Console.WindowWidth));
                    var pos = CursorPosition.GetCursorPosition();
                    Console.SetCursorPosition(0, pos.Top - options.Count);
                }
                else
                {
                    if (_optionsInline)
                        Prompt.WriteGutter(Prompt.GetGutterBar(status));
                    foreach (var option in options)
                    {
                        if (!_optionsInline)
                            Prompt.WriteGutter(Prompt.GetGutterBar(status));

                        var isActive = option == active;
                        var isSelected = selected.Contains(option);
                        var marker = isSelected
                            ? Prompt.CHECK_SELECTED.Pastel(Color.DodgerBlue)
                            : Prompt
                                .CHECK_INACTIVE
                                .Pastel(isActive ? Color.DodgerBlue : Color.DimGray);

                        var display = option
                            .Display
                            .Pastel(isActive ? Color.WhiteSmoke : Color.DimGray);
                        if (!string.IsNullOrWhiteSpace(option.Hint) && isActive && !_optionsInline)
                            display = string.Format(
                                "{0} {1}",
                                display,
                                option.Hint.Pastel(Color.DimGray)
                            );

                        if (!_optionsInline)
                            display = display.PadRight(
                                Console.WindowWidth - display.Length - Prompt.GUTTER_PAD_RIGHT - 3
                            );
                        Prompt.WriteText(
                            string.Format(" {0} {1}", marker, display),
                            !_optionsInline
                        );
                    }

                    if (_optionsInline)
                    {
                        var pos = CursorPosition.GetCursorPosition();
                        Prompt.WriteText(string.Empty.PadRight(Console.WindowWidth - pos.Left));
                    }
                }

                var gutterEnd = Prompt.GetGutterEnd(status);
                var gutterEndNote = Prompt.GetGutterEndNote(status, string.Empty);
                gutterEndNote = gutterEndNote.PadRight(
                    Console.WindowWidth - gutterEndNote.Length - Prompt.GUTTER_PAD_RIGHT
                );
                gutterEnd = string.Format("{0}  {1}", gutterEnd, gutterEndNote);
                Prompt.WriteGutter(gutterEnd, newLine: true);
            };

            Action<
                string,
                IList<PromptOption<T>>,
                IList<PromptOption<T>>,
                PromptOption<T>,
                PromptStatus
            > reRender = (
                string prompt,
                IList<PromptOption<T>> options,
                IList<PromptOption<T>> selected,
                PromptOption<T>? active,
                PromptStatus status
            ) =>
            {
                var pos = CursorPosition.GetCursorPosition();
                var topOffset = _optionsInline ? 3 : _options.Count + 2;
                Console.SetCursorPosition(0, pos.Top - topOffset);
                render(prompt, options, selected, active, status);
            };

            var activeItem = _options.First();
            IList<PromptOption<T>> selectedItems = new List<PromptOption<T>>(
                _options.Where(o => o.IsDefault)
            );
            render(_text, _options, selectedItems, activeItem, PromptStatus.Active);

            Console.Out.Flush();
            ConsoleKeyInfo cki;

            do
            {
                cki = Console.ReadKey(true);
                var idx = _options.IndexOf(activeItem);
                switch (cki.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.H:
                        if (!_optionsInline)
                            break;
                        if (idx == 0)
                            continue;
                        activeItem = _options[idx - 1];
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.L:
                        if (!_optionsInline)
                            break;
                        if (idx + 1 > _options.Count() - 1)
                            continue;
                        activeItem = _options[idx + 1];
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.K:
                        if (_optionsInline)
                            break;
                        if (idx == 0)
                            continue;
                        activeItem = _options[idx - 1];
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.J:
                        if (_optionsInline)
                            break;
                        if (idx + 1 > _options.Count() - 1)
                            continue;
                        activeItem = _options[idx + 1];
                        break;
                    case ConsoleKey.Spacebar:
                        if (selectedItems.Contains(activeItem))
                            selectedItems.Remove(activeItem);
                        else
                            selectedItems.Add(activeItem);
                        break;
                }

                reRender(_text, _options, selectedItems, activeItem, PromptStatus.Active);
            } while (cki.Key != ConsoleKey.Enter || !selectedItems.Any());

            reRender(_text, _options, selectedItems, null, PromptStatus.Complete);
            Console.CursorVisible = true;

            return selectedItems.Select(p => p.Value).ToList();
        }

        public T SelectOne()
        {
            if (_options == null || !_options.Any())
                throw new Exception("No options provided");

            Action<
                string,
                IList<PromptOption<T>>,
                PromptOption<T>?,
                PromptOption<T>?,
                PromptStatus
            > render = (
                string prompt,
                IList<PromptOption<T>> options,
                PromptOption<T>? selected,
                PromptOption<T>? active,
                PromptStatus status
            ) =>
            {
                Console.CursorVisible = false;
                Prompt.WriteGutter(Prompt.GetGutterPrompt(status));
                Prompt.WriteText(prompt.Pastel(Color.WhiteSmoke));

                if (status == PromptStatus.Complete)
                {
                    Prompt.WriteGutter(Prompt.GetGutterBar(status));
                    if (selected != null)
                    {
                        var display = selected
                            .Display
                            .PadRight(
                                Console.WindowWidth
                                    - selected.Display.Length
                                    - Prompt.GUTTER_PAD_RIGHT
                            );
                        Prompt.WriteText(display.Pastel(Color.DimGray));
                    }
                }
                else
                {
                    if (_optionsInline)
                        Prompt.WriteGutter(Prompt.GetGutterBar(status));

                    foreach (var option in options)
                    {
                        if (!_optionsInline)
                            Prompt.WriteGutter(Prompt.GetGutterBar(status));

                        var isActive = option == active;
                        var marker = (
                            isActive ? Prompt.RADIO_ACTIVE : Prompt.RADIO_INACTIVE
                        ).Pastel(isActive ? Color.DodgerBlue : Color.DimGray);
                        var display = option
                            .Display
                            .Pastel(isActive ? Color.WhiteSmoke : Color.DimGray);
                        if (!string.IsNullOrWhiteSpace(option.Hint) && isActive && !_optionsInline)
                            display = string.Format(
                                "{0} {1}",
                                display,
                                option.Hint.Pastel(Color.DimGray)
                            );

                        if (!_optionsInline)
                            display = display.PadRight(
                                Console.WindowWidth - display.Length - Prompt.GUTTER_PAD_RIGHT - 3
                            );
                        Prompt.WriteText(
                            string.Format(" {0} {1}", marker, display),
                            !_optionsInline
                        );
                    }

                    if (_optionsInline)
                        Prompt.WriteText(string.Empty);
                }

                var gutterEnd = Prompt.GetGutterEnd(status);
                var gutterEndNote = Prompt.GetGutterEndNote(status, string.Empty);
                gutterEndNote = gutterEndNote.PadRight(
                    Console.WindowWidth - gutterEndNote.Length - Prompt.GUTTER_PAD_RIGHT
                );
                gutterEnd = string.Format("{0}  {1}", gutterEnd, gutterEndNote);
                Prompt.WriteGutter(gutterEnd, newLine: true);
            };

            Action<
                string,
                IList<PromptOption<T>>,
                PromptOption<T>?,
                PromptOption<T>?,
                PromptStatus
            > reRender = (
                string prompt,
                IList<PromptOption<T>> options,
                PromptOption<T>? selected,
                PromptOption<T>? active,
                PromptStatus status
            ) =>
            {
                var pos = CursorPosition.GetCursorPosition();

                Console.SetCursorPosition(0, pos.Top - (_optionsInline ? 1 : 2) - _options.Count);

                render(prompt, options, selected, active, status);
            };

            var activeItem = _options.First();
            PromptOption<T>? selectedItem = null;
            render(_text, _options, selectedItem, activeItem, PromptStatus.Active);

            Console.Out.Flush();

            ConsoleKeyInfo cki;

            do
            {
                cki = Console.ReadKey(true);
                var idx = _options.IndexOf(activeItem);
                switch (cki.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.H:
                        if (!_optionsInline)
                            break;
                        if (idx == 0)
                            continue;
                        activeItem = _options[idx - 1];
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.L:
                        if (!_optionsInline)
                            break;
                        if (idx + 1 > _options.Count() - 1)
                            continue;
                        activeItem = _options[idx + 1];
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.K:
                        if (_optionsInline)
                            break;
                        if (idx == 0)
                            continue;
                        activeItem = _options[idx - 1];
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.J:
                        if (_optionsInline)
                            break;
                        if (idx + 1 > _options.Count() - 1)
                            continue;
                        activeItem = _options[idx + 1];
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        selectedItem = activeItem;
                        break;
                }

                reRender(_text, _options, selectedItem, activeItem, PromptStatus.Active);
            } while (cki.Key != ConsoleKey.Enter || selectedItem == null);

            reRender(_text, _options, selectedItem, null, PromptStatus.Complete);
            Console.CursorVisible = true;

            return selectedItem.Value;
        }

        IUserInputPrompt<T> IUserInputPrompt<T>.Title(string title)
        {
            _text = title;
            return this;
        }

        ISelectPrompt<T> ISelectPrompt<T>.Title(string title)
        {
            _text = title;
            return this;
        }

        public IUserInputPrompt<T> Validate(Func<string, Tuple<bool, string>> validator)
        {
            _validator = validator;
            return this;
        }
    }

    internal class CursorPosition
    {
        public int Top { get; set; }
        public int Left { get; set; }

        public static CursorPosition GetCursorPosition()
        {
            return new CursorPosition { Top = Console.CursorTop, Left = Console.CursorLeft };
        }
    }
}
