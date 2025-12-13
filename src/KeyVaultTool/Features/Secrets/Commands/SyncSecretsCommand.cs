using Azure.Core;
using KeyVaultTool.Auth;
using KeyVaultTool.Features.Secrets.Contracts;
using KeyVaultTool.Features.Secrets.Services;
using KeyVaultTool.Features.Secrets.Settings;
using KeyVaultTool.Infrastructure.KeyVault;
using KeyVaultTool.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Secrets.Commands;

public sealed class SyncSecretsCommand : AsyncCommand<SyncSecretsSettings>
{
    private readonly ICredentialFactory _credentialFactory;
    private readonly ISecretComparer _comparer;

    public SyncSecretsCommand(ICredentialFactory credentialFactory, ISecretComparer comparer)
    {
        _credentialFactory = credentialFactory;
        _comparer = comparer;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, SyncSecretsSettings settings, CancellationToken cancellationToken)
    {
        var authOptions = CommandHelpers.BuildAuthOptions(settings.AuthMode, settings.TenantId, settings.ClientId, settings.ClientSecret);

        TokenCredential credential;
        try { credential = _credentialFactory.Create(authOptions); }
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
                $"Sync secrets from [yellow]{sourceUri}[/] to [yellow]{targetUri}[/]?" +
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
            await AnsiConsole.Status().StartAsync("Syncing secrets...", async _ =>
            {
                var secretService = new AzureKeyVaultSecretService(credential);
                var syncService = new SecretSyncService(secretService, _comparer);

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
