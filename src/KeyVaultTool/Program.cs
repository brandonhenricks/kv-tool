using KeyVaultTool;
using KeyVaultTool.Auth;
using KeyVaultTool.Commands;
using KeyVaultTool.Features.Keys.Commands;
using KeyVaultTool.Features.Keys.Contracts;
using KeyVaultTool.Features.Keys.Services;
using KeyVaultTool.Features.Secrets.Commands;
using KeyVaultTool.Features.Secrets.Contracts;
using KeyVaultTool.Features.Secrets.Services;
using KeyVaultTool.Infrastructure.KeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
        options.SingleLine = true;
        options.TimestampFormat = "[HH:mm:ss] ";
    });
});

services.AddSingleton<ICredentialFactory, DefaultCredentialFactory>();
services.AddSingleton<ISecretComparer, SecretComparer>();
services.AddSingleton<IKeyComparer, KeyComparer>();
services.AddSingleton<IKeyVaultKeyServiceFactory, KeyVaultKeyServiceFactory>();
services.AddSingleton<IKeyVaultSecretServiceFactory, KeyVaultSecretServiceFactory>();
services.AddSingleton<IKeySyncServiceFactory, KeySyncServiceFactory>();
services.AddSingleton<ISecretSyncServiceFactory, SecretSyncServiceFactory>();

var commandRegistry = new CommandRegistry();
services.AddSingleton(commandRegistry);

var registrar = new TypeRegistrar(services);
var app = new CommandApp<RootCommand>(registrar);

app.Configure(config =>
{
    config.SetApplicationName("kvtool");

    RegisterCommand<HelpCommand>(config, commandRegistry.HelpCommand);
    RegisterCommand<ListSecretsCommand>(config, CommandRegistry.GetDescriptor<ListSecretsCommand>());
    RegisterCommand<ListKeysCommand>(config, CommandRegistry.GetDescriptor<ListKeysCommand>());
    RegisterCommand<CompareSecretsCommand>(config, CommandRegistry.GetDescriptor<CompareSecretsCommand>());
    RegisterCommand<CompareKeysCommand>(config, CommandRegistry.GetDescriptor<CompareKeysCommand>());
    RegisterCommand<SyncSecretsCommand>(config, CommandRegistry.GetDescriptor<SyncSecretsCommand>());
    RegisterCommand<SyncKeysCommand>(config, CommandRegistry.GetDescriptor<SyncKeysCommand>());
});

static void RegisterCommand<TCommand>(IConfigurator config, CommandDescriptor descriptor)
    where TCommand : class, ICommand
{
    config.AddCommand<TCommand>(descriptor.Name)
        .WithDescription(descriptor.Description);
}

return await app.RunAsync(args);
