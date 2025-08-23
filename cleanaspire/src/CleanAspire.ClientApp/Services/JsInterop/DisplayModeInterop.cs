// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace CleanAspire.ClientApp.Services.JsInterop;

public sealed class DisplayModeInterop
{
    private readonly IJSRuntime _jsRuntime;

    public DisplayModeInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public async Task<string> GetDisplayModeAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("displayModeInterop.getDisplayMode");
    }
}
