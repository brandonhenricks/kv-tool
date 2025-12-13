using Azure.Core;
using KeyVaultTool.Auth;
using KeyVaultTool.Features.Keys.Settings;
using KeyVaultTool.Infrastructure.KeyVault;
using KeyVaultTool.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Keys.Commands;

public sealed class ListKeysCommand : AsyncCommand<ListKeysSettings>
{
    private readonly ICredentialFactory _credentialFactory;

    public ListKeysCommand(ICredentialFactory credentialFactory) => _credentialFactory = credentialFactory;

    public override async Task<int> ExecuteAsync(CommandContext context, ListKeysSettings settings, CancellationToken cancellationToken)
    {
        var authOptions = CommandHelpers.BuildAuthOptions(settings.AuthMode, settings.TenantId, settings.ClientId, settings.ClientSecret);

        TokenCredential credential;
        try
        {
            credential = _credentialFactory.Create(authOptions);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create credential: {ex.Message}[/]");
            return -1;
        }

        var vaultUri = CommandHelpers.BuildVaultUri(settings.Vault);
        var keyService = new AzureKeyVaultKeyService(credential);

        try
        {
            var keys = await keyService.GetKeysAsync(vaultUri);

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
            AnsiConsole.MarkupLine($"[red]Error listing keys: {ex.Message}[/]");
            return -1;
        }
    }
}
