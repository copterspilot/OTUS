// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CleanAspire.ClientApp.Components.Autocompletes;

public static class PicklistDataSource
{
    public static readonly Dictionary<PicklistType, string[]> Data = new Dictionary<PicklistType, string[]>
    {
        { PicklistType.UOM, new[] { "EA", "PCS", "KG", "M", "L", "BOX", "PKG" } },
        { PicklistType.Currency, new[] { "USD", "EUR", "GBP", "CNY", "JPY", "AUD", "CAD" } }
    };
}
public enum PicklistType
{
    UOM,
    Currency
}
