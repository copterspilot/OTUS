// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanAspire.Domain.Common;

namespace CleanAspire.Domain.Entities;

/// <summary>
/// Represents a stock entity.
/// </summary>
public class Stock : BaseAuditableEntity, IAuditTrial
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product associated with the stock.
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the stock.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the location of the stock.
    /// </summary>
    public string Location { get; set; } = string.Empty;
}
