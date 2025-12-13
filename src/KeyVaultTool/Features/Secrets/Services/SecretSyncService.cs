using KeyVaultTool.Features.Secrets.Contracts;
using KeyVaultTool.Infrastructure.KeyVault;

namespace KeyVaultTool.Features.Secrets.Services;

public sealed class SecretSyncService : ISecretSyncService
{
    private readonly IKeyVaultSecretService _secrets;
    private readonly ISecretComparer _comparer;

    public SecretSyncService(IKeyVaultSecretService secrets, ISecretComparer comparer)
    {
        _secrets = secrets;
        _comparer = comparer;
    }

    public async Task SyncAsync(Uri sourceVault, Uri targetVault, bool allowOverwrite, CancellationToken cancellationToken = default)
    {
        var source = await _secrets.GetSecretsAsync(sourceVault, cancellationToken);
        var target = await _secrets.GetSecretsAsync(targetVault, cancellationToken);

        var diff = _comparer.Compare(source, target);

        foreach (var secret in diff.MissingInTarget)
            await _secrets.SetSecretAsync(targetVault, secret, overwrite: false, cancellationToken);

        if (!allowOverwrite)
            return;

        foreach (var (src, _) in diff.ValueDifferences)
            await _secrets.SetSecretAsync(targetVault, src, overwrite: true, cancellationToken);
    }
}
