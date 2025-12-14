using KeyVaultTool.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace KeyVaultTool.Infrastructure.KeyVault;

public sealed class KeyVaultSecretServiceFactory : IKeyVaultSecretServiceFactory
{
    private readonly IServiceProvider _provider;
    private readonly ICredentialFactory _credentialFactory;

    public KeyVaultSecretServiceFactory(IServiceProvider provider, ICredentialFactory credentialFactory)
    {
        _provider = provider;
        _credentialFactory = credentialFactory;
    }

    public IKeyVaultSecretService Create(AuthOptions authOptions)
    {
        var credential = _credentialFactory.Create(authOptions);
        return ActivatorUtilities.CreateInstance<AzureKeyVaultSecretService>(_provider, credential);
    }
}
