namespace KeyVaultTool.Auth;

public sealed class AuthOptions
{
    public AuthMode Mode { get; init; } = AuthMode.Cli;
    public string? TenantId { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
}
