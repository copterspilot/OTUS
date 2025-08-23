// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CleanAspire.Api.Webpushr;

public class WebpushrOptions
{
    public static string Key = "Webpushr";
    public string Token { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
}
