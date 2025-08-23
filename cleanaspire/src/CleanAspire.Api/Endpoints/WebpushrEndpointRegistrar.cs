// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using CleanAspire.Api.Webpushr;
using Microsoft.Extensions.Options;


namespace CleanAspire.Api.Endpoints;

public class WebpushrEndpointRegistrar : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var webpushrOptions = routes.ServiceProvider.GetRequiredService<IOptions<WebpushrOptions>>().Value;
        var group = routes.MapGroup("/webpushr").WithTags("webpushr").AllowAnonymous();

        group.MapGet("/config", () =>
        {
            return TypedResults.Ok(webpushrOptions);
        })
          .Produces<WebpushrOptions>(StatusCodes.Status200OK)
          .WithSummary("Retrieve current Webpushr configuration")
          .WithDescription("Returns the Webpushr configuration details currently loaded from the application's configuration system. This information includes keys and tokens used for Webpushr push notifications.");
    }
}


