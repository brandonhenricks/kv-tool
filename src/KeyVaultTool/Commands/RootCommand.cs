using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Commands;

public sealed class RootCommand : Command
{
    private readonly IAnsiConsole _console;

    public RootCommand(IAnsiConsole console)
    {
        _console = console;
    }

    public override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        _console.Clear();
        _console.Write(
            new FigletText("KeyVaultTool")
                .Centered()
                .Color(Color.Yellow));
        _console.MarkupLine("[bold yellow]A tool for managing Azure Key Vault resources.[/]");
        _console.WriteLine();
        _console.MarkupLine("[green]Usage:[/] [bold]kvtool <command>[/]");
        _console.WriteLine();
        _console.MarkupLine("[bold]Available commands:[/]");
        _console.MarkupLine("  [cyan]secrets[/]   [grey]Manage secrets[/]");
        _console.MarkupLine("  [cyan]keys[/]      [grey]Manage keys[/]");
        _console.MarkupLine("  [cyan]compare[/]   [grey]Compare vaults[/]");
        _console.MarkupLine("  [cyan]sync[/]      [grey]Sync secrets/keys[/]");
        _console.WriteLine();
        _console.MarkupLine("Use [green]--help[/] to see all available commands and options.");
        _console.WriteLine();
        return 0;
    }
}
