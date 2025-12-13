#!/usr/bin/env bash
set -euo pipefail

VERSION=${KVTOOL_VERSION:-}
CONFIGURATION=${CONFIGURATION:-Release}

if [ -z "$VERSION" ]; then
  echo "KVTOOL_VERSION must be set when running package.sh"
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
DIST_ROOT="$REPO_ROOT/dist"
ARTIFACT_ROOT="$DIST_ROOT/$VERSION"

echo "Packaging kvtool $VERSION..."

rids=(win-x64 linux-x64 osx-arm64)
if [ -n "${KVTOOL_RIDS:-}" ]; then
  IFS=',' read -r -a rids <<< "$KVTOOL_RIDS"
  for i in "${!rids[@]}"; do
    rids[$i]="$(echo "${rids[$i]}" | xargs)"
  done
fi
publish_options=(
  -c "$CONFIGURATION"
  -p:PublishSingleFile=true
  -p:SelfContained=true
  -p:IncludeNativeLibrariesForSelfExtract=true
  -p:DebugType=None
  -p:DebugSymbols=false
)

rm -rf "$ARTIFACT_ROOT"
mkdir -p "$ARTIFACT_ROOT"

for rid in "${rids[@]}"; do
  publish_dir="$ARTIFACT_ROOT/$rid/publish"
  mkdir -p "$publish_dir"

  dotnet publish src/KeyVaultTool -r "$rid" "${publish_options[@]}" -o "$publish_dir"

  artifact_name="kvtool-$VERSION-$rid"
  case "$rid" in
    win-x64)
      exe_path="$publish_dir/kvtool.exe"
      zip_path="$ARTIFACT_ROOT/$artifact_name.zip"
      rm -f "$zip_path"
      zip -j "$zip_path" "$exe_path"
      ;;
    *)
      bin_path="$publish_dir/kvtool"
      archive_path="$ARTIFACT_ROOT/$artifact_name.tar.gz"
      rm -f "$archive_path"
      tar -czf "$archive_path" -C "$publish_dir" "kvtool"
      ;;
  esac
done

checksum_file="$ARTIFACT_ROOT/checksums.txt"
rm -f "$checksum_file"

hash_cmd(){
  if command -v sha256sum >/dev/null 2>&1; then
    sha256sum "$1" | awk '{print $1}'
  else
    shasum -a 256 "$1" | awk '{print $1}'
  fi
}

find "$ARTIFACT_ROOT" -maxdepth 1 -type f \( -name '*.zip' -o -name '*.tar.gz' \) -print0 |
  while IFS= read -r -d '' artifact; do
    sha=$(hash_cmd "$artifact")
    rel=$(basename "$artifact")
    echo "$sha  $rel" >> "$checksum_file"
  done

echo "Packaging complete."