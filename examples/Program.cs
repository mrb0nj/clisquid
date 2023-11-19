// See https://aka.ms/new-console-template for more information
using CliFx;
using CliSquid;
using CliSquid.Enums;
using CliSquid.Themes;

Prompt.Configure(opts =>
{
            opts.Theme = Theme.GetTheme(PromptTheme.Default);
});
await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();
