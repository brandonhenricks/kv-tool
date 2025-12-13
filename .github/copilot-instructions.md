# GitHub Copilot Instructions for kvtool

## High-level orientation
- kvtool is a cross-platform Spectre.Console CLI focused on listing, comparing, and syncing Azure Key Vault secrets and keys (see README.md for the mission statement).
- The `Program.cs` bootstraps `Spectre.Console.Cli` with a `ServiceCollection`/`TypeRegistrar`, registers comparers and credential logic, and wires up the six top-level commands under `Commands`.
- Command implementations live under `src/KeyVaultTool/Features/{Keys,Secrets}`, each subfolder exposing `Commands`, `Settings`, `Services`, `Contracts`, and `Models` to keep the CLI/service boundary clear.

## Command flow & conventions
- Each `AsyncCommand<TSettings>` obtains CLI options via Spectre.Console `CommandOption` attributes (examples: `--source-vault`, `--target-vault`, `--auth`, `--allow-overwrite`, `--yes` in `Features/Keys/Settings/SyncKeysSettings.cs`).
- Shared helpers in `Shared/CommandHelpers.cs` normalize vault URIs and auth mode strings before creating `AuthOptions`, so follow those helpers instead of re-parsing strings manually.
- Commands always build a `TokenCredential` through `DefaultCredentialFactory` (Auth/DefaultCredentialFactory.cs) and then instantiate the appropriate Azure Key Vault client service (`Infrastructure/KeyVault/AzureKeyVault{Secret,Key}Service.cs`).
- Messaging uses `Spectre.Console.Status`, `AnsiConsole.MarkupLine`, and explicit cancellation tokens (see `Features/Keys/Commands/SyncKeysCommand.cs`); mirror that pattern when adding new commands.
- Comparison logic uses comparer services like `KeyComparer` and `SecretComparer` to produce `DiffResult` models; sync services consume those diffs to copy or skip entries and honor `--allow-overwrite`.

## Integration & data flow notes
- `Infrastructure/KeyVault` services wrap Azure SDK clients and always skip disabled items (they check `Properties.Enabled == false` before copying).
- Sync paths call `KeySyncService` or `SecretSyncService`, which fetch both vault snapshots and only copy missing or changed items (`MissingInTarget`, `AttributeDifferences`, etc.).
- For auth, respect `AuthMode`: `cli` uses `DefaultAzureCredential`, `devicecode` writes the device code callback to console, and `sp` requires tenant/client/secret in AuthOptions before creating `ClientSecretCredential`.

## Build, packaging, and release workflows
- Local builds target .NET SDK 10; use the scripts in `build/` to produce consistent release artifacts: set `KVTOOL_VERSION` and run `./build/package.sh` (Linux/macOS) or `pwsh ./build/package.ps1` (Windows).  `KVTOOL_RIDS` can trim the published runtime list.
- Scripts create `dist/<version>/` archives with checksums; keep those artifacts aligned with the templates under `dist/packaging/{homebrew,chocolatey,winget}`.
- Release automation comes from `.github/workflows/ci.yml` (pull-request validation) and `.github/workflows/release.yml` (tag-triggered multi-RID builds that publish zips/tars and checksums).
- When bumping releases, update the packaged formula/manifest placeholders (Homebrew `dist/packaging/homebrew/kvtool.rb`, Chocolatey nuspec, WinGet yaml) with the new version/SHA256.
- The README notes the project no longer uses `<PackAsTool>`; keep tool-packing tags removed from `KeyVaultTool.csproj` if you touch the csproj.

## Testing & quick checks
- The only automated unit test today is `tests/KeyVaultTool.Tests/TypeRegistrarTests.cs`; run `dotnet test` at the root whenever you touch dependency wiring or types used by tests.
- CI runs restore/build and optional tests via its workflows, so mimic `dotnet restore` + `dotnet build/publish` locally before pushing.

## Feedback loop
- After editing or adding commands, scan the commands listed in `Program.cs` to ensure the Spectre.Console registration stays in sync and the description strings remain accurate.
- If you add Vault interactions, reuse `CommandHelpers` and the Azure service wrappers so telemetry, messaging, and exception handling stay uniform.
- Let me know if any architectural or workflow detail feels unclear so I can expand or correct these instructions.
