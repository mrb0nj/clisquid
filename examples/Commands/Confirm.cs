
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CliSquid;

[Command("confirm", Description = "Display the confirmation prompt")]
public class ConfirmPrompt : ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var confirmed = Prompt.Confirm("Are you sure you want to do this?");
        Prompt.Complete(string.Format("You selected: {0}", confirmed));

        return default;
    }
}
