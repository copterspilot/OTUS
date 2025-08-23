// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace CleanAspire.ClientApp.Services.PushNotifications;

public interface IWebpushrService
{
    Task<string> GetPublicKeyAsync();
    Task SendNotificationAsync(string title, string message, string url, string? sid = null);
}
