namespace KeyVaultTool.Features.Secrets.Models;

public sealed record SecretDiffResult(
    IReadOnlyList<SecretSnapshot> MissingInTarget,
    IReadOnlyList<SecretSnapshot> MissingInSource,
    IReadOnlyList<(SecretSnapshot Source, SecretSnapshot Target)> ValueDifferences);
