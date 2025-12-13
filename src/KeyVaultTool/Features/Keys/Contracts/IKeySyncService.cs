namespace KeyVaultTool.Features.Keys.Contracts;

public interface IKeySyncService
{
    Task SyncAsync(Uri sourceVault, Uri targetVault, bool allowOverwrite, CancellationToken cancellationToken = default);
}
