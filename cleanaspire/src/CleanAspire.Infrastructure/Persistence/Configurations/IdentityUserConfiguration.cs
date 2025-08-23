// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanAspire.Domain.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanAspire.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Each User can have many UserLogins
        builder.HasOne(x => x.Superior).WithMany().HasForeignKey(u => u.SuperiorId);
        builder.Property(x => x.Nickname).HasMaxLength(50);
        builder.Property(x => x.Provider).HasMaxLength(50);
        builder.Property(x => x.TenantId).HasMaxLength(50);
        builder.Property(x => x.AvatarUrl).HasMaxLength(255);
        builder.Property(x => x.RefreshToken).HasMaxLength(255);
        builder.Property(x => x.LanguageCode).HasMaxLength(255);
        builder.Property(x => x.TimeZoneId).HasMaxLength(255);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(50);
    }
}
