using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Persistence.Configurations
{
    public partial class ConfirmEmailTokenConfiguration : IEntityTypeConfiguration<ConfirmEmailToken>
    {
        public void Configure(EntityTypeBuilder<ConfirmEmailToken> builder)
        {
            builder.ToTable("ConfirmEmailTokens");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(256); // adjust length depending on how you generate token

            builder.Property(e => e.UserId)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
               .HasDefaultValueSql("(getdate())")
               .HasColumnType("datetime");

            builder.Property(e => e.ExpiredAt)
                .IsRequired();

            builder.Property(e => e.IsLocked).HasDefaultValue(false).IsRequired();

            // 1-to-many (Users can have multiple tokens)
            builder.HasOne(e => e.User)
                .WithMany(u => u.ConfirmEmailTokens) // Add this collection in Users entity
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_ConfirmEmailTokens_UserId");

            OnConfigurePartial(builder);
        }
        partial void OnConfigurePartial(EntityTypeBuilder<ConfirmEmailToken> builder);
    }
}
