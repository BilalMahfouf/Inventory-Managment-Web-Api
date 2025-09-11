using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Infrastructure.Infrastructure.Persistence.Configurations
{
    
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("ima");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(i => i.StoragePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.MimeType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.SizeInBytes)
                .IsRequired();

            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.IsDeleted)
                .HasDefaultValue(false);

            // Relationships (Audit trail)
            builder.HasOne(i => i.CreatedByUser)
                .WithMany()
                .HasForeignKey(i => i.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.UpdatedByUser)
                .WithMany()
                .HasForeignKey(i => i.UpdatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(i => i.DeletedByUser)
                .WithMany()
                .HasForeignKey(i => i.DeletedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes
            builder.HasIndex(i => i.FileName);
            builder.HasIndex(i => i.StoragePath).IsUnique();
            builder.HasIndex(i => i.MimeType);
            builder.HasIndex(i => i.IsDeleted);
            builder.HasIndex(i => i.CreatedByUserId);
        }
    }

}
