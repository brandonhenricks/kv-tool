using KeyVaultTool.Auth;
using KeyVaultTool.Features.Keys.Contracts;
using KeyVaultTool.Infrastructure.KeyVault;

namespace KeyVaultTool.Features.Keys.Services;

public sealed class KeySyncServiceFactory : IKeySyncServiceFactory
{
    private readonly IKeyComparer _comparer;
    private readonly IKeyVaultKeyServiceFactory _keyVaultKeyServiceFactory;

    public KeySyncServiceFactory(IKeyVaultKeyServiceFactory keyVaultKeyServiceFactory, IKeyComparer comparer)
    {
        _keyVaultKeyServiceFactory = keyVaultKeyServiceFactory;
        _comparer = comparer;
    }

    public IKeySyncService Create(AuthOptions authOptions)
    {
        var keyService = _keyVaultKeyServiceFactory.Create(authOptions);
        return new KeySyncService(keyService, _comparer);
    }
}
