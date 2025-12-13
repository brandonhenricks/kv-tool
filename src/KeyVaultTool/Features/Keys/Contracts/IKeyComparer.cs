using KeyVaultTool.Features.Keys.Models;

namespace KeyVaultTool.Features.Keys.Contracts;

public interface IKeyComparer
{
    KeyDiffResult Compare(IReadOnlyList<KeySnapshot> source, IReadOnlyList<KeySnapshot> target);
}
