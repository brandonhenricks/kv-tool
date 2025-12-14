using KeyVaultTool.Auth;

namespace KeyVaultTool.Features.Keys.Contracts;

public interface IKeySyncServiceFactory
{
    IKeySyncService Create(AuthOptions authOptions);
}
