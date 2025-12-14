using KeyVaultTool.Auth;
using KeyVaultTool.Features.Common.Commands;
using KeyVaultTool.Features.Keys.Contracts;
using KeyVaultTool.Features.Keys.Settings;
using Microsoft.Extensions.Logging;

namespace KeyVaultTool.Features.Keys.Commands;

public sealed class SyncKeysCommand : BaseSyncCommand<SyncKeysSettings>
{
    private readonly IKeySyncServiceFactory _syncServiceFactory;

    public SyncKeysCommand(IKeySyncServiceFactory syncServiceFactory, ILogger<SyncKeysCommand> logger)
        : base(logger)
    {
        _syncServiceFactory = syncServiceFactory;
    }

    protected override string StatusMessage => "Syncing keys...";

    protected override string EntityDisplayName => "keys";

    protected override Task ExecuteSyncAsync(AuthOptions authOptions, Uri sourceVault, Uri targetVault, SyncKeysSettings settings, CancellationToken cancellationToken)
    {
        var syncService = _syncServiceFactory.Create(authOptions);
        return syncService.SyncAsync(sourceVault, targetVault, settings.AllowOverwrite, cancellationToken);
    }
}
