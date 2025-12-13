using Azure.Core;

namespace KeyVaultTool.Auth;

public interface ICredentialFactory
{
    TokenCredential Create(AuthOptions options);
}
