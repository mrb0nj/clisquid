namespace CliSquid
{
    using System.Drawing;

    public class PromptOptions
    {
        public Color ActiveColour { get; set; } = Color.DodgerBlue;
        public Color CompleteColour { get; set; } = Color.GreenYellow;
        public Color WarningColour { get; set; } = Color.Goldenrod;
        public Color ErrorColour { get; set; } = Color.Crimson;
        public Color MutedColour { get; set; } = Color.DimGray;
        public Color TextColour { get; set; } = Color.WhiteSmoke;
    }
}
