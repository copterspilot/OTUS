using FSH.Starter.Aspire.Models;
using Microsoft.Extensions.Configuration;

namespace FSH.Starter.Aspire.Extension;

public class VaultConfigurationSource : IConfigurationSource
{
    private readonly FshVaultOptions _options;

    public VaultConfigurationSource(FshVaultOptions options)
    {
        _options = options;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(_options);
    }
}
