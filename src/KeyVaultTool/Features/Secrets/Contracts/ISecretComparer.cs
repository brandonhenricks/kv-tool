using KeyVaultTool.Features.Secrets.Models;

namespace KeyVaultTool.Features.Secrets.Contracts;

public interface ISecretComparer
{
    SecretDiffResult Compare(IReadOnlyList<SecretSnapshot> source, IReadOnlyList<SecretSnapshot> target);
}
