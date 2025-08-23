using FSH.Starter.Aspire.Extension;
using FSH.Starter.Aspire.Models;
using VaultSharp.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("grafana", "grafana/grafana")
       .WithBindMount("../../../compose/grafana/config", "/etc/grafana", isReadOnly: true)
       .WithBindMount("../../../compose/grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
       .WithHttpEndpoint(port: 3000, targetPort: 3000, name: "http");

builder.AddContainer("prometheus", "prom/prometheus")
       .WithBindMount("../../../compose/prometheus", "/etc/prometheus", isReadOnly: true)
       .WithHttpEndpoint(port: 9090, targetPort: 9090);

var cache = builder.AddValkey("cache")
    .WithDataVolume(isReadOnly: false)
    .WithPersistence(interval: TimeSpan.FromMinutes(5), keysChangedThreshold: 100);

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithDataVolume();

var vault = builder.AddContainer("vault", "hashicorp/vault", "latest")
    .WithHttpEndpoint(8200, 8200)
    .WithEnvironment("VAULT_DEV_ROOT_TOKEN_ID", "dev-only-token")
    .WithEnvironment("VAULT_DEV_LISTEN_ADDRESS", "0.0.0.0:8200")
    .WithVolume("vault-data", "/vault/data");

// Настройки Vault
var vaultOptions = new FshVaultOptions
{
    Address = "http://localhost:8200",
    Token = "dev-only-token",
    SecretPath = "secret/data/postgres"
};

// Добавляем Vault в конфигурацию
//builder.Configuration.AddVaultConfiguration(vaultOptions);

var username = builder.AddParameter("pg-username", "postgres");
var password = builder.AddParameter("pg-password", "postgres");

var postgres = builder.AddPostgres("fullstackhero", username, password, port: 5432)
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("fsh");

var api = builder.AddProject<Projects.Server>("webapi")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(keycloak)
    .WaitFor(keycloak);

var blazor = builder.AddProject<Projects.Client>("blazor")
    .WithReference(api);

using var app = builder.Build();

await app.RunAsync();
