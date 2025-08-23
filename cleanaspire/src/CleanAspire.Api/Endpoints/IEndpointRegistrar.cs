// This namespace contains utilities for defining and registering API endpoint routes in a minimal API setup.

// Purpose:
// 1. **`IEndpointRegistrar` Interface**:
//    - Provides a contract for defining endpoint registration logic.
//    - Ensures consistency across all endpoint registration implementations by enforcing a common method (`RegisterRoutes`).

namespace CleanAspire.Api.Endpoints;

/// <summary>
/// Defines a contract for registering endpoint routes.
/// </summary>
public interface IEndpointRegistrar
{
    /// <summary>
    /// Registers the routes for the application.
    /// </summary>
    /// <param name="routes">The <see cref="IEndpointRouteBuilder"/> to add routes to.</param>
    void RegisterRoutes(IEndpointRouteBuilder routes);
}
