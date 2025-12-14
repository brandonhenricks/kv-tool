using System.Linq;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Commands;

public sealed class HelpCommand : Command
{
    private readonly IAnsiConsole _console;
    private readonly CommandRegistry _registry;

    public HelpCommand(IAnsiConsole console, CommandRegistry registry)
    {
        _console = console;
        _registry = registry;
    }

    public override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        _console.WriteLine();

        var table = new Table()
            .AddColumn(new TableColumn("[bold cyan]Command[/]")
                .Centered())
            .AddColumn(new TableColumn("[bold]Description[/]")
                .LeftAligned());

        foreach (var descriptor in CommandRegistry.Commands.Concat(new[] { _registry.HelpCommand }))
        {
            table.AddRow($"[yellow]{descriptor.Name}[/]", descriptor.Description);
        }

        _console.WriteLine();
        _console.MarkupLine("[bold yellow]KeyVaultTool[/] available commands:");
        _console.Write(table);
        _console.WriteLine();
        _console.MarkupLine("Use [green]kvtool <command> --help[/] for command-specific options.");

        return 0;
    }
}
