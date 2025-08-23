// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace CleanAspire.ClientApp.Services.JsInterop;

using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public class OnlineStatusInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private DotNetObjectReference<OnlineStatusInterop>? _dotNetRef;

    public event Action<bool>? OnlineStatusChanged;

    public void Initialize()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        jsRuntime.InvokeVoidAsync("onlineStatusInterop.addOnlineStatusListener", _dotNetRef);
    }

    public async Task<bool> GetOnlineStatusAsync()
    {
        return await jsRuntime.InvokeAsync<bool>("onlineStatusInterop.getOnlineStatus");
    }

    [JSInvokable]
    public void UpdateOnlineStatus(bool isOnline)
    {
        OnlineStatusChanged?.Invoke(isOnline);
    }

    public ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        return ValueTask.CompletedTask;
    }
}

