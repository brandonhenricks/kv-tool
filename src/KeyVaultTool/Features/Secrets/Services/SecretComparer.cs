using KeyVaultTool.Features.Secrets.Contracts;
using KeyVaultTool.Features.Secrets.Models;

namespace KeyVaultTool.Features.Secrets.Services;

public sealed class SecretComparer : ISecretComparer
{
    public SecretDiffResult Compare(IReadOnlyList<SecretSnapshot> source, IReadOnlyList<SecretSnapshot> target)
    {
        var sourceByName = source.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);
        var targetByName = target.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);

        var missingInTarget = new List<SecretSnapshot>();
        var missingInSource = new List<SecretSnapshot>();
        var different = new List<(SecretSnapshot Source, SecretSnapshot Target)>();

        foreach (var (name, src) in sourceByName)
        {
            if (!targetByName.TryGetValue(name, out var tgt))
            {
                missingInTarget.Add(src);
                continue;
            }

            if (!string.Equals(src.Value, tgt.Value, StringComparison.Ordinal))
                different.Add((src, tgt));
        }

        foreach (var (name, tgt) in targetByName)
        {
            if (!sourceByName.ContainsKey(name))
                missingInSource.Add(tgt);
        }

        return new SecretDiffResult(missingInTarget, missingInSource, different);
    }
}
