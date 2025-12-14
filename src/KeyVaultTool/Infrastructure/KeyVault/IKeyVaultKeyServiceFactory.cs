using KeyVaultTool.Auth;

namespace KeyVaultTool.Infrastructure.KeyVault;

public interface IKeyVaultKeyServiceFactory
{
    IKeyVaultKeyService Create(AuthOptions authOptions);
}
