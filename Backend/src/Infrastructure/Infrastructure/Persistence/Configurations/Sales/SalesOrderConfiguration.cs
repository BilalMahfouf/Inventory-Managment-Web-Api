using Domain.Shared.Errors;
using Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace Infrastructure.Infrastructure.Persistence.Configurations.Sales
{
    public partial class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
    {
        public void Configure(EntityTypeBuilder<SalesOrder> entity)
        {
            entity.HasIndex(e => e.CustomerId, "IX_SalesOrders_CustomerId");

            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.OrderDate);
            entity.Property(e => e.SalesStatus).HasDefaultValue(SalesOrderStatus.Pending);
            entity.Property(e => e.PaymentStatus).HasDefaultValue(PaymentStatus.Unpaid);
            entity.Property(e => e.IsWalkIn).HasDefaultValue(false);
            entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            entity.Property(e => e.TrackingNumber).HasMaxLength(150);
            entity.Property(e => e.TotalAmount);

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesOrders_CreatedByUser");

            entity.HasOne(d => d.Customer).WithMany()
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SalesOrders_Customers");


            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<SalesOrder> entity);
    }
}
