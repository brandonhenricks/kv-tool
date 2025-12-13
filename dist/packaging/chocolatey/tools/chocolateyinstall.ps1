$packageName = 'kvtool'
$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$url = 'https://github.com/YOUR_ORG/kv-tool/releases/download/v0.1.0/kvtool-0.1.0-win-x64.zip'
$checksum = '<SHA256_FOR_WIN_X64>'

Install-ChocolateyZipPackage -PackageName $packageName \
  -ZipFileFullPath "$toolsDir\kvtool.zip" \
  -Destination "$toolsDir" \
  -Url $url \
  -Checksum $checksum \
  -ChecksumType 'sha256'