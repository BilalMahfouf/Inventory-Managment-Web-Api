using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Persistence.Configurations
{
    public partial class AlertRuleConfiguration : IEntityTypeConfiguration<AlertRule>
    {
        public void Configure(EntityTypeBuilder<AlertRule> entity)
        {
            entity.HasIndex(e => e.AlertType, "IX_AlertRules_AlertType");

            entity.HasIndex(e => e.LocationId, "IX_AlertRules_LocationId");

            entity.HasIndex(e => e.ProductId, "IX_AlertRules_ProductId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.AlertTypeNavigation).WithMany()
                .HasForeignKey(d => d.AlertType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlertRules_AlertTypes");

            entity.HasOne(d => d.Location).WithMany()
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlertRules_Locations");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlertRules_Products");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<AlertRule> entity);
    }
}
