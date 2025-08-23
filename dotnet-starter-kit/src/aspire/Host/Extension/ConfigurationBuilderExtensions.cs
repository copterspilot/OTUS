using FSH.Starter.Aspire.Models;
using Microsoft.Extensions.Configuration;

namespace FSH.Starter.Aspire.Extension;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddVaultConfiguration(
           this IConfigurationBuilder builder,
           FshVaultOptions options)
    {
        builder.Add(new VaultConfigurationSource(options));
        return builder;
    }
}
