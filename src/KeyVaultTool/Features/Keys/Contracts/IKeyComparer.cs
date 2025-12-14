using KeyVaultTool.Features.Keys.Models;

namespace KeyVaultTool.Features.Keys.Contracts;

/// <summary>
/// Defines a contract for comparing two collections of <see cref="KeySnapshot"/> objects
/// and producing a <see cref="KeyDiffResult"/> representing the differences.
/// </summary>
public interface IKeyComparer
{
    /// <summary>
    /// Compares the specified source and target collections of <see cref="KeySnapshot"/> objects.
    /// </summary>
    /// <param name="source">The source collection of key snapshots.</param>
    /// <param name="target">The target collection of key snapshots.</param>
    /// <returns>
    /// A <see cref="KeyDiffResult"/> representing the differences between the source and target collections.
    /// </returns>
    KeyDiffResult Compare(IReadOnlyList<KeySnapshot> source, IReadOnlyList<KeySnapshot> target);
}
