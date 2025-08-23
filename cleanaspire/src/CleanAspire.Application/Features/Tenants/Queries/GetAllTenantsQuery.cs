// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.Application.Features.Tenants.DTOs;
namespace CleanAspire.Application.Features.Tenants.Queries;

public record GetAllTenantsQuery() : IFusionCacheRequest<List<TenantDto>>
{
    public string CacheKey => "GetAllTenants";
    public IEnumerable<string>? Tags => new[] { "tenants" };
}

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, List<TenantDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllTenantsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<List<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _dbContext.Tenants.OrderBy(x=>x.Name)
            .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description
            })
            .ToListAsync(cancellationToken);

        return tenants;
    }
}
