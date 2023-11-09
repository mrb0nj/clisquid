namespace CliSquid
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    internal class PrefixWriter : TextWriter
    {
        private TextWriter _output;

        public PrefixWriter()
        {
            _output = Console.Out;
        }

        public override Encoding Encoding => System.Text.Encoding.UTF8;

        public override IFormatProvider FormatProvider => _output.FormatProvider;

        public override string NewLine
        {
            get => _output.NewLine;
            set => _output.NewLine = value;
        }

        public override void Close()
        {
            _output.Close();
        }

        public override ValueTask DisposeAsync()
        {
            return _output.DisposeAsync();
        }

        public override void WriteLine(string message)
        {
            var pos = CursorPosition.GetCursorPosition();
            if (pos.Left == 0)
                message = string.Format(
                    "{0} {1}",
                    Prompt.GetGutterBar(PromptStatus.Complete),
                    message
                );

            _output.WriteLine(message);
        }

        public override void Write(string message)
        {
            var pos = CursorPosition.GetCursorPosition();
            if (pos.Left == 0)
                message = string.Format(
                    "{0} {1}",
                    Prompt.GetGutterBar(PromptStatus.Complete),
                    message
                );

            _output.Write(message);
        }
    }
}
