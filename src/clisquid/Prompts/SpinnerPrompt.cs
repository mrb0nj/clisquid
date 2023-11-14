namespace CliSquid.Prompts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CliSquid.Enums;
    using CliSquid.Interfaces;
    using Pastel;

    public class SpinnerPrompt<T> : ISpinner<T>
    {
        private string _title = "";
        private string _status = "";
        private SpinnerType _spinnerType = SpinnerType.Dots;
        private Configuration _promptOptions;

        public SpinnerPrompt()
        {
            _promptOptions = new Configuration();
        }

        public SpinnerPrompt(Configuration promptOptions)
        {
            _promptOptions = promptOptions;
        }

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
            while (!t.IsCompleted && !Prompt.Token.IsCancellationRequested)
            {
                Console.SetCursorPosition(0, pos.Top);
                Prompt.WriteGutter(sp[c].Pastel(_promptOptions.Theme.Spinner));
                Prompt.WriteText(_title.Pastel(_promptOptions.Theme.SpinnerForeground));
                Prompt.WriteGutter(Prompt.GetGutterBar(PromptStatus.Active));
                Prompt.WriteText(
                    _status
                        .PadRight(Console.WindowWidth - _status.Length - Prompt.GUTTER_PAD_RIGHT)
                        .Pastel(_promptOptions.Theme.SpinnerStatusForeground)
                );
                Prompt.WriteGutter(Prompt.GetGutterEnd(PromptStatus.Active));
                Thread.Sleep(st);
                c = c >= sp.Length - 1 ? 0 : c + 1;
            }

            Console.CursorVisible = true;
            if (Prompt.Token.IsCancellationRequested)
            {
                Console.SetCursorPosition(0, pos.Top);
                Prompt.ExitGracefully(_title);
            }

            var result = t.Result;
            var resultText = result != null ? result.ToString() : "Complete";

            Console.SetCursorPosition(0, pos.Top);
            Prompt.WriteGutter(Prompt.GetGutterPrompt(PromptStatus.Complete));
            Prompt.WriteText(
                _title.PadRight(Console.WindowWidth - _title.Length - Prompt.GUTTER_PAD_RIGHT)
            );
            Prompt.WriteGutter(Prompt.GetGutterBar(PromptStatus.Complete));
            Prompt.WriteText(resultText.Pastel(_promptOptions.Theme.SpinnerResultForeground));
            Prompt.WriteGutter(Prompt.GetGutterBar(PromptStatus.Complete), newLine: true);
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
