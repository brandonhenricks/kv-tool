using Azure;
using Azure.Core;
using Azure.Security.KeyVault.Keys;
using KeyVaultTool.Features.Keys.Models;

namespace KeyVaultTool.Infrastructure.KeyVault;

public sealed class AzureKeyVaultKeyService : IKeyVaultKeyService
{
    private readonly TokenCredential _credential;

    public AzureKeyVaultKeyService(TokenCredential credential) => _credential = credential;

    public async Task<IReadOnlyList<KeySnapshot>> GetKeysAsync(Uri vaultUri, CancellationToken cancellationToken = default)
    {
        var client = new KeyClient(vaultUri, _credential);
        var results = new List<KeySnapshot>();

        await foreach (KeyProperties props in client.GetPropertiesOfKeysAsync(cancellationToken))
        {
            KeyVaultKey key = await client.GetKeyAsync(props.Name, cancellationToken: cancellationToken);
            var tags = key.Properties.Tags is { Count: > 0 }
                ? new Dictionary<string, string>(key.Properties.Tags)
                : null;

            results.Add(new KeySnapshot(
                Name: key.Name,
                Enabled: key.Properties.Enabled ?? true,
                KeyType: key.KeyType.ToString(),
                Tags: tags));
        }

        return results;
    }

    public async Task CopyKeyAsync(Uri sourceVault, Uri targetVault, string keyName, bool overwrite, CancellationToken cancellationToken = default)
    {
        var sourceClient = new KeyClient(sourceVault, _credential);
        var targetClient = new KeyClient(targetVault, _credential);

        KeyVaultKey sourceKey = await sourceClient.GetKeyAsync(keyName, cancellationToken: cancellationToken);

        if (sourceKey.Properties.Enabled is false)
            return;

        if (!overwrite)
        {
            try
            {
                _ = await targetClient.GetKeyAsync(keyName, cancellationToken: cancellationToken);
                return;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
            }
        }

        var options = new CreateKeyOptions
        {
            Enabled = sourceKey.Properties.Enabled,
            NotBefore = sourceKey.Properties.NotBefore,
            ExpiresOn = sourceKey.Properties.ExpiresOn
        };

        if (sourceKey.Properties.Tags is { Count: > 0 })
        {
            foreach (var kvp in sourceKey.Properties.Tags)
                options.Tags[kvp.Key] = kvp.Value;
        }

        await targetClient.CreateKeyAsync(sourceKey.Name, sourceKey.KeyType, options, cancellationToken);
    }
}
