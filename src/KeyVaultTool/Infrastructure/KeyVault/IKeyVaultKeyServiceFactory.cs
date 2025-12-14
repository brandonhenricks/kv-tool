using KeyVaultTool.Auth;

namespace KeyVaultTool.Infrastructure.KeyVault;

/// <summary>
/// Defines a factory for creating <see cref="IKeyVaultKeyService"/> instances
/// using specified authentication options.
/// </summary>
public interface IKeyVaultKeyServiceFactory
{
    /// <summary>
    /// Creates a new <see cref="IKeyVaultKeyService"/> using the provided authentication options.
    /// </summary>
    /// <param name="authOptions">The authentication options to use for service creation.</param>
    /// <returns>
    /// An instance of <see cref="IKeyVaultKeyService"/> configured with the given authentication options.
    /// </returns>
    IKeyVaultKeyService Create(AuthOptions authOptions);
}
