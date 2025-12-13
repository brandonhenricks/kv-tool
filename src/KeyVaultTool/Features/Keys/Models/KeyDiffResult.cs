namespace KeyVaultTool.Features.Keys.Models;

public sealed record KeyDiffResult(
    IReadOnlyList<KeySnapshot> MissingInTarget,
    IReadOnlyList<KeySnapshot> MissingInSource,
    IReadOnlyList<(KeySnapshot Source, KeySnapshot Target)> AttributeDifferences);
