using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Persistence.Configurations
{
    public partial class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> entity)
        {
            entity.HasIndex(e => e.ProductId, "IX_ProductImages_ProductId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImages_CreatedByUser");

            entity.HasOne(d => d.DeletedByUser).WithMany()
                .HasForeignKey(d => d.DeletedByUserId)
                .HasConstraintName("FK_ProductImages_DeletedByUser");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImages_Products");

            entity.HasOne(e => e.UpdatedByUser).WithMany()
                .HasForeignKey(e => e.UpdatedByUserId).OnDelete(
                DeleteBehavior.Restrict);

            entity.HasOne(e => e.Image).WithMany()
                .HasForeignKey(e => e.ImageId).OnDelete(
                DeleteBehavior.Restrict);

            entity.HasIndex(i=>i.ImageId);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<ProductImage> entity);
    }
}
