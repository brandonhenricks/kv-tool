using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Secrets.Settings;

public sealed class ListSecretsSettings : CommandSettings
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

    [CommandOption("--show-values")]
    public bool ShowValues { get; init; }
}
