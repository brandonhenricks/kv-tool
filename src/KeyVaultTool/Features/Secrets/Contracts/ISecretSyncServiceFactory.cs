using KeyVaultTool.Auth;

namespace KeyVaultTool.Features.Secrets.Contracts;

public interface ISecretSyncServiceFactory
{
    ISecretSyncService Create(AuthOptions authOptions);
}
