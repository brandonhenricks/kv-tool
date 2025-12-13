namespace KeyVaultTool.Features.Keys.Models;

public sealed record KeySnapshot(
    string Name,
    bool Enabled,
    string KeyType,
    IReadOnlyDictionary<string, string>? Tags);
