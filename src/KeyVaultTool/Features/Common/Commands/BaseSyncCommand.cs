using KeyVaultTool.Auth;
using KeyVaultTool.Shared;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Common.Commands;

public abstract class BaseSyncCommand<TSettings> : AsyncCommand<TSettings>
    where TSettings : SyncCommandSettings
{
    private readonly ILogger _logger;

    protected BaseSyncCommand(ILogger logger)
    {
        _logger = logger;
    }

    protected ILogger Logger => _logger;

    protected abstract string StatusMessage { get; }

    protected abstract string EntityDisplayName { get; }

    protected abstract Task ExecuteSyncAsync(AuthOptions authOptions, Uri sourceVault, Uri targetVault, TSettings settings, CancellationToken cancellationToken);

    protected virtual string ConfirmationText(Uri sourceUri, Uri targetUri, TSettings settings)
        => $"Sync {EntityDisplayName} from [yellow]{sourceUri}[/] to [yellow]{targetUri}[/]?" +
            (settings.AllowOverwrite ? " [red](overwrites enabled)[/]" : " (missing only)");

    public override sealed async Task<int> ExecuteAsync(CommandContext context, TSettings settings, CancellationToken cancellationToken)
    {
        var authOptions = CommandHelpers.BuildAuthOptions(settings.AuthMode, settings.TenantId, settings.ClientId, settings.ClientSecret);
        var sourceUri = CommandHelpers.BuildVaultUri(settings.SourceVault);
        var targetUri = CommandHelpers.BuildVaultUri(settings.TargetVault);

        if (!settings.Yes)
        {
            var proceed = await AnsiConsole.ConfirmAsync(ConfirmationText(sourceUri, targetUri, settings), cancellationToken: cancellationToken);
            if (!proceed)
            {
                _logger.LogInformation("User aborted {Entity} sync from {Source} to {Target}.", EntityDisplayName, sourceUri, targetUri);
                AnsiConsole.MarkupLine("[grey]Aborted.[/]");
                return 0;
            }
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        ConsoleCancelEventHandler onCancel = (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };
        Console.CancelKeyPress += onCancel;

        try
        {
            _logger.LogInformation("Starting {Entity} sync from {Source} to {Target} (Overwrite={AllowOverwrite}).", EntityDisplayName, sourceUri, targetUri, settings.AllowOverwrite);

            await AnsiConsole.Status().StartAsync(StatusMessage, async _ => await ExecuteSyncAsync(authOptions, sourceUri, targetUri, settings, cts.Token));

            _logger.LogInformation("{Entity} sync completed from {Source} to {Target}.", EntityDisplayName, sourceUri, targetUri);
            AnsiConsole.MarkupLine("[green]Sync completed.[/]");
            return 0;
        }
        catch (OperationCanceledException oex)
        {
            _logger.LogError(oex, "{Entity} sync was cancelled.", EntityDisplayName);
            AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during {Entity} sync.", EntityDisplayName);
            AnsiConsole.MarkupLine($"[red]Error during sync: {ex.Message}[/]");
            return -1;
        }
        finally
        {
            Console.CancelKeyPress -= onCancel;
        }
    }
}
