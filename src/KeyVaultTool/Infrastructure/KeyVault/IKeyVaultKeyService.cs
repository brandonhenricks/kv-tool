using KeyVaultTool.Features.Keys.Models;

namespace KeyVaultTool.Infrastructure.KeyVault;

public interface IKeyVaultKeyService
{
    Task<IReadOnlyList<KeySnapshot>> GetKeysAsync(Uri vaultUri, CancellationToken cancellationToken = default);

    Task CopyKeyAsync(Uri sourceVault, Uri targetVault, string keyName, bool overwrite, CancellationToken cancellationToken = default);
}
