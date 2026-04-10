using Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace Infrastructure.Infrastructure.Persistence.Configurations.Sales
{
    public partial class SalesOrderItemConfiguration : IEntityTypeConfiguration<SalesOrderItem>
    {
        public void Configure(EntityTypeBuilder<SalesOrderItem> entity)
        {
            entity.HasIndex(e => new { e.SalesOrderId, e.ProductId }, "IX_SalesOrderItems_Order_Product");

            entity.HasIndex(e => e.ProductId, "IX_SalesOrderItems_ProductId");

            entity.HasIndex(e => e.InventoryId, "IX_SalesOrderItems_InventoryId");

            entity.HasIndex(e => e.SalesOrderId, "IX_SalesOrderItems_SalesOrderId");

            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.LineAmount);
            entity.Property(e => e.OrderedQuantity);
            entity.Property(e => e.ReceivedQuantity);
            entity.Property(e => e.UnitCost);

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesOrderItems_CreatedByUser");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesOrderItems_Products");

            entity.HasOne(d => d.Inventory).WithMany()
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesOrderItems_Inventory");

            entity.HasOne(d => d.SalesOrder).WithMany(d=>d.Items)
                .HasForeignKey(d => d.SalesOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesOrderItems_SalesOrders");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<SalesOrderItem> entity);
    }
}
