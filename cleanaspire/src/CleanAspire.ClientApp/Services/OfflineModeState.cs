// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.ClientApp.Services.Interfaces;

namespace CleanAspire.ClientApp.Services;

public class OfflineModeState
{
    private const string OfflineModeKey = "_offlineMode";
    private readonly IStorageService _storageService;
    public bool Enabled { get; private set; }

    public event Func<Task>? OnChange;
    public OfflineModeState(IStorageService storageService)
    {
        _storageService = storageService;
        // Initialize the OfflineModeEnabled with a default value
        Enabled = true;
    }
    // Initialize the offline mode setting from localStorage
    public async Task InitializeAsync()
    {
        var storedValue = await _storageService.GetItemAsync<bool?>(OfflineModeKey);
        Enabled = storedValue ?? true;
    }
    // Update the OfflineModeEnabled and persist it to localStorage
    public async Task SetOfflineModeAsync(bool isEnabled)
    {
        Enabled = isEnabled;
        await _storageService.SetItemAsync(OfflineModeKey, isEnabled);
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
