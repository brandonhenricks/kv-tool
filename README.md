# kvtool (CLI)

A cross-platform CLI for listing, comparing, and syncing Azure Key Vault **Secrets** and **Keys**.

## Scope

- ✅ Secrets + Keys
- ❌ Certificates
- ✅ Auth modes: `cli` (default), `devicecode`, `sp`
- ✅ Overwrite behavior is opt-in (`--allow-overwrite`)
- ✅ Disabled secrets/keys are **never copied**
- ⚠️ Key “copy” recreates a key with same name/type/attributes; it does **not** preserve key material.

---

## Build & Run (local)

### Prereqs
- .NET SDK 10
## Publish as a CLI binary (recommended)

This project now ships **kvtool** as a native CLI binary instead of a .NET tool.

### Build locally

Use `dotnet publish` with the `build/package` helpers so the artifacts are packaged consistently and include SHA256 checksums.

```bash
KVTOOL_VERSION=0.1.0 ./build/package.sh  # Linux/macOS
KVTOOL_VERSION=0.1.0 pwsh ./build/package.ps1  # Windows PowerShell
```

By default the scripts package every supported runtime identifier (`win-x64`, `linux-x64`, `osx-arm64`).
Set `KVTOOL_RIDS` if you only need a subset (for example, an OS-specific CI job).

Output files land under `dist/<version>/` and include the platform archives plus `checksums.txt` for every release.

### Release automation

GitHub Actions runs the pipelines defined in `.github/workflows`:

- `ci.yml` validates pull requests and pushes to `main` (restore/build + optional tests).
- `release.yml` triggers on `v*.*.*` tags, builds each RID on its matching runner, uploads the artifacts, and creates a GitHub Release titled `kvtool <version>` with the zip/tar and `checksums.txt` attached.

Use those artifacts when updating downstream distribution feeds.

## Distribution / Installation Options

### GitHub Releases (baseline)

Each release publishes:
* `kvtool-<version>-win-x64.zip`
* `kvtool-<version>-linux-x64.tar.gz`
* `kvtool-<version>-osx-arm64.tar.gz`
* `checksums.txt`

Download the archive for your platform and drop it on `PATH`.

### Homebrew

Reference the template at `dist/packaging/homebrew/kvtool.rb`. Copy it into your tap, replace the placeholders (`YOUR_ORG`, `SHA256`), and point to the GitHub release tarballs.

### Chocolatey

The `dist/packaging/chocolatey` directory contains a `.nuspec` and the necessary `tools/chocolateyinstall.ps1`/`chocolateyuninstall.ps1` scripts. The install script downloads the Windows zip, validates its SHA256, and extracts `kvtool.exe` into `$toolsDir`.

### WinGet

Use `dist/packaging/winget/kvtool.yaml` as a manifest skeleton. Update the `InstallerUrl`, `InstallerSha256`, and `Version` fields to point at your release assets before submitting to the winget-pkgs repo or another Windows package source.

### Scoop (optional)

If your audience prefers Scoop, host a custom bucket and point it at the GitHub release zip, directing the user to fetch the `kvtool.exe`.

## Security

See [SECURITY.md](SECURITY.md) for guidance about avoiding `--show-values` in shared logs and following least-privilege Key Vault access patterns.
1. Create WinGet manifests pointing to your installer/zip on GitHub Releases.
2. Submit to the `microsoft/winget-pkgs` repo (public) or your enterprise/private WinGet source.

**User install**
```powershell
winget install YourOrg.KvTool
```

WinGet is excellent for enterprise distribution when paired with Intune / endpoint tooling.

---

### Option E: Scoop (Windows, developer-friendly)
If your audience is developer-heavy, Scoop is a nice lightweight alternative.

**User install**
```powershell
scoop bucket add your-org https://github.com/your-org/scoop-bucket
scoop install kvtool
```

---

## Recommended Release Pipeline (practical)

1. `dotnet test` (if/when tests exist)
2. `dotnet publish` for:
   - `win-x64`
   - `linux-x64`
   - `osx-arm64`
3. Package each publish folder into:
   - `kvtool-win-x64.zip`
   - `kvtool-linux-x64.tar.gz`
   - `kvtool-osx-arm64.tar.gz`
4. Create GitHub Release with artifacts + checksums
5. Update:
   - Homebrew formula SHA/version
   - Chocolatey nuspec + checksum
   - WinGet manifests (optional)

---

## Security Notes

- Avoid `--show-values` in shared terminals/logged sessions.
- Prefer `az login` and least-privilege RBAC on Key Vault.
- Consider adding `--output json` for automation scenarios (future enhancement).
