# CliSquid

Fancy prompts for dotnet console applications. Heavily "inspired" by [Clack](https://github.com/fukamachi/clack) and [Bubbles](https://github.com/charmbracelet/bubbles)

## VERY EARLY RELEASE USE AT YOUR OWN PERIL!!

### Installation

NuGet package is hosted on [NuGet](https://www.nuget.org/packages/CliSquid/) and can be installed from your package manager

```
PM> Install-Package CliSquid --prerelease
```

or from the dotnet cli

```
> dotnet add package CliSquid --prerelease
```

### Usage

Configure the library, currently this is where you could set theme options and other settings in the future
```csharp
using CliSquid;
using CliSquid.Enums;
using CliSquid.Themes;

Prompt.Configure(options =>
{
    options.Theme = Theme.GetTheme(PromptTheme.Default);
});
```

#### Prompts

##### Intro - informational prompt with some styling, bit like an app header

```csharp

Prompt.Intro("my-app-name");

```

##### UserInput - capture text input from the user

```csharp

// text
var name = Prompt
    .FromUserInput<string>()
    .Title("What is your name?")
    .Placeholder("Ezekiel")
    .Validate(
        (string x) =>
        {
            if (x == "Tony" || x == "Ezekiel")
                return Tuple.Create(true, string.Empty);
            else
                return Tuple.Create(false, "You can only be Tony or Ezekiel");
        }
    )
    .Read();

var name2 = Prompt
    .FromUserInput<string>()
    .Title("What is your name?")
    .Placeholder("Tony")
    .Validate(
        (string x) =>
        {
            if (x.Equals(name, StringComparison.OrdinalIgnoreCase))
                return Tuple.Create(false, string.Format("You can't both be {0}", x));
            else if (x == "Tony" || x == "Ezekiel")
                return Tuple.Create(true, string.Empty);
            else
                return Tuple.Create(false, "You can only be Ezekiel or Tony");
        }
    )
    .Read();
```

Input validtation returns a Tuple<bool, string> to represent the valid status and any error message that might be required in the console.

##### Select list

From Enum:
```csharp

public enum NotificationStatus
{
    Scheduled,
    InProgress,
    Complete,
    RolledBack
}

var status = Prompt
		.FromEnum<NotificationStatus>()
		.Title("Notification status")
		.SelectOne();
```

Single value from custom list:
```csharp

var options = new List<PromptOption<string>>()
{
    new PromptOption<string>("Hello") { Display = "Hi!" },
    new PromptOption<string>("Goodbye") { Hint = "not hello", IsDefault = true }
};
var greeting = Prompt.FromList<string>().Title("Select one").Options(options).SelectOne();
```

Multiple values from custom list:
```csharp
var tech = new List<string> { "Typescript", "Tailwind", "React", "dotnet", "Vuejs" };
var techOptions = tech.Select(t => new PromptOption<string>(t)).ToList();
techOptions[1].IsDefault = true;
techOptions[1].Hint = "[recommended]";
techOptions[2].IsDefault = true;
techOptions[2].Hint = "[recommended]";

var selectedTech = Prompt
    .FromList<string>()
    .Title("Choose your tech stack")
    .Options(techOptions)
    .SelectMany();
```

or optionally as an inline list:
```csharp
var selectedTech = Prompt
    .FromList<string>()
    .Inline()
    .Title("Choose your tech stack")
    .Options(techOptions)
    .SelectMany();
```

##### Spinner

Show an 'animated' cursor while performing other actions (TODO: Async version)
```csharp
var result = Prompt
    .Spinner<string>()
    .Title("Perfoming complicated stuff")
    .SetSpinnerType(SpinnerType.Dots)
    .Spin(p =>
    {
        p.SetStatus("Starting...");
        for (var i = 0; i < 2; i++)
        {
            p.SetStatus(string.Format("Processing... {0}", i));
            Task.Delay(2000).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        p.SetStatus("Completed..");
        return "Finished doing something time consuming...";
    });
```

##### Confirm returns true/false

```csharp
var confirm = Prompt.Confirm("Are you sure?");
```

##### Complete

Used at the end of the application flow

```csharp
Prompt.Complete("Fuck you, Tony!");
```
