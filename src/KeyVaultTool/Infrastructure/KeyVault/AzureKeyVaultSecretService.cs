using Azure;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using KeyVaultTool.Features.Secrets.Models;

namespace KeyVaultTool.Infrastructure.KeyVault;

public sealed class AzureKeyVaultSecretService : IKeyVaultSecretService
{
    private readonly TokenCredential _credential;

    public AzureKeyVaultSecretService(TokenCredential credential) => _credential = credential;

    public async Task<IReadOnlyList<SecretSnapshot>> GetSecretsAsync(Uri vaultUri, CancellationToken cancellationToken = default)
    {
        var client = new SecretClient(vaultUri, _credential);
        var results = new List<SecretSnapshot>();

        await foreach (SecretProperties props in client.GetPropertiesOfSecretsAsync(cancellationToken))
        {
            KeyVaultSecret secret = await client.GetSecretAsync(props.Name, cancellationToken: cancellationToken);
            var tags = secret.Properties.Tags is { Count: > 0 }
                ? new Dictionary<string, string>(secret.Properties.Tags)
                : null;

            results.Add(new SecretSnapshot(
                Name: secret.Name,
                Value: secret.Value,
                Enabled: secret.Properties.Enabled ?? true,
                NotBefore: secret.Properties.NotBefore,
                ExpiresOn: secret.Properties.ExpiresOn,
                Tags: tags));
        }

        return results;
    }

    public async Task SetSecretAsync(Uri vaultUri, SecretSnapshot secret, bool overwrite, CancellationToken cancellationToken = default)
    {
        if (!secret.Enabled)
            return;

        var client = new SecretClient(vaultUri, _credential);

        if (!overwrite)
        {
            try
            {
                _ = await client.GetSecretAsync(secret.Name, cancellationToken: cancellationToken);
                return;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
            }
        }

        if (secret.Value is null)
            throw new InvalidOperationException($"Secret '{secret.Name}' has no value.");

        var keyVaultSecret = new KeyVaultSecret(secret.Name, secret.Value);
        keyVaultSecret.Properties.Enabled = secret.Enabled;
        keyVaultSecret.Properties.NotBefore = secret.NotBefore;
        keyVaultSecret.Properties.ExpiresOn = secret.ExpiresOn;

        if (secret.Tags is { Count: > 0 })
        {
            foreach (var kvp in secret.Tags)
                keyVaultSecret.Properties.Tags[kvp.Key] = kvp.Value;
        }

        await client.SetSecretAsync(keyVaultSecret, cancellationToken);
    }
}
