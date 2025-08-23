// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CleanAspire.Application.Features.Tenants.DTOs;

public class TenantDto
{
    // Unique identifier for the tenant
    public string Id { get; set; } = string.Empty;
    // Tenant name, can be null
    public string? Name { get; set; }
    // Tenant description, can be null
    public string? Description { get; set; }
}
