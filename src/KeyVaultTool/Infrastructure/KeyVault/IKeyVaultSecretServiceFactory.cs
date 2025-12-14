using KeyVaultTool.Auth;

namespace KeyVaultTool.Infrastructure.KeyVault;

/// <summary>
/// Defines a factory for creating <see cref="IKeyVaultSecretService"/> instances
/// using specified authentication options.
/// </summary>
public interface IKeyVaultSecretServiceFactory
{
    /// <summary>
    /// Creates a new <see cref="IKeyVaultSecretService"/> using the provided authentication options.
    /// </summary>
    /// <param name="authOptions">The authentication options to use for service creation.</param>
    /// <returns>An instance of <see cref="IKeyVaultSecretService"/> configured with the given authentication options.</returns>
    IKeyVaultSecretService Create(AuthOptions authOptions);
}
