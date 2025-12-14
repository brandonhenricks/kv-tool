# AGENTS.md — kvtool (C# Key Vault CLI)

## Goal
kvtool is a .NET CLI for working with Azure Key Vault across resource groups/subscriptions.
Optimize for correctness, safety (no secrets leakage), and maintainability.

## Tech stack

- Language: C#
- Runtime: .NET (use global.json if present; otherwise target latest LTS used by the repo)
- CLI framework: (Spectre.Console.Cli or System.CommandLine) — follow existing patterns in this repo
- Auth: Azure Identity (DefaultAzureCredential + device code / az login flows as implemented)

## Commands to run (local)

Prefer the repo-defined commands first; if missing, use the defaults below.

### Restore / build

- `dotnet restore`
- `dotnet build -c Release`

### Test

- `dotnet test -c Release`

### Format / lint (only if configured)

- `dotnet format` (only if dotnet-format is installed/configured)
- If analyzers exist, ensure build is clean (treat warnings as errors if repo enforces it)

## Project structure (expected)

- `src/` contains production code
- `tests/` contains unit/integration tests
- Keep CLI parsing/commands thin; push logic into application services.
- Avoid “god” helper classes; prefer cohesive services + small focused utilities.

## Architecture & design rules

- Follow Clean Architecture boundaries if present:
  - CLI layer: argument parsing, console IO, exit codes
  - Application layer: orchestration, use-cases, policies
  - Infrastructure layer: Azure SDK clients, Key Vault access, filesystem, process/az cli integration
- Keep domain logic testable:
  - Wrap Azure SDK calls behind interfaces (e.g., IKeyVaultClient, IBlobClientFactory, IProcessRunner)
  - Prefer dependency injection over statics/singletons.
- Prefer async APIs end-to-end where IO is involved.
- Follow Best Practices for OOP.
- Adhere to KISS, DRY, and SOLID practices.

## Coding standards

- Use nullable reference types if enabled.
- Prefer explicit cancellation tokens for network calls.
- Do not swallow exceptions; map to meaningful CLI errors + non-zero exit codes.
- Add XML docs only for public APIs used by other assemblies; keep internal code self-explanatory.

## Testing guidance

- Add/extend unit tests for:
  - command routing/validation
  - overwrite/skip rules
  - filtering behavior (e.g., “do not copy disabled secrets”)
- For integration tests:
  - Do not hit real Azure unless the repo already has a sanctioned approach.
  - Prefer recorded/mocked clients where possible.

## Git / PR workflow

- Keep changes in small vertical slices (command + handler + tests).
- Write commit messages in imperative mood.
- Do not modify versioning/release files unless the task explicitly requires it.

## Boundaries / do-not-touch

- Never commit secrets, connection strings, tenant IDs, or private keys.
- Do not add new external dependencies without strong justification; prefer existing packages.
- Do not change CI/CD or packaging (Homebrew/Chocolatey/GitHub Actions/Azure DevOps) unless requested.
- Avoid editing generated files, vendor directories, or lockfiles unless necessary.

## Security & safety

- Treat Key Vault data as sensitive even if “not secret” (names and metadata can be sensitive).
- Redact secret values in logs and console output by default.
- Ensure overwrite/copy actions require explicit flags and have safe defaults.

## Operational notes

- If the task involves Azure auth:
  - Use existing credential chain behavior in the repo.
  - Prefer DefaultAzureCredential unless the command explicitly requests another flow.
- If agent execution is available:
  - Run tests relevant to touched areas before finishing.
  - If tests fail, fix them (don’t disable them).

## If instructions conflict

- Follow the nearest AGENTS.md in the directory tree.
- User instructions in chat override repository guidance.

# ABSOLUTE MANDATORY RULES

- You must review these instructions in full before executing any steps to understand the full instructions guidelines
- You must follow these instructions exactly as specified without deviation
- Do not keep repeating status updates while processing or explanations unless explicitly required
- NO verbose explanations or commentary
- NO comments should be generated in code unless asked
- When creating new features, first start with analyzing the existing codebase and create an implementation plan stored as a markdown file in the repository root.