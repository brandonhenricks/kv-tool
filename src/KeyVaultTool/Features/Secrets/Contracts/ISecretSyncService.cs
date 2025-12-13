namespace KeyVaultTool.Features.Secrets.Contracts;

public interface ISecretSyncService
{
    Task SyncAsync(Uri sourceVault, Uri targetVault, bool allowOverwrite, CancellationToken cancellationToken = default);
}
