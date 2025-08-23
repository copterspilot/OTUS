// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CleanAspire.ClientApp.Services;

public sealed class OfflineSyncService
{
    public SyncStatus CurrentStatus { get; private set; } = SyncStatus.Idle;
    public string StatusMessage { get; private set; } = "Idle";

    public int TotalRequests { get; private set; }
    public int RequestsProcessed { get; private set; }
    public event Action? OnSyncStateChanged;
    public void SetSyncStatus(SyncStatus status, string message, int total = 0, int processed = 0)
    {
        CurrentStatus = status;
        StatusMessage = message;
        TotalRequests = total;
        RequestsProcessed = processed;
        OnSyncStateChanged?.Invoke();
    }
}
public enum SyncStatus
{
    Idle,
    Syncing,
    Completed,
    Failed
}
