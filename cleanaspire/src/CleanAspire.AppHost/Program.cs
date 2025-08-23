var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.CleanAspire_Api>("apiservice");

builder.AddProject<Projects.CleanAspire_WebApp>("blazorweb")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
