using KeyVaultTool.Auth;

namespace KeyVaultTool.Shared;

/// <summary>
/// Provides helper methods for command operations, including vault URI construction and authentication options parsing.
/// </summary>
internal static class CommandHelpers
{
    /// <summary>
    /// Builds a valid Azure Key Vault URI from a vault name or a full URI string.
    /// </summary>
    /// <param name="vaultNameOrUri">
    /// The vault name (e.g., "myvault") or a full URI (e.g., "https://myvault.vault.azure.net/").
    /// </param>
    /// <returns>
    /// An absolute <see cref="Uri"/> representing the Azure Key Vault endpoint.
    /// </returns>
    public static Uri BuildVaultUri(string vaultNameOrUri)
        => vaultNameOrUri.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? new Uri(vaultNameOrUri, UriKind.Absolute)
            : new Uri($"https://{vaultNameOrUri}.vault.azure.net/", UriKind.Absolute);

    /// <summary>
    /// Parses a string value into an <see cref="AuthMode"/> enumeration.
    /// </summary>
    /// <param name="value">
    /// The authentication mode as a string ("cli", "devicecode", or "sp").
    /// </param>
    /// <returns>
    /// The corresponding <see cref="AuthMode"/> value. Defaults to <see cref="AuthMode.Cli"/> if unrecognized.
    /// </returns>
    public static AuthMode ParseAuthMode(string? value)
        => value?.ToLowerInvariant() switch
        {
            "cli" => AuthMode.Cli,
            "devicecode" => AuthMode.DeviceCode,
            "sp" => AuthMode.ServicePrincipal,
            _ => AuthMode.Cli
        };

    /// <summary>
    /// Constructs an <see cref="AuthOptions"/> instance from the provided authentication parameters.
    /// </summary>
    /// <param name="authMode">The authentication mode as a string.</param>
    /// <param name="tenantId">The Azure Active Directory tenant ID.</param>
    /// <param name="clientId">The client (application) ID.</param>
    /// <param name="clientSecret">The client secret.</param>
    /// <returns>
    /// An <see cref="AuthOptions"/> object populated with the specified values.
    /// </returns>
    public static AuthOptions BuildAuthOptions(string? authMode, string? tenantId, string? clientId, string? clientSecret)
        => new()
        {
            Mode = ParseAuthMode(authMode),
            TenantId = tenantId,
            ClientId = clientId,
            ClientSecret = clientSecret
        };
}
