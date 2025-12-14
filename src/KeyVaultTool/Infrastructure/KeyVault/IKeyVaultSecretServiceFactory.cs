using KeyVaultTool.Auth;

namespace KeyVaultTool.Infrastructure.KeyVault;

public interface IKeyVaultSecretServiceFactory
{
    IKeyVaultSecretService Create(AuthOptions authOptions);
}
