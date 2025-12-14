using KeyVaultTool.Features.Keys.Settings;
using KeyVaultTool.Infrastructure.KeyVault;
using KeyVaultTool.Shared;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Keys.Commands;

public sealed class ListKeysCommand : AsyncCommand<ListKeysSettings>
{
    private readonly IKeyVaultKeyServiceFactory _keyServiceFactory;
    private readonly ILogger<ListKeysCommand> _logger;

    public ListKeysCommand(IKeyVaultKeyServiceFactory keyServiceFactory, ILogger<ListKeysCommand> logger)
    {
        _keyServiceFactory = keyServiceFactory;
        _logger = logger;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ListKeysSettings settings, CancellationToken cancellationToken)
    {
        var authOptions = CommandHelpers.BuildAuthOptions(settings.AuthMode, settings.TenantId, settings.ClientId, settings.ClientSecret);
        var vaultUri = CommandHelpers.BuildVaultUri(settings.Vault);

        try
        {
            var keyService = _keyServiceFactory.Create(authOptions);
            var keys = await keyService.GetKeysAsync(vaultUri, cancellationToken);

            var table = new Table()
                .AddColumn("Name")
                .AddColumn("Enabled")
                .AddColumn("Type")
                .AddColumn("Tags");

            foreach (var key in keys)
            {
                table.AddRow(
                    key.Name,
                    key.Enabled ? "true" : "false",
                    key.KeyType,
                    key.Tags is { Count: > 0 }
                        ? string.Join(", ", key.Tags.Select(kvp => $"{kvp.Key}={kvp.Value}"))
                        : string.Empty);
            }

            AnsiConsole.Write(table);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list keys from {Vault}.", vaultUri);
            AnsiConsole.MarkupLine($"[red]Error listing keys: {ex.Message}[/]");
            return -1;
        }
    }
}
