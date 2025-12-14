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
        _console.WriteLine();
        _console.MarkupLine("[bold yellow]KeyVaultTool[/] - A tool for managing Azure Key Vault resources.");
        _console.MarkupLine("Use [green]--help[/] to see available commands and options.");
        _console.WriteLine();
        return 0;
    }
}
