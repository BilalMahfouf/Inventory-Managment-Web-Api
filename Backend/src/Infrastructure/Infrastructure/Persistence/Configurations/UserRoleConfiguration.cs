using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Persistence.Configurations
{
    public partial class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> entity)
        {
            entity.HasIndex(e => e.Name, "UQ_UserRoles_Name").IsUnique();

            entity.Property(e => e.CreatedAt).HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.DeletedByUser).WithMany()
                .HasForeignKey(d => d.DeletedByUserId)
                .HasConstraintName("FK_UserRoles_DeletedByUser");

            entity.HasOne(d => d.CreatedByUser).WithMany()
             .HasForeignKey(d => d.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            

            entity.HasOne(d => d.UpdatedByUser).WithMany()
            .HasForeignKey(d => d.UpdatedByUserId).OnDelete(DeleteBehavior.Restrict);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<UserRole> entity);
    }
}
