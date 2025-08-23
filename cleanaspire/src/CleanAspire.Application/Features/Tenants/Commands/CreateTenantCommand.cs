// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CleanAspire.Application.Features.Tenants.Commands;

public record CreateTenantCommand(string Name, string Description) : IFusionCacheRefreshRequest<string>
{
    public IEnumerable<string>? Tags => new[] { "tenants" };
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand,string>
{
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly IApplicationDbContext _dbContext;

    public CreateTenantCommandHandler(ILogger<CreateTenantCommandHandler> logger, IApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async ValueTask<string> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Creating a new tenant instance with a unique Id
        var tenant = new Tenant
        {
            Name = request.Name,
            Description = request.Description
        };
        // Logic to add tenant to the database
        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Tenant {TenantId} is created", tenant.Id);
        return tenant.Id;
    }
}
