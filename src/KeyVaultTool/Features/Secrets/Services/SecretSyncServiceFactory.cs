using KeyVaultTool.Auth;
using KeyVaultTool.Features.Secrets.Contracts;
using KeyVaultTool.Infrastructure.KeyVault;

namespace KeyVaultTool.Features.Secrets.Services;

public sealed class SecretSyncServiceFactory : ISecretSyncServiceFactory
{
    private readonly ISecretComparer _comparer;
    private readonly IKeyVaultSecretServiceFactory _keyVaultSecretServiceFactory;

    public SecretSyncServiceFactory(IKeyVaultSecretServiceFactory secretServiceFactory, ISecretComparer comparer)
    {
        _keyVaultSecretServiceFactory = secretServiceFactory;
        _comparer = comparer;
    }

    public ISecretSyncService Create(AuthOptions authOptions)
    {
        var secretService = _keyVaultSecretServiceFactory.Create(authOptions);
        return new SecretSyncService(secretService, _comparer);
    }
}
