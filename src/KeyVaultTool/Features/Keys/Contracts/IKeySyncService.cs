namespace KeyVaultTool.Features.Keys.Contracts;

/// <summary>
/// Defines a service for synchronizing keys between two Azure Key Vault instances.
/// </summary>
public interface IKeySyncService
{
    /// <summary>
    /// Synchronizes keys from the source vault to the target vault.
    /// </summary>
    /// <param name="sourceVault">The URI of the source Azure Key Vault.</param>
    /// <param name="targetVault">The URI of the target Azure Key Vault.</param>
    /// <param name="allowOverwrite">
    /// If <c>true</c>, existing keys in the target vault may be overwritten; otherwise, only missing keys are copied.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous sync operation.</returns>
    Task SyncAsync(Uri sourceVault, Uri targetVault, bool allowOverwrite, CancellationToken cancellationToken = default);
}
