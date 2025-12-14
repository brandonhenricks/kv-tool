using KeyVaultTool.Features.Keys.Commands;
using KeyVaultTool.Features.Secrets.Commands;

namespace KeyVaultTool.Commands;

/// <summary>
/// Describes a CLI command, including its name, description, and implementing type.
/// </summary>
/// <param name="Name">The unique name of the command as used in the CLI.</param>
/// <param name="Description">A brief description of the command's purpose.</param>
/// <param name="CommandType">The <see cref="Type"/> that implements the command logic.</param>
public sealed record CommandDescriptor(string Name, string Description, Type CommandType);

/// <summary>
/// Represents a registry for available CLI commands, providing access to command descriptors and lookup utilities.
/// </summary>
public sealed class CommandRegistry
{
    /// <summary>
    /// Backing field for the list of registered command descriptors.
    /// </summary>
    private static readonly IReadOnlyList<CommandDescriptor> s_commands = new[]
    {
        new CommandDescriptor("list-secrets", "List secrets in a Key Vault.", typeof(ListSecretsCommand)),
        new CommandDescriptor("list-keys", "List keys in a Key Vault.", typeof(ListKeysCommand)),
        new CommandDescriptor("compare-secrets", "Compare secrets between two Key Vaults.", typeof(CompareSecretsCommand)),
        new CommandDescriptor("compare-keys", "Compare keys between two Key Vaults.", typeof(CompareKeysCommand)),
        new CommandDescriptor("sync-secrets", "Sync secrets from a source Key Vault to a target Key Vault.", typeof(SyncSecretsCommand)),
        new CommandDescriptor("sync-keys", "Sync keys from a source Key Vault to a target Key Vault.", typeof(SyncKeysCommand)),
    };

    /// <summary>
    /// Gets the list of registered command descriptors.
    /// </summary>
    public static IReadOnlyList<CommandDescriptor> Commands => s_commands;

    /// <summary>
    /// Gets the descriptor for the help command.
    /// </summary>
    public CommandDescriptor HelpCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandRegistry"/> class,
    /// setting up the help command descriptor.
    /// </summary>
    public CommandRegistry() =>
        HelpCommand = new CommandDescriptor("help", "Show available commands and their descriptions.", typeof(HelpCommand));

    /// <summary>
    /// Retrieves the <see cref="CommandDescriptor"/> for the specified command type.
    /// </summary>
    /// <param name="commandType">The type of the command.</param>
    /// <returns>The matching <see cref="CommandDescriptor"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no matching command descriptor is found.</exception>
    public static CommandDescriptor GetDescriptor(Type commandType) =>
        Commands.Single(descriptor => descriptor.CommandType == commandType);

    /// <summary>
    /// Retrieves the <see cref="CommandDescriptor"/> for the specified command type parameter.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <returns>The matching <see cref="CommandDescriptor"/>.</returns>
    public static CommandDescriptor GetDescriptor<TCommand>() => GetDescriptor(typeof(TCommand));
}
