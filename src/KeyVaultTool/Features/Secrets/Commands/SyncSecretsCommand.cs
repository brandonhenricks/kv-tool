using KeyVaultTool.Auth;
using KeyVaultTool.Features.Common.Commands;
using KeyVaultTool.Features.Secrets.Contracts;
using KeyVaultTool.Features.Secrets.Settings;
using Microsoft.Extensions.Logging;

namespace KeyVaultTool.Features.Secrets.Commands;

public sealed class SyncSecretsCommand : BaseSyncCommand<SyncSecretsSettings>
{
    private readonly ISecretSyncServiceFactory _syncServiceFactory;

    public SyncSecretsCommand(ISecretSyncServiceFactory syncServiceFactory, ILogger<SyncSecretsCommand> logger)
        : base(logger)
    {
        _syncServiceFactory = syncServiceFactory;
    }

    protected override string StatusMessage => "Syncing secrets...";

    protected override string EntityDisplayName => "secrets";

    protected override Task ExecuteSyncAsync(AuthOptions authOptions, Uri sourceVault, Uri targetVault, SyncSecretsSettings settings, CancellationToken cancellationToken)
    {
        var syncService = _syncServiceFactory.Create(authOptions);
        return syncService.SyncAsync(sourceVault, targetVault, settings.AllowOverwrite, cancellationToken);
    }
}
