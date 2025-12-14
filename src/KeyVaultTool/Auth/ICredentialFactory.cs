using Azure.Core;

namespace KeyVaultTool.Auth;

/// <summary>
/// Defines a factory for creating <see cref="TokenCredential"/> instances
/// based on the provided <see cref="AuthOptions"/>.
/// </summary>
public interface ICredentialFactory
{
    /// <summary>
    /// Creates a <see cref="TokenCredential"/> using the specified authentication options.
    /// </summary>
    /// <param name="options">The authentication options to use for credential creation.</param>
    /// <returns>
    /// An instance of <see cref="TokenCredential"/> configured according to the provided options.
    /// </returns>
    TokenCredential Create(AuthOptions options);
}
