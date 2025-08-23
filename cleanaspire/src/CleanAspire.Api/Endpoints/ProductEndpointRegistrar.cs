// This class defines minimal API endpoints related to product management.
// Each endpoint corresponds to a specific command or query defined in the Features layer.
// The purpose is to expose a RESTful interface for interacting with products, delegating request handling to command/query handlers via the Mediator pattern.

// Key points:
// 1. Each endpoint is linked to a command or query, ensuring clear separation between API and application logic.
// 2. Commands represent actions (e.g., CreateProductCommand, DeleteProductCommand).
// 3. Queries retrieve data (e.g., GetAllProductsQuery, GetProductByIdQuery, ProductsWithPaginationQuery).
// 4. Mediator is used to send commands/queries to their respective handlers, enabling a clean and testable architecture.


using CleanAspire.Application.Common.Models;
using CleanAspire.Application.Features.Products.Commands;
using CleanAspire.Application.Features.Products.DTOs;
using CleanAspire.Application.Features.Products.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using static CleanAspire.Api.Endpoints.FileUploadEndpointRegistrar;

namespace CleanAspire.Api.Endpoints;

/// <summary>
/// This class defines minimal API endpoints related to product management.
/// Each endpoint corresponds to a specific command or query defined in the Features layer.
/// The purpose is to expose a RESTful interface for interacting with products, delegating request handling to command/query handlers via the Mediator pattern.
/// </summary>
public class ProductEndpointRegistrar(ILogger<ProductEndpointRegistrar> logger) : IEndpointRegistrar
{
    /// <summary>
    /// Registers the routes for product-related endpoints.
    /// </summary>
    /// <param name="routes">The route builder to which the endpoints will be added.</param>
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/products").WithTags("products").RequireAuthorization();

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>A list of all products in the system.</returns>
        group.MapGet("/", async ([FromServices] IMediator mediator) =>
        {
            var query = new GetAllProductsQuery();
            return await mediator.Send(query);
        })
        .Produces<IEnumerable<ProductDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Get all products")
        .WithDescription("Returns a list of all products in the system.");

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">The unique ID of the product.</param>
        /// <returns>The details of the specified product.</returns>
        group.MapGet("/{id}", (IMediator mediator, [FromRoute] string id) => mediator.Send(new GetProductByIdQuery(id)))
        .Produces<ProductDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Get product by ID")
        .WithDescription("Returns the details of a specific product by its unique ID.");

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="command">The command containing the details of the product to create.</param>
        /// <returns>The created product.</returns>
        group.MapPost("/", ([FromServices] IMediator mediator, [FromBody] CreateProductCommand command) => mediator.Send(command))
             .Produces<ProductDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Create a new product")
        .WithDescription("Creates a new product with the provided details.");

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="command">The command containing the updated details of the product.</param>
        group.MapPut("/", ([FromServices] IMediator mediator, [FromBody] UpdateProductCommand command) => mediator.Send(command))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Update an existing product")
        .WithDescription("Updates the details of an existing product.");

        /// <summary>
        /// Deletes products by their IDs.
        /// </summary>
        /// <param name="command">The command containing the IDs of the products to delete.</param>
        group.MapDelete("/", ([FromServices] IMediator mediator, [FromBody] DeleteProductCommand command) => mediator.Send(command))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Delete products by IDs")
        .WithDescription("Deletes one or more products by their unique IDs.");

        /// <summary>
        /// Gets products with pagination and filtering.
        /// </summary>
        /// <param name="query">The query containing pagination and filtering parameters.</param>
        /// <returns>A paginated list of products.</returns>
        group.MapPost("/pagination", ([FromServices] IMediator mediator, [FromBody] ProductsWithPaginationQuery query) => mediator.Send(query))
        .Produces<PaginatedResult<ProductDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Get products with pagination")
        .WithDescription("Returns a paginated list of products based on search keywords, page size, and sorting options.");

        /// <summary>
        /// Exports products to a CSV file.
        /// </summary>
        /// <param name="keywords">The keywords to filter the products.</param>
        /// <returns>A CSV file containing the product data.</returns>
        group.MapGet("/export", async ([FromQuery] string keywords, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ExportProductsQuery(keywords));
            result.Position = 0;
            return Results.File(result, "text/csv", "exported-products.csv");
        })
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Export Products to CSV")
        .WithDescription("Exports the product data to a CSV file based on the provided keywords. The CSV file includes product details such as ID, name, description, price, SKU, and category.");

        /// <summary>
        /// Imports products from CSV files.
        /// </summary>
        /// <param name="request">The request containing the CSV files to import.</param>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A list of responses for each imported file.</returns>
        group.MapPost("/import", async ([FromForm] FileUploadRequest request, HttpContext context, [FromServices] IMediator mediator) =>
        {
            var response = new List<FileUploadResponse>();
            foreach (var file in request.Files)
            {
                // Validate file type
                if (Path.GetExtension(file.FileName).ToLower() != ".csv")
                {
                    logger.LogWarning($"Invalid file type: {file.FileName}");
                    return Results.BadRequest("Only CSV files are supported.");
                }
                var fileName = file.FileName;
                // Copy file to memory stream
                var filestream = file.OpenReadStream();
                var stream = new MemoryStream();
                await filestream.CopyToAsync(stream);
                stream.Position = 0;
                var fileSize = stream.Length;
                // Send the file stream to ImportProductsCommand
                var importCommand = new ImportProductsCommand(stream);
                await mediator.Send(importCommand);

                response.Add(new FileUploadResponse
                {
                    Path = file.FileName,
                    Url = $"Imported {fileName}",
                    Size = fileSize
                });
            }

            return TypedResults.Ok(response);

        }).DisableAntiforgery()
          .Accepts<FileUploadRequest>("multipart/form-data")
          .Produces<IEnumerable<FileUploadResponse>>()
          .ProducesProblem(StatusCodes.Status400BadRequest)
          .ProducesProblem(StatusCodes.Status500InternalServerError)
          .WithMetadata(new ConsumesAttribute("multipart/form-data"))
          .WithSummary("Import Products from CSV")
          .WithDescription("Imports product data from one or more CSV files. The CSV files should contain product details in the required format.");
    }
}

