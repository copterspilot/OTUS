// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.Application.Features.Tenants.Commands;
using CleanAspire.Application.Features.Tenants.DTOs;
using CleanAspire.Application.Features.Tenants.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CleanAspire.Api.Endpoints;

public class TenantEndpointRegistrar : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/tenants").WithTags("tenants");

        group.MapGet("/", async ([FromServices] IMediator mediator) => await mediator.Send(new GetAllTenantsQuery()))
             .Produces<IEnumerable<TenantDto>>()
             .AllowAnonymous()
             .WithSummary("Get all tenants")
             .WithDescription("Returns a list of all tenants in the system.");

        group.MapGet("/{id}", async (IMediator mediator, [FromRoute] string id) => await mediator.Send(new GetTenantByIdQuery(id)))
            .Produces<TenantDto>()
            .AllowAnonymous()
            .WithSummary("Get tenant by ID")
            .WithDescription("Returns the details of a specific tenant by their unique ID.");

        group.MapPost("/", async ([FromServices] IMediator mediator, [FromBody] CreateTenantCommand command) =>
        {
            var id = await mediator.Send(command);
            return TypedResults.Ok(id);
        }).RequireAuthorization()
            .WithSummary("Create a new tenant")
            .WithDescription("Creates a new tenant with the provided details.");

        group.MapPut("/", async ([FromServices] IMediator mediator, [FromBody] UpdateTenantCommand command) => await mediator.Send(command))
            .RequireAuthorization()
            .WithSummary("Update an existing tenant")
            .WithDescription("Updates the details of an existing tenant.");

        group.MapDelete("/", async (IMediator mediator, [FromQuery] string[] ids) => await mediator.Send(new DeleteTenantCommand(ids)))
            .RequireAuthorization()
            .WithSummary("Delete tenants by IDs")
            .WithDescription("Deletes one or more tenants by their unique IDs.");
    }
}
