// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;
using System.Text;
using CleanAspire.Api.Client;
using CleanAspire.ClientApp.Services.Interfaces;

namespace CleanAspire.ClientApp.Services.PushNotifications;

public class WebpushrService: IWebpushrService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiClient _apiClient;
    private readonly IStorageService _storageService;
    private readonly ILogger<WebpushrService> _logger;
    private const string WEBPUSHRPUBLICKEY = "_webpushrPublicKey";
    public WebpushrService(IHttpClientFactory httpClientFactory, ApiClient apiClient, IStorageService storageService, ILogger<WebpushrService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _apiClient = apiClient;
        _storageService = storageService;
        _logger = logger;
    }
    public async Task<string> GetPublicKeyAsync()
    {
        try
        {
            var publicKey = await _storageService.GetItemAsync<string>(WEBPUSHRPUBLICKEY);
            if (string.IsNullOrEmpty(publicKey))
            {
                var res = await _apiClient.Webpushr.Config.GetAsync();
                if (res != null)
                {
                    await _storageService.SetItemAsync(WEBPUSHRPUBLICKEY, res.PublicKey);
                    publicKey = res.PublicKey;
                }
            }
            return publicKey ?? string.Empty;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
    public async Task SendNotificationAsync(string title, string message, string targetUrl, string? sid = null)
    {
        var client = _httpClientFactory.CreateClient("Webpushr");
        try
        {
            var payload = $@"{{
                ""title"": ""{title}"",
                ""message"": ""{message}"",
                ""target_url"": ""{targetUrl}"",
                ""icon"":""icon-192.png""
            }}";

            HttpContent httpContent = new StringContent(payload, Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var url = sid == null ? "/v1/notification/send/all" : $"/v1/notification/send/sid/{sid}";
            var httpResponseMessage = await client.PostAsync(url, httpContent).ConfigureAwait(false);
            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Error sending notification: {responseContent}");
            }

            httpContent.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending notification.");
        }
        finally
        {
            client.Dispose();
        }
    }
}

