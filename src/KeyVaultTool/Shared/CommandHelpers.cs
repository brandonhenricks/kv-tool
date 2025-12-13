using KeyVaultTool.Auth;

namespace KeyVaultTool.Shared;

internal static class CommandHelpers
{
    public static Uri BuildVaultUri(string vaultNameOrUri)
        => vaultNameOrUri.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? new Uri(vaultNameOrUri, UriKind.Absolute)
            : new Uri($"https://{vaultNameOrUri}.vault.azure.net/", UriKind.Absolute);

    public static AuthMode ParseAuthMode(string? value)
        => value?.ToLowerInvariant() switch
        {
            "cli" => AuthMode.Cli,
            "devicecode" => AuthMode.DeviceCode,
            "sp" => AuthMode.ServicePrincipal,
            _ => AuthMode.Cli
        };

    public static AuthOptions BuildAuthOptions(string? authMode, string? tenantId, string? clientId, string? clientSecret)
        => new()
        {
            Mode = ParseAuthMode(authMode),
            TenantId = tenantId,
            ClientId = clientId,
            ClientSecret = clientSecret
        };
}
