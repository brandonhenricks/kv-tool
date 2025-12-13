using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Keys.Settings;

public sealed class CompareKeysSettings : CommandSettings
{
    [CommandOption("--source-vault <NAME_OR_URI>")]
    public string SourceVault { get; init; } = default!;

    [CommandOption("--target-vault <NAME_OR_URI>")]
    public string TargetVault { get; init; } = default!;

    [CommandOption("--auth [cli|devicecode|sp]")]
    public string AuthMode { get; init; } = "cli";

    [CommandOption("--tenant-id <TENANT>")]
    public string? TenantId { get; init; }

    [CommandOption("--client-id <CLIENT>")]
    public string? ClientId { get; init; }

    [CommandOption("--client-secret <SECRET>")]
    public string? ClientSecret { get; init; }
}
