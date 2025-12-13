using Azure.Core;
using KeyVaultTool.Auth;
using KeyVaultTool.Features.Secrets.Contracts;
using KeyVaultTool.Features.Secrets.Settings;
using KeyVaultTool.Infrastructure.KeyVault;
using KeyVaultTool.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Secrets.Commands;

public sealed class CompareSecretsCommand : AsyncCommand<CompareSecretsSettings>
{
    private readonly ICredentialFactory _credentialFactory;
    private readonly ISecretComparer _comparer;

    public CompareSecretsCommand(ICredentialFactory credentialFactory, ISecretComparer comparer)
    {
        _credentialFactory = credentialFactory;
        _comparer = comparer;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, CompareSecretsSettings settings, CancellationToken cancellationToken)
    {
        var authOptions = CommandHelpers.BuildAuthOptions(settings.AuthMode, settings.TenantId, settings.ClientId, settings.ClientSecret);

        TokenCredential credential;
        try { credential = _credentialFactory.Create(authOptions); }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create credential: {ex.Message}[/]");
            return -1;
        }

        var sourceUri = CommandHelpers.BuildVaultUri(settings.SourceVault);
        var targetUri = CommandHelpers.BuildVaultUri(settings.TargetVault);

        var secretService = new AzureKeyVaultSecretService(credential);

        if (settings.ShowValues)
            AnsiConsole.MarkupLine("[yellow]Warning: secret values will be printed to the console.[/]");

        try
        {
            var sourceSecrets = await secretService.GetSecretsAsync(sourceUri);
            var targetSecrets = await secretService.GetSecretsAsync(targetUri);

            var diff = _comparer.Compare(sourceSecrets, targetSecrets);

            var summary = new Table().AddColumn("Category").AddColumn("Count");
            summary.AddRow("Missing in target", diff.MissingInTarget.Count.ToString());
            summary.AddRow("Missing in source", diff.MissingInSource.Count.ToString());
            summary.AddRow("Value differences", diff.ValueDifferences.Count.ToString());

            AnsiConsole.MarkupLine("[bold]Summary[/]");
            AnsiConsole.Write(summary);

            if (diff.MissingInTarget.Count > 0)
            {
                var t = new Table().AddColumn("Missing in target");
                foreach (var s in diff.MissingInTarget) t.AddRow(s.Name);
                AnsiConsole.Write(t);
            }

            if (diff.MissingInSource.Count > 0)
            {
                var t = new Table().AddColumn("Missing in source");
                foreach (var s in diff.MissingInSource) t.AddRow(s.Name);
                AnsiConsole.Write(t);
            }

            if (diff.ValueDifferences.Count > 0)
            {
                var t = new Table().AddColumn("Name").AddColumn("Source").AddColumn("Target");
                foreach (var (src, tgt) in diff.ValueDifferences)
                {
                    t.AddRow(
                        src.Name,
                        settings.ShowValues ? (src.Value ?? string.Empty) : "********",
                        settings.ShowValues ? (tgt.Value ?? string.Empty) : "********");
                }

                AnsiConsole.MarkupLine("[bold]Value differences[/]");
                AnsiConsole.Write(t);
            }

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error comparing secrets: {ex.Message}[/]");
            return -1;
        }
    }
}
