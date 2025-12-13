namespace KeyVaultTool.Features.Secrets.Models;

public sealed record SecretSnapshot(
    string Name,
    string? Value,
    bool Enabled,
    DateTimeOffset? NotBefore,
    DateTimeOffset? ExpiresOn,
    IReadOnlyDictionary<string, string>? Tags);
