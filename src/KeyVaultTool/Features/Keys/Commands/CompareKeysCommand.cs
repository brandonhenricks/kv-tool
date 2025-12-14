using Azure.Core;
using KeyVaultTool.Auth;
using KeyVaultTool.Features.Keys.Contracts;
using KeyVaultTool.Features.Keys.Settings;
using KeyVaultTool.Infrastructure.KeyVault;
using KeyVaultTool.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KeyVaultTool.Features.Keys.Commands;

public sealed class CompareKeysCommand : AsyncCommand<CompareKeysSettings>
{
    private readonly ICredentialFactory _credentialFactory;
    private readonly IKeyComparer _comparer;

    public CompareKeysCommand(ICredentialFactory credentialFactory, IKeyComparer comparer)
    {
        _credentialFactory = credentialFactory;
        _comparer = comparer;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, CompareKeysSettings settings, CancellationToken cancellationToken)
    {
        var authOptions = CommandHelpers.BuildAuthOptions(settings.AuthMode.Value, settings.TenantId, settings.ClientId, settings.ClientSecret);

        TokenCredential credential;
        try
        {
            credential = _credentialFactory.Create(authOptions);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create credential: {ex.Message}[/]");
            return -1;
        }

        var sourceUri = CommandHelpers.BuildVaultUri(settings.SourceVault);
        var targetUri = CommandHelpers.BuildVaultUri(settings.TargetVault);

        var keyService = new AzureKeyVaultKeyService(credential);

        try
        {
            var sourceKeys = await keyService.GetKeysAsync(sourceUri, cancellationToken);
            var targetKeys = await keyService.GetKeysAsync(targetUri, cancellationToken);

            var diff = _comparer.Compare(sourceKeys, targetKeys);

            var summary = new Table().AddColumn("Category").AddColumn("Count");
            summary.AddRow("Missing in target", diff.MissingInTarget.Count.ToString());
            summary.AddRow("Missing in source", diff.MissingInSource.Count.ToString());
            summary.AddRow("Attribute differences", diff.AttributeDifferences.Count.ToString());

            AnsiConsole.MarkupLine("[bold]Summary[/]");
            AnsiConsole.Write(summary);

            if (diff.MissingInTarget.Count > 0)
            {
                var missing = new Table().AddColumn("Missing in target");
                foreach (var key in diff.MissingInTarget)
                    missing.AddRow(key.Name);

                AnsiConsole.Write(missing);
            }

            if (diff.MissingInSource.Count > 0)
            {
                var missing = new Table().AddColumn("Missing in source");
                foreach (var key in diff.MissingInSource)
                    missing.AddRow(key.Name);

                AnsiConsole.Write(missing);
            }

            if (diff.AttributeDifferences.Count > 0)
            {
                var differences = new Table()
                    .AddColumn("Name")
                    .AddColumn("Source Type")
                    .AddColumn("Target Type");

                foreach (var (src, tgt) in diff.AttributeDifferences)
                {
                    differences.AddRow(
                        src.Name,
                        src.KeyType,
                        tgt.KeyType);
                }

                AnsiConsole.MarkupLine("[bold]Attribute differences[/]");
                AnsiConsole.Write(differences);
            }

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error comparing keys: {ex.Message}[/]");
            return -1;
        }
    }
}
