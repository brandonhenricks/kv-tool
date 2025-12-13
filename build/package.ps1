param(
  [string]$Version = $env:KVTOOL_VERSION,
  [string]$Configuration = 'Release'
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir '..')
Set-Location $repoRoot

if (-not $Version) {
  Write-Error 'Version must be provided via KVTOOL_VERSION or the -Version parameter.'
  exit 1
}

$publishBase = Join-Path $repoRoot 'dist'
$artifactRoot = Join-Path $publishBase $Version

Write-Host "Packaging kvtool $Version..."

if (Test-Path $artifactRoot) {
  Remove-Item $artifactRoot -Recurse -Force
}

$rids = @('win-x64', 'linux-x64', 'osx-arm64')
if ($env:KVTOOL_RIDS) {
  $rids = $env:KVTOOL_RIDS -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
}
$publishArgs = @(
  '-c', $Configuration,
  '-p:PublishSingleFile=true',
  '-p:SelfContained=true',
  '-p:IncludeNativeLibrariesForSelfExtract=true',
  '-p:DebugType=None',
  '-p:DebugSymbols=false'
)

foreach ($rid in $rids) {
  $publishDir = Join-Path $artifactRoot $rid
  $publishOutput = Join-Path $publishDir 'publish'
  if (Test-Path $publishDir) {
    Remove-Item $publishDir -Recurse -Force
  }
  New-Item $publishOutput -ItemType Directory -Force | Out-Null

  dotnet publish src/KeyVaultTool -r $rid @publishArgs -o $publishOutput

  $artifactDir = $publishDir
  $artifactName = "kvtool-$Version-$rid"
  switch ($rid) {
    'win-x64' {
      $exePath = Join-Path $publishOutput 'kvtool.exe'
      $zipPath = Join-Path $artifactRoot "$artifactName.zip"
      Compress-Archive -Path $exePath -DestinationPath $zipPath -Force
      break
    }
    default {
      $binName = 'kvtool'
      $packageName = "$artifactName.tar.gz"
      $tarPath = Join-Path $artifactRoot $packageName
      if (Test-Path $tarPath) { Remove-Item $tarPath }
      & tar -czf $tarPath -C $publishOutput $binName
      break
    }
  }
}

$artifactFiles = Get-ChildItem $artifactRoot -File | Where-Object { $_.Extension -in '.zip', '.gz' }
$checksums = @()
foreach ($file in $artifactFiles) {
  $hash = Get-FileHash $file.FullName -Algorithm SHA256
  $relativePath = $file.FullName.Substring($artifactRoot.Length + 1) -replace '\\', '/'
  $checksums += "$($hash.Hash)  $relativePath"
}

$checksumFile = Join-Path $artifactRoot 'checksums.txt'
$checksums | Set-Content -Path $checksumFile -Encoding UTF8

Write-Host 'Packaging complete.'