// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanAspire.ClientApp.Services;

public static class SupportedLocalization
{
    public const string ResourcesPath = "Resources";

    public static readonly LanguageCode[] SupportedLanguages =
    {
        new()
        {
            Code = "en-US",
            DisplayName = "English (United States)"
        },
        new()
        {
            Code = "zh-CN",
            DisplayName = "中文（简体，中国）"
        },
        new()
        {
            Code = "de-DE",
            DisplayName = "Deutsch (Deutschland)"
        },
        new()
        {
            Code = "fr-FR",
            DisplayName = "français (France)"
        },
        new()
        {
            Code = "ja-JP",
            DisplayName = "日本語 (日本)"
        },
        new()
        {
            Code = "es-ES",
            DisplayName = "español (España)"
        },
        new()
        {
            Code = "ko-KR",
            DisplayName = "한국어(대한민국)"
        },
        new()
        {
            Code = "pt-BR",
            DisplayName = "português (Brasil)"
        }
    };
}

public class LanguageCode
{
    public string DisplayName { get; set; } = "en-US";
    public string Code { get; set; } = "English";
}
