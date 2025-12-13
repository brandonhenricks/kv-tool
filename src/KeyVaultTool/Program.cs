using KeyVaultTool;
using KeyVaultTool.Auth;
using KeyVaultTool.Commands;
using KeyVaultTool.Features.Keys.Commands;
using KeyVaultTool.Features.Keys.Contracts;
using KeyVaultTool.Features.Keys.Services;
using KeyVaultTool.Features.Secrets.Commands;
using KeyVaultTool.Features.Secrets.Contracts;
using KeyVaultTool.Features.Secrets.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var services = new ServiceCollection();

services.AddSingleton<ICredentialFactory, DefaultCredentialFactory>();
services.AddSingleton<ISecretComparer, SecretComparer>();
services.AddSingleton<IKeyComparer, KeyComparer>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp<RootCommand>(registrar);

app.Configure(config =>
{
    config.SetApplicationName("kvtool");

    config.AddCommand<ListSecretsCommand>("list-secrets")
        .WithDescription("List secrets in a Key Vault.");

    config.AddCommand<ListKeysCommand>("list-keys")
        .WithDescription("List keys in a Key Vault.");

    config.AddCommand<CompareSecretsCommand>("compare-secrets")
        .WithDescription("Compare secrets between two Key Vaults.");

    config.AddCommand<CompareKeysCommand>("compare-keys")
        .WithDescription("Compare keys between two Key Vaults.");

    config.AddCommand<SyncSecretsCommand>("sync-secrets")
        .WithDescription("Sync secrets from a source Key Vault to a target Key Vault.");

    config.AddCommand<SyncKeysCommand>("sync-keys")
        .WithDescription("Sync keys from a source Key Vault to a target Key Vault.");
});

return app.Run(args);
