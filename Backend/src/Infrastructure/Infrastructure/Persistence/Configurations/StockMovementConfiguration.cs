using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Persistence.Configurations
{
    public partial class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> entity)
        {
            entity.HasIndex(e => e.InventoryId, "IX_StockMovements_InventoryId");

            entity.HasIndex(e => e.MovementTypeId, "IX_StockMovements_MovementTypeId");

            entity.HasIndex(e => e.ProductId, "IX_StockMovements_ProductId");

            entity.HasIndex(e => new { e.ProductId, e.CreatedAt }, "IX_StockMovements_Product_Date");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StockMovmentStatus)
                .HasDefaultValue(StockMovementStatus.Pending)
                .HasConversion<byte>();

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_CreatedByUser");

            entity.HasOne(d => d.Inventory).WithMany(d=>d.StockMovements)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_Locations");

            entity.HasOne(d => d.MovementType).WithMany()
                .HasForeignKey(d => d.MovementTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_MovementTypes");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_Products");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<StockMovement> entity);
    }
}
