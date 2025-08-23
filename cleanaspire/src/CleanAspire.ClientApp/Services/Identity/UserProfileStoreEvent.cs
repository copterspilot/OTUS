// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.Api.Client.Models;

namespace CleanAspire.ClientApp.Services.Identity;

public class UserProfileStoreEvent
{
    public ProfileResponse? Profile { get; private set; }


    public UserProfileStoreEvent(ProfileResponse? profile)
    {
        Profile = profile;
    }

    public override string ToString()
    {
        return $"userId: {Profile?.UserId}";
    }
}

