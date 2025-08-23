using FSH.Starter.Aspire.Models;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace FSH.Starter.Aspire.Extension;

public class VaultConfigurationProvider(FshVaultOptions options) : ConfigurationProvider
{
    private readonly FshVaultOptions _options = options;

    public override void Load()
    {
        try
        {
            var authMethod = new TokenAuthMethodInfo(_options.Token);
            var vaultClientSettings = new VaultClientSettings(_options.Address, authMethod);
            var vaultClient = new VaultClient(vaultClientSettings);

            var secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(_options.SecretPath).Result;

            // Сохраняем значения в конфигурацию
            Data["pg-username"] = secret.Data.Data[_options.UsernameKey]?.ToString() ?? string.Empty;
            Data["pg-password"] = secret.Data.Data[_options.PasswordKey]?.ToString() ?? string.Empty;

            // Также можно сохранить как ConnectionString
            Data["ConnectionStrings:PostgreSQL"] = $"Host=localhost;Database=fullstackhero;Username={Data["pg-username"]};Password={Data["pg-password"]}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка загрузки конфигурации из Vault: {ex.Message}", ex);
        }
    }
}
