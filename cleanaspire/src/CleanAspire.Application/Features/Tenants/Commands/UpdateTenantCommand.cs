// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CleanAspire.Application.Features.Tenants.Commands;

public record UpdateTenantCommand(string Id, string Name, string Description) : IFusionCacheRefreshRequest<Unit>
{
    public IEnumerable<string>? Tags => new[] { "tenants" };
}

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand,Unit>
{
    private readonly ILogger<UpdateTenantCommandHandler> _logger;
    private readonly IApplicationDbContext _dbContext;

    public UpdateTenantCommandHandler(ILogger<UpdateTenantCommandHandler> logger, IApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async ValueTask<Unit> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        // Logic to update tenant in the database
        var tenant = await _dbContext.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);
        if (tenant == null)
        {
            _logger.LogError($"Tenant with Id {request.Id} not found.");
            throw new Exception($"Tenant with Id {request.Id} not found.");
        }

        tenant.Name = request.Name;
        tenant.Description = request.Description;
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Updated Tenant: {request.Id}, Name: {request.Name}");
        return Unit.Value;
    }
}
