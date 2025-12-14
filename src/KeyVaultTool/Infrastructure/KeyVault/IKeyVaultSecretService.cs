using KeyVaultTool.Features.Secrets.Models;

namespace KeyVaultTool.Infrastructure.KeyVault;

/// <summary>
/// Provides methods for interacting with Azure Key Vault secrets.
/// </summary>
public interface IKeyVaultSecretService
{
    /// <summary>
    /// Retrieves all secrets from the specified Azure Key Vault.
    /// </summary>
    /// <param name="vaultUri">The URI of the Azure Key Vault.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only list of <see cref="SecretSnapshot"/> objects.
    /// </returns>
    Task<IReadOnlyList<SecretSnapshot>> GetSecretsAsync(Uri vaultUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets or updates a secret in the specified Azure Key Vault.
    /// </summary>
    /// <param name="vaultUri">The URI of the Azure Key Vault.</param>
    /// <param name="secret">The secret to set or update.</param>
    /// <param name="overwrite">If <c>true</c>, overwrites the secret if it already exists; otherwise, does not overwrite.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetSecretAsync(Uri vaultUri, SecretSnapshot secret, bool overwrite, CancellationToken cancellationToken = default);
}
