using Domain.Entities.Products;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Persistence.Configurations
{
    public partial class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryId");

            entity.HasIndex(e => new { e.CategoryId, e.IsActive }, "IX_Products_Category_Active");

            entity.HasIndex(e => e.Name, "IX_Products_Name");

            entity.HasIndex(e => e.Sku, "IX_Products_SKU");

            entity.HasIndex(e => e.UnitOfMeasureId, "IX_Products_UnitOfMeasureId");

            entity.HasIndex(e => e.Sku, "UQ_Products_SKU").IsUnique();

            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("SKU");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany()
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_CreatedByUser");

            entity.HasOne(d => d.DeletedByUser).WithMany()
                .HasForeignKey(d => d.DeletedByUserId)
                .HasConstraintName("FK_Products_DeletedByUser");

            entity.HasOne(d => d.UnitOfMeasure).WithMany()
                .HasForeignKey(d => d.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_UnitOfMeasures");

            entity.HasOne(d => d.UpdatedByUser).WithMany()
                .HasForeignKey(d => d.UpdatedByUserId)
                .HasConstraintName("FK_Products_UpdatedByUser");
            entity.HasIndex(e => e.IsDeleted, "IX_Products_IsDeleted");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Product> entity);
    }
}
