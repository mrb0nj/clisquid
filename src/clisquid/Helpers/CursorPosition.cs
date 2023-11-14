namespace CliSquid
{
    using System;

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
