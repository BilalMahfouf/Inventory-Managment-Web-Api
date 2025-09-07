using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Persistence.Configurations
{
    public partial class SupplierContactConfiguration : IEntityTypeConfiguration<SupplierContact>
    {
        public void Configure(EntityTypeBuilder<SupplierContact> entity)
        {
            entity.HasIndex(e => e.SupplierId, "IX_SupplierContacts_SupplierId");

            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.ContactType).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JobTitle).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierContacts_CreatedByUser");

            entity.HasOne(d => d.DeletedByUser).WithMany()
                .HasForeignKey(d => d.DeletedByUserId)
                .HasConstraintName("FK_SupplierContacts_DeletedByUser");

            entity.HasOne(d => d.Supplier).WithMany()
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierContacts_Suppliers");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<SupplierContact> entity);
    }
}
