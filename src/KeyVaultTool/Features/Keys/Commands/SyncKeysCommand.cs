using Azure.Core;
using KeyVaultTool.Auth;
using KeyVaultTool.Features.Keys.Contracts;
using KeyVaultTool.Features.Keys.Services;
using KeyVaultTool.Features.Keys.Settings;
using KeyVaultTool.Infrastructure.KeyVault;
using KeyVaultTool.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Keys.Commands;

public sealed class SyncKeysCommand : AsyncCommand<SyncKeysSettings>
{
    private readonly ICredentialFactory _credentialFactory;
    private readonly IKeyComparer _comparer;

    public SyncKeysCommand(ICredentialFactory credentialFactory, IKeyComparer comparer)
    {
        _credentialFactory = credentialFactory;
        _comparer = comparer;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, SyncKeysSettings settings, CancellationToken cancellationToken)
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

        var sourceUri = CommandHelpers.BuildVaultUri(settings.SourceVault);
        var targetUri = CommandHelpers.BuildVaultUri(settings.TargetVault);

        if (!settings.Yes)
        {
            var proceed = await AnsiConsole.ConfirmAsync(
                $"Sync keys from [yellow]{sourceUri}[/] to [yellow]{targetUri}[/]?" +
                (settings.AllowOverwrite ? " [red](overwrites enabled)[/]" : " (missing only)"), cancellationToken: cancellationToken);

            if (!proceed)
            {
                AnsiConsole.MarkupLine("[grey]Aborted.[/]");
                return 0;
            }
        }

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        try
        {
            await AnsiConsole.Status().StartAsync("Syncing keys...", async _ =>
            {
                var keyService = new AzureKeyVaultKeyService(credential);
                var syncService = new KeySyncService(keyService, _comparer);

                await syncService.SyncAsync(sourceUri, targetUri, settings.AllowOverwrite, cts.Token);
            });

            AnsiConsole.MarkupLine("[green]Sync completed.[/]");
            return 0;
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
            return -1;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error during sync: {ex.Message}[/]");
            return -1;
        }
    }
}
