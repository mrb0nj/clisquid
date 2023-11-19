using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CliSquid;

[Command("intro", Description = "Display the intro prompt")]
public class IntroCommand : ICommand
{
    [CommandParameter(0, IsRequired = false)]
    public string Intro { get; set; } = "my-awesome-app";

    public ValueTask ExecuteAsync(IConsole console)
    {
        Prompt.Intro(Intro);
        return default;
    }
}
