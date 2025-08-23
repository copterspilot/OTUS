// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.Application.Common.Models;
using CleanAspire.Application.Features.Stocks.Commands;
using CleanAspire.Application.Features.Stocks.DTOs;
using CleanAspire.Application.Features.Stocks.Queryies;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CleanAspire.Api.Endpoints;

public class StockEndpointRegistrar(ILogger<ProductEndpointRegistrar> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/stocks").WithTags("stocks").RequireAuthorization();

        // Dispatch stock
        group.MapPost("/dispatch", ([FromServices] IMediator mediator, [FromBody] StockDispatchingCommand command) => mediator.Send(command))
            .Produces<Unit>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Dispatch stock")
            .WithDescription("Dispatches a specified quantity of stock from a location.");

        // Receive stock
        group.MapPost("/receive", ([FromServices] IMediator mediator, [FromBody] StockReceivingCommand command) => mediator.Send(command))
            .Produces<Unit>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Receive stock")
            .WithDescription("Receives a specified quantity of stock into a location.");

        // Get stocks with pagination
        group.MapPost("/pagination", ([FromServices] IMediator mediator, [FromBody] StocksWithPaginationQuery query) => mediator.Send(query))
            .Produces<PaginatedResult<StockDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get stocks with pagination")
            .WithDescription("Returns a paginated list of stocks based on search keywords, page size, and sorting options.");
    }

}
