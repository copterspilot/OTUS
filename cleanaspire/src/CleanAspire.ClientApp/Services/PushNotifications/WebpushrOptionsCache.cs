// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.Api.Client.Models;

namespace CleanAspire.ClientApp.Services.PushNotifications;

public class WebpushrOptionsCache
{
    private WebpushrOptions? _options;
    private DateTime _cacheExpiry = DateTime.MinValue;

    public async Task<WebpushrOptions?> GetOptionsAsync(Func<Task<WebpushrOptions?>> fetchOptionsFunc)
    {
        if (_options == null || _cacheExpiry < DateTime.UtcNow)
        {
            _options = await fetchOptionsFunc();
            _cacheExpiry = DateTime.UtcNow.AddHours(24);
        }
        return _options;
    }
}
