using KeyVaultTool.Features.Keys.Models;

namespace KeyVaultTool.Infrastructure.KeyVault;

/// <summary>
/// Provides methods for interacting with Azure Key Vault keys, including listing and copying keys between vaults.
/// </summary>
public interface IKeyVaultKeyService
{
    /// <summary>
    /// Retrieves a read-only list of key snapshots from the specified Azure Key Vault.
    /// </summary>
    /// <param name="vaultUri">The URI of the Azure Key Vault.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only list of <see cref="KeySnapshot"/> objects.
    /// </returns>
    Task<IReadOnlyList<KeySnapshot>> GetKeysAsync(Uri vaultUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies a key from a source Azure Key Vault to a target Azure Key Vault.
    /// </summary>
    /// <param name="sourceVault">The URI of the source Azure Key Vault.</param>
    /// <param name="targetVault">The URI of the target Azure Key Vault.</param>
    /// <param name="keyName">The name of the key to copy.</param>
    /// <param name="overwrite">If set to <c>true</c>, overwrites the key in the target vault if it exists; otherwise, does not overwrite.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    Task CopyKeyAsync(Uri sourceVault, Uri targetVault, string keyName, bool overwrite, CancellationToken cancellationToken = default);
}
