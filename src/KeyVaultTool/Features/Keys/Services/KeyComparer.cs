using KeyVaultTool.Features.Keys.Contracts;
using KeyVaultTool.Features.Keys.Models;

namespace KeyVaultTool.Features.Keys.Services;

public sealed class KeyComparer : IKeyComparer
{
    public KeyDiffResult Compare(IReadOnlyList<KeySnapshot> source, IReadOnlyList<KeySnapshot> target)
    {
        var sourceByName = source.ToDictionary(k => k.Name, StringComparer.OrdinalIgnoreCase);
        var targetByName = target.ToDictionary(k => k.Name, StringComparer.OrdinalIgnoreCase);

        var missingInTarget = new List<KeySnapshot>();
        var missingInSource = new List<KeySnapshot>();
        var attributeDifferences = new List<(KeySnapshot Source, KeySnapshot Target)>();

        foreach (var (name, src) in sourceByName)
        {
            if (!targetByName.TryGetValue(name, out var tgt))
            {
                missingInTarget.Add(src);
                continue;
            }

            if (src.Enabled != tgt.Enabled ||
                !string.Equals(src.KeyType, tgt.KeyType, StringComparison.OrdinalIgnoreCase))
            {
                attributeDifferences.Add((src, tgt));
            }
        }

        foreach (var (name, tgt) in targetByName)
        {
            if (!sourceByName.ContainsKey(name))
                missingInSource.Add(tgt);
        }

        return new KeyDiffResult(missingInTarget, missingInSource, attributeDifferences);
    }
}
