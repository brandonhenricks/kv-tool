class Kvtool < Formula
  desc 'CLI for listing, comparing, and syncing Azure Key Vault secrets and keys'
  homepage 'https://github.com/YOUR_ORG/kv-tool'
  url 'https://github.com/YOUR_ORG/kv-tool/releases/download/v0.1.0/kvtool-linux-x64.tar.gz'
  sha256 '<LINUX_SHA256>'
  license 'MIT'

  on_macos do
    if Hardware::CPU.arm?
      url 'https://github.com/YOUR_ORG/kv-tool/releases/download/v0.1.0/kvtool-osx-arm64.tar.gz'
      sha256 '<OSX_ARM64_SHA256>'
    end
  end

  def install
    bin.install 'kvtool'
  end
end