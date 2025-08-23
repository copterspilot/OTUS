// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using CleanAspire.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanAspire.Infrastructure.Persistence.Configurations;
/// <summary>
/// Configures the Stock entity.
/// </summary>
public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    /// <summary>
    /// Configures the properties and relationships of the Stock entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the Stock entity.</param>
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        /// <summary>
        /// Configures the ProductId property of the Stock entity.
        /// </summary>
        builder.Property(x => x.ProductId).HasMaxLength(50).IsRequired();

        /// <summary>
        /// Configures the relationship between the Stock and Product entities.
        /// </summary>
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);

        /// <summary>
        /// Configures the Location property of the Stock entity.
        /// </summary>
        builder.Property(x => x.Location).HasMaxLength(12).IsRequired();

        /// <summary>
        /// Ignores the DomainEvents property of the Stock entity.
        /// </summary>
        builder.Ignore(e => e.DomainEvents);
    }
}
