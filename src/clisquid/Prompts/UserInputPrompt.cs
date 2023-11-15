namespace CliSquid.Prompts
{
    using System;
    using System.Text;
    using CliSquid.Enums;
    using CliSquid.Interfaces;
    using Pastel;

    public class UserInputPrompt<T> : IUserInputPrompt<T>
    {
        private string _title = "";
        private string _placeholder = "";
        private string _initialValue = "";
        private Func<string, Tuple<bool, string>>? _validator;
        private Configuration _promptOptions;

        public UserInputPrompt()
        {
            _promptOptions = new Configuration();
        }

        public UserInputPrompt(Configuration promptOptions)
        {
            _promptOptions = promptOptions;
        }

        public IUserInputPrompt<T> InitialValue(string initialValue)
        {
            _initialValue = initialValue;
            return this;
        }

        public IUserInputPrompt<T> Placeholder(string placeholder)
        {
            _placeholder = placeholder;
            return this;
        }

        public T Read(Func<string, T> converter)
        {
            Action<string, string, PromptStatus, string> render = (
                string prompt,
                string response,
                PromptStatus status,
                string note
            ) =>
            {
                Prompt.WriteGutter(Prompt.GetGutterPrompt(status));
                Prompt.WriteText(prompt.Pastel(_promptOptions.Theme.UserInputForeground));
                Prompt.WriteGutter(Prompt.GetGutterBar(status));

                Prompt.WriteText(
                    response.Pastel(
                        status == PromptStatus.Complete
                            ? _promptOptions.Theme.UserInputResultForeground
                            : _promptOptions.Theme.UserInputActiveForeground
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

            var sb = new StringBuilder(_initialValue);

            render(
                _title,
                sb.Length > 0
                    ? sb.ToString().Pastel(_promptOptions.Theme.UserInputActiveForeground)
                    : _placeholder.Pastel(_promptOptions.Theme.UserInputPlaceholderForeground),
                PromptStatus.Active,
                string.Empty
            );

            Console.Out.Flush();
            var pos = CursorPosition.GetCursorPosition();
            Console.SetCursorPosition(Prompt.GUTTER_PAD_RIGHT + sb.Length, pos.Top - 2);

            ConsoleKeyInfo cki = default;
            var valid = Tuple.Create(true, string.Empty);

            do
            {
                var currentInput = sb.ToString();
                Console.TreatControlCAsInput = true;
                cki = Console.ReadKey(true);
                Console.TreatControlCAsInput = false;
                pos = CursorPosition.GetCursorPosition();

                var validCharacter = false;
                var forceValidation = false;
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
                    case ConsoleKey.Escape:
                        Prompt.CancelToken();
                        break;
                    case ConsoleKey.Enter:
                        forceValidation = true;
                        break; // ignore this as an input character
                    case ConsoleKey.C:
                        if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
                            Prompt.CancelToken();
                        else
                            validCharacter = true;
                        break;
                    default:
                        validCharacter = true;
                        break;
                }

                if (Prompt.Token.IsCancellationRequested)
                {
                    Console.SetCursorPosition(0, pos.Top - 1);
                    Prompt.ExitGracefully(_title);
                }

                if (validCharacter)
                {
                    var ins = pos.Left - Prompt.GUTTER_PAD_RIGHT;
                    if (ins >= 0 && sb.Length > ins)
                        sb.Insert(ins, cki.KeyChar);
                    else
                        sb.Append(new char[] { cki.KeyChar });

                    pos.Left++;
                }

                var newInput = sb.ToString();
                if (_validator != null)
                    valid = _validator(newInput);
                else
                    valid = Tuple.Create(_validator == null, string.Empty);

                var r = string.IsNullOrWhiteSpace(newInput)
                    ? _placeholder.Pastel(_promptOptions.Theme.UserInputPlaceholderForeground)
                    : newInput;

                var isActive =
                    valid.Item1 || (string.IsNullOrWhiteSpace(newInput) && !forceValidation);
                reRender(
                    _title,
                    r.PadRight(Console.WindowWidth - r.Length - Prompt.GUTTER_PAD_RIGHT),
                    isActive ? PromptStatus.Active : PromptStatus.Warning,
                    !isActive ? valid.Item2 : string.Empty
                );

                Console.SetCursorPosition(pos.Left, pos.Top);
            } while (cki.Key != ConsoleKey.Enter || !valid.Item1);

            var response = sb.ToString();
            reRender(_title, response, PromptStatus.Complete, string.Empty);

            Console.SetCursorPosition(0, pos.Top + 2);

            T output;
            try
            {
                if (converter != null)
                    output = converter(response);
                else
                    output = (T)Convert.ChangeType(response, typeof(T));
            }
            catch
            {
                output = default(T);
            } // eeeugh
            return output!;
        }

        IUserInputPrompt<T> IUserInputPrompt<T>.Title(string title)
        {
            _title = title;
            return this;
        }

        public IUserInputPrompt<T> Validate(Func<string, Tuple<bool, string>> validator)
        {
            _validator = validator;
            return this;
        }
    }
}
