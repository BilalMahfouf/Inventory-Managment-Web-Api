using Domain.Inventories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Persistence.Configurations
{
    public partial class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> entity)
        {
            entity.ToTable("Inventory");

            entity.HasIndex(e => e.LocationId, "IX_Inventory_LocationId");

            entity.HasIndex(e => e.ProductId, "IX_Inventory_ProductId");

            entity.HasIndex(e => new { e.ProductId, e.LocationId }, "IX_Inventory_Product_Location_Quantity");

            entity.HasIndex(e => e.QuantityOnHand, "IX_Inventory_QuantityOnHand");

            entity.HasIndex(e => e.ReorderLevel, "IX_Inventory_ReorderLevel");

            entity.HasIndex(e => new { e.ProductId, e.LocationId }, "UQ_Inventory_ProductLocation").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MaxLevel).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuantityOnHand).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_CreatedByUser");

            entity.HasOne(d => d.Location).WithMany()
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Locations");

            entity.HasOne(d => d.Product).WithMany(d=>d.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Products");

            entity.HasOne(d => d.UpdatedByUser).WithMany()
                .HasForeignKey(d => d.UpdatedByUserId)
                .HasConstraintName("FK_Inventory_UpdatedByUser");

            entity.HasOne(d => d.DeletedByUser).WithMany()
                .HasForeignKey(d => d.DeletedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Inventory> entity);
    }
}
