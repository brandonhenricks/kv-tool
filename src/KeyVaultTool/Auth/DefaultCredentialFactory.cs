using Azure.Core;
using Azure.Identity;

namespace KeyVaultTool.Auth;

public sealed class DefaultCredentialFactory : ICredentialFactory
{
    public TokenCredential Create(AuthOptions options)
        => options.Mode switch
        {
            AuthMode.Cli => new DefaultAzureCredential(),

            AuthMode.DeviceCode => new DeviceCodeCredential(new DeviceCodeCredentialOptions
            {
                DeviceCodeCallback = (cb, _) =>
                {
                    Console.WriteLine(cb.Message);
                    return Task.CompletedTask;
                }
            }),

            AuthMode.ServicePrincipal => CreateSpCredential(options),

            _ => throw new NotSupportedException($"Auth mode '{options.Mode}' is not supported.")
        };

    private static ClientSecretCredential CreateSpCredential(AuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.TenantId) ||
            string.IsNullOrWhiteSpace(options.ClientId) ||
            string.IsNullOrWhiteSpace(options.ClientSecret))
        {
            throw new InvalidOperationException("ServicePrincipal auth requires --tenant-id, --client-id, and --client-secret.");
        }

        return new ClientSecretCredential(options.TenantId, options.ClientId, options.ClientSecret);
    }
}
