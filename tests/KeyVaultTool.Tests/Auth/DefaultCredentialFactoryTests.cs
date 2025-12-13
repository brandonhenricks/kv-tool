using System;
using Azure.Core;
using Azure.Identity;
using KeyVaultTool.Auth;
using Xunit;

namespace KeyVaultTool.Tests.Auth;

public class DefaultCredentialFactoryTests
{
    [Fact]
    public void Create_ReturnsDefaultAzureCredential_WhenModeIsCli()
    {
        var factory = new DefaultCredentialFactory();
        var options = new AuthOptions { Mode = AuthMode.Cli };

        var credential = factory.Create(options);

        Assert.IsType<DefaultAzureCredential>(credential);
    }

    [Fact]
    public void Create_ReturnsDeviceCodeCredential_WhenModeIsDeviceCode()
    {
        var factory = new DefaultCredentialFactory();
        var options = new AuthOptions { Mode = AuthMode.DeviceCode };

        var credential = factory.Create(options);

        Assert.IsType<DeviceCodeCredential>(credential);
    }

    [Fact]
    public void Create_ReturnsClientSecretCredential_WhenModeIsServicePrincipal_AndOptionsValid()
    {
        var factory = new DefaultCredentialFactory();
        var options = new AuthOptions
        {
            Mode = AuthMode.ServicePrincipal,
            TenantId = "tenant",
            ClientId = "client",
            ClientSecret = "secret"
        };

        var credential = factory.Create(options);

        Assert.IsType<ClientSecretCredential>(credential);
    }

    [Fact]
    public void Create_ThrowsInvalidOperationException_WhenServicePrincipalOptionsMissing()
    {
        var factory = new DefaultCredentialFactory();
        var options = new AuthOptions
        {
            Mode = AuthMode.ServicePrincipal,
            TenantId = null,
            ClientId = null,
            ClientSecret = null
        };

        Assert.Throws<InvalidOperationException>(() => factory.Create(options));
    }

    [Fact]
    public void Create_ThrowsNotSupportedException_WhenModeIsUnknown()
    {
        var factory = new DefaultCredentialFactory();
        var options = new AuthOptions { Mode = (AuthMode)999 };

        Assert.Throws<NotSupportedException>(() => factory.Create(options));
    }
}
