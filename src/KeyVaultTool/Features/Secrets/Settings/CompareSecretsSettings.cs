using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Secrets.Settings;

public sealed class CompareSecretsSettings : CommandSettings
{
    [CommandOption("--source-vault <NAME_OR_URI>")]
    public string SourceVault { get; init; } = default!;

    [CommandOption("--target-vault <NAME_OR_URI>")]
    public string TargetVault { get; init; } = default!;

    [CommandOption("--auth <cli|devicecode|sp>")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public FlagValue<string> AuthMode { get; init; } = new();
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [CommandOption("--tenant-id <TENANT>")]
    public string? TenantId { get; init; }

    [CommandOption("--client-id <CLIENT>")]
    public string? ClientId { get; init; }

    [CommandOption("--client-secret <SECRET>")]
    public string? ClientSecret { get; init; }

    [CommandOption("--show-values")]
    public bool ShowValues { get; init; }
}
