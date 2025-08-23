// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CleanAspire.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpointDefinitions(this IEndpointRouteBuilder routes)
    {
        using (var scope = routes.ServiceProvider.CreateScope())
        {
            var registrars = scope.ServiceProvider.GetServices<IEndpointRegistrar>();
            foreach (var registrar in registrars)
            {
                registrar.RegisterRoutes(routes);
            }
        }
        return routes;
    }
}
