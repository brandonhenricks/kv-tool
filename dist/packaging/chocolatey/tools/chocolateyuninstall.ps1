$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Remove-Item "$toolsDir\kvtool.exe" -Force -ErrorAction SilentlyContinue
Remove-Item "$toolsDir\kvtool.zip" -Force -ErrorAction SilentlyContinue