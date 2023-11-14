namespace CliSquid
{
    using System;
    using System.Drawing;
    using System.Text;
    using Pastel;

    public class UserInputPrompt<T> : IUserInputPrompt<T>
    {
        private string _title = "";
        private string _placeholder = "";
        private Func<string, Tuple<bool, string>>? _validator;

        public UserInputPrompt() { }

        public IUserInputPrompt<T> InitialValue(string initialValue)
        {
            throw new NotImplementedException();
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

            render(_title, _placeholder.Pastel(Color.DimGray), PromptStatus.Active, string.Empty);

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
                    _title,
                    r.PadRight(padLength),
                    valid.Item1 ? PromptStatus.Active : PromptStatus.Warning,
                    valid.Item2
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
