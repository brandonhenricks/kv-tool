using KeyVaultTool.Features.Secrets.Settings;
using KeyVaultTool.Infrastructure.KeyVault;
using KeyVaultTool.Shared;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Secrets.Commands;

public sealed class ListSecretsCommand : AsyncCommand<ListSecretsSettings>
{
    private readonly IKeyVaultSecretServiceFactory _secretServiceFactory;
    private readonly ILogger<ListSecretsCommand> _logger;

    public ListSecretsCommand(IKeyVaultSecretServiceFactory secretServiceFactory, ILogger<ListSecretsCommand> logger)
    {
        _secretServiceFactory = secretServiceFactory;
        _logger = logger;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ListSecretsSettings settings, CancellationToken cancellationToken)
    {
        var authOptions = CommandHelpers.BuildAuthOptions(settings.AuthMode, settings.TenantId, settings.ClientId, settings.ClientSecret);
        var vaultUri = CommandHelpers.BuildVaultUri(settings.Vault);

        if (settings.ShowValues)
            AnsiConsole.MarkupLine("[yellow]Warning: secret values will be printed to the console.[/]");

        try
        {
            var secretService = _secretServiceFactory.Create(authOptions);
            var secrets = await secretService.GetSecretsAsync(vaultUri, cancellationToken);

            var table = new Table()
                .AddColumn("Name")
                .AddColumn("Enabled")
                .AddColumn("NotBefore")
                .AddColumn("ExpiresOn")
                .AddColumn(settings.ShowValues ? "Value" : "Value (hidden)");

            foreach (var s in secrets)
            {
                table.AddRow(
                    s.Name,
                    s.Enabled ? "true" : "false",
                    s.NotBefore?.ToString("u") ?? string.Empty,
                    s.ExpiresOn?.ToString("u") ?? string.Empty,
                    settings.ShowValues ? (s.Value ?? string.Empty) : "********");
            }

            AnsiConsole.Write(table);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list secrets from {Vault}.", vaultUri);
            AnsiConsole.MarkupLine($"[red]Error listing secrets: {ex.Message}[/]");
            return -1;
        }
    }
}
