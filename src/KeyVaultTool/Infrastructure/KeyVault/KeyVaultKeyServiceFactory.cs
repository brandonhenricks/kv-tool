using KeyVaultTool.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace KeyVaultTool.Infrastructure.KeyVault;

public sealed class KeyVaultKeyServiceFactory : IKeyVaultKeyServiceFactory
{
    private readonly IServiceProvider _provider;
    private readonly ICredentialFactory _credentialFactory;

    public KeyVaultKeyServiceFactory(IServiceProvider provider, ICredentialFactory credentialFactory)
    {
        _provider = provider;
        _credentialFactory = credentialFactory;
    }

    public IKeyVaultKeyService Create(AuthOptions authOptions)
    {
        var credential = _credentialFactory.Create(authOptions);
        return ActivatorUtilities.CreateInstance<AzureKeyVaultKeyService>(_provider, credential);
    }
}
