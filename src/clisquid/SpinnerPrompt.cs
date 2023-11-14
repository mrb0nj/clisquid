namespace CliSquid
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Pastel;

    public class SpinnerPrompt<T> : ISpinner<T>
    {
        private string _title = "";
        private string _status = "";
        private SpinnerType _spinnerType = SpinnerType.Dots;

        public ISpinner<T> SetSpinnerType(SpinnerType spinnerType)
        {
            _spinnerType = spinnerType;
            return this;
        }

        public void SetStatus(string status)
        {
            _status = status;
        }

        public T Spin(Func<ISpinner<T>, T> func)
        {
            Console.CursorVisible = false;
            var t = Task.Run(() => func(this));
            var sp = GetSpinner(_spinnerType);
            var st = sp.Length == 0 ? 1000 : 1000 / sp.Length;

            var c = 0;
            var pos = CursorPosition.GetCursorPosition();
            while (!t.IsCompleted)
            {
                Console.SetCursorPosition(0, pos.Top);
                Prompt.WriteGutter(sp[c].Pastel(Color.DodgerBlue));
                Prompt.WriteText(_title);
                Prompt.WriteGutter(Prompt.GetGutterBar(PromptStatus.Active));
                Prompt.WriteText(
                    _status
                        .PadRight(Console.WindowWidth - _status.Length - Prompt.GUTTER_PAD_RIGHT)
                        .Pastel(Color.DimGray)
                );
                Prompt.WriteGutter(Prompt.GetGutterEnd(PromptStatus.Active));
                Thread.Sleep(st);
                c = c >= sp.Length - 1 ? 0 : c + 1;
            }

            var result = t.Result;
            var resultText = result != null ? result.ToString() : "Complete";

            Console.SetCursorPosition(0, pos.Top);
            Prompt.WriteGutter(Prompt.GetGutterPrompt(PromptStatus.Complete));
            Prompt.WriteText(
                _title.PadRight(Console.WindowWidth - _title.Length - Prompt.GUTTER_PAD_RIGHT)
            );
            Prompt.WriteGutter(Prompt.GetGutterBar(PromptStatus.Complete));
            Prompt.WriteText(resultText.Pastel(Color.DimGray));
            Prompt.WriteGutter(Prompt.GetGutterBar(PromptStatus.Complete), newLine: true);

            Console.CursorVisible = true;
            return result;
        }

        public ISpinner<T> Title(string title)
        {
            _title = title;
            return this;
        }

        private string[] GetSpinner(SpinnerType spinnerType)
        {
            switch (spinnerType)
            {
                case SpinnerType.Dots:
                    return new string[] { "⣾", "⣽", "⣻", "⢿", "⡿", "⣟", "⣯", "⣷" };
                case SpinnerType.Line:
                    return new string[] { "|", "/", "-", "\\" };
                case SpinnerType.Jump:
                    return new string[] { "⢄", "⢂", "⢁", "⡁", "⡈", "⡐", "⡠" };
                case SpinnerType.Pulse:
                    return new string[] { "█", "▓", "▒", "░" };
                case SpinnerType.MiniDots:
                    return new string[] { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
                case SpinnerType.Hamburger:
                    return new string[] { "☱", "☲", "☴", "☲" };
            }

            return new string[0];
        }
    }
}
