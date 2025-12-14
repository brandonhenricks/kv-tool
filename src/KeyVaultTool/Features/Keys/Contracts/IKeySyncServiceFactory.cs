using KeyVaultTool.Auth;

namespace KeyVaultTool.Features.Keys.Contracts;

/// <summary>
/// Factory interface for creating <see cref="IKeySyncService"/> instances
/// with the specified authentication options.
/// </summary>
public interface IKeySyncServiceFactory
{
    /// <summary>
    /// Creates a new <see cref="IKeySyncService"/> using the provided authentication options.
    /// </summary>
    /// <param name="authOptions">The authentication options to use for the service.</param>
    /// <returns>An instance of <see cref="IKeySyncService"/> configured with the given authentication options.</returns>
    IKeySyncService Create(AuthOptions authOptions);
}
