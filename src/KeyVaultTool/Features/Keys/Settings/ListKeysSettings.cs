using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Keys.Settings;

public sealed class ListKeysSettings : CommandSettings
{
    [CommandOption("--vault <NAME_OR_URI>")]
    public string Vault { get; init; } = default!;

    [CommandOption("--auth [cli|devicecode|sp]")]
    public string AuthMode { get; init; } = "cli";

    [CommandOption("--tenant-id <TENANT>")]
    public string? TenantId { get; init; }

    [CommandOption("--client-id <CLIENT>")]
    public string? ClientId { get; init; }

    [CommandOption("--client-secret <SECRET>")]
    public string? ClientSecret { get; init; }
}
