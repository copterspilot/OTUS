// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.Api.Client;
namespace CleanAspire.ClientApp.Services.PushNotifications;

public class WebpushrAuthHandler : DelegatingHandler
{
    private readonly ApiClient _apiClient;
    private readonly WebpushrOptionsCache _optionsCache;
    public WebpushrAuthHandler(ApiClient apiClient, WebpushrOptionsCache optionsCache)
    {
        _apiClient = apiClient;
        _optionsCache = optionsCache;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var webpushrOptions = await _optionsCache.GetOptionsAsync(() => _apiClient.Webpushr.Config.GetAsync());
        if (webpushrOptions == null)
            throw new InvalidOperationException("Failed to retrieve Webpushr options.");

        request.Headers.Clear();
        request.Headers.Add("webpushrKey", webpushrOptions.ApiKey);
        request.Headers.Add("webpushrAuthToken", webpushrOptions.Token);
        request.Headers.Add("Accept", "application/json");
        return await base.SendAsync(request, cancellationToken);
    }
}
