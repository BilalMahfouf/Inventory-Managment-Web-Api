using Domain.Entities;
using Domain.Entities.Products;
using Domain.Inventories;
using Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Infrastructure.Persistence.Configurations.Sales;

internal class SalesOrderReservationConfiguration
    : IEntityTypeConfiguration<SalesOrderReservation>
{
    public void Configure(EntityTypeBuilder<SalesOrderReservation> builder)
    {
        builder.ToTable("SalesOrderReservations");
        builder.HasKey(e => e.Id);

        builder.HasOne<SalesOrder>()
            .WithMany(e => e.Reservations)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne<Inventory>()
            .WithMany()
            .HasForeignKey(e => e.InventoryId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        
        builder.HasOne<StockMovement>()
            .WithMany()
            .HasForeignKey(e => e.StockMovemntId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
