using Azure.Core;
using KeyVaultTool.Auth;
using KeyVaultTool.Features.Secrets.Settings;
using KeyVaultTool.Infrastructure.KeyVault;
using KeyVaultTool.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Secrets.Commands;

public sealed class ListSecretsCommand : AsyncCommand<ListSecretsSettings>
{
    private readonly ICredentialFactory _credentialFactory;

    public ListSecretsCommand(ICredentialFactory credentialFactory) => _credentialFactory = credentialFactory;

    public override async Task<int> ExecuteAsync(CommandContext context, ListSecretsSettings settings, CancellationToken cancellationToken)
    {
        var authOptions = CommandHelpers.BuildAuthOptions(settings.AuthMode, settings.TenantId, settings.ClientId, settings.ClientSecret);

        TokenCredential credential;
        try { credential = _credentialFactory.Create(authOptions); }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create credential: {ex.Message}[/]");
            return -1;
        }

        var vaultUri = CommandHelpers.BuildVaultUri(settings.Vault);
        var secretService = new AzureKeyVaultSecretService(credential);

        if (settings.ShowValues)
            AnsiConsole.MarkupLine("[yellow]Warning: secret values will be printed to the console.[/]");

        try
        {
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
            AnsiConsole.MarkupLine($"[red]Error listing secrets: {ex.Message}[/]");
            return -1;
        }
    }
}
