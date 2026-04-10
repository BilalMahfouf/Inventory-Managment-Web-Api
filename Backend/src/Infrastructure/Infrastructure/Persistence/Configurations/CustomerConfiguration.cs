using Domain.Shared.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Persistence.Configurations
{
    public partial class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            entity.HasIndex(e => e.Email, "IX_Customers_Email");

            entity.HasIndex(e => e.Name, "IX_Customers_Name");
            entity.ComplexProperty(c => c.Address, cb =>
            {
                cb.Property(c => c.Street).HasMaxLength(255);
                cb.Property(c => c.City).HasMaxLength(100);
                cb.Property(c => c.State).HasMaxLength(100);
                cb.Property(c => c.ZipCode).HasMaxLength(20);
            });

            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.CreditStatus)
            .HasConversion<byte>();

            entity.Property(e => e.DeletedAt);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.CustomerCategory).WithMany()
                .HasForeignKey(d => d.CustomerCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customers_CreatedByUser");

            entity.HasOne(d => d.DeletedByUser).WithMany()
                .HasForeignKey(d => d.DeletedByUserId)
                .HasConstraintName("FK_Customers_DeletedByUser");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Customer> entity);
    }
}
