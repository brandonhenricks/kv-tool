using KeyVaultTool.Features.Keys.Contracts;
using KeyVaultTool.Infrastructure.KeyVault;

namespace KeyVaultTool.Features.Keys.Services;

public sealed class KeySyncService : IKeySyncService
{
    private readonly IKeyVaultKeyService _keys;
    private readonly IKeyComparer _comparer;

    public KeySyncService(IKeyVaultKeyService keys, IKeyComparer comparer)
    {
        _keys = keys;
        _comparer = comparer;
    }

    public async Task SyncAsync(Uri sourceVault, Uri targetVault, bool allowOverwrite, CancellationToken cancellationToken = default)
    {
        var source = await _keys.GetKeysAsync(sourceVault, cancellationToken);
        var target = await _keys.GetKeysAsync(targetVault, cancellationToken);

        var diff = _comparer.Compare(source, target);

        foreach (var key in diff.MissingInTarget)
            await _keys.CopyKeyAsync(sourceVault, targetVault, key.Name, overwrite: false, cancellationToken);

        if (!allowOverwrite)
            return;

        foreach (var (src, _) in diff.AttributeDifferences)
            await _keys.CopyKeyAsync(sourceVault, targetVault, src.Name, overwrite: true, cancellationToken);
    }
}
