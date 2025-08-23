var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddValkey("cache")
    .WithDataVolume(isReadOnly: false)
    .WithPersistence(interval: TimeSpan.FromMinutes(5), keysChangedThreshold: 100);

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithRealmImport("./Realms/WeatherShop-realm.json");

var apiService = builder.AddProject<Projects.Keycloak_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(keycloak).WaitFor(keycloak);

builder.AddProject<Projects.Keycloak_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache).WaitFor(cache)
    .WithReference(keycloak).WaitFor(keycloak)
    .WithReference(apiService).WaitFor(apiService);

await builder.Build().RunAsync().ConfigureAwait(false);
