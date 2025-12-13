using KeyVaultTool.Features.Secrets.Models;

namespace KeyVaultTool.Infrastructure.KeyVault;

public interface IKeyVaultSecretService
{
    Task<IReadOnlyList<SecretSnapshot>> GetSecretsAsync(Uri vaultUri, CancellationToken cancellationToken = default);

    Task SetSecretAsync(Uri vaultUri, SecretSnapshot secret, bool overwrite, CancellationToken cancellationToken = default);
}
