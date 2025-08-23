// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.Api.Client.Models;

namespace CleanAspire.ClientApp.Services.Identity;

public class UserProfileStore
{
    public event EventHandler<UserProfileStoreEvent>? OnChange;
    public ProfileResponse? Profile { get; private set; }

    public void Set(ProfileResponse? profile)
    {
        Profile = profile;
        OnChange?.Invoke(this, new UserProfileStoreEvent(Profile));
    }

    public void Clear()
    {
        Profile = null;
        OnChange?.Invoke(this, new UserProfileStoreEvent(Profile));
    }
}

