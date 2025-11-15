using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Infrastructure.Persistence.Configurations;

public partial class OutboxMessagesConfiguration : IEntityTypeConfiguration<OutboxMessages>
{
    public void Configure(EntityTypeBuilder<OutboxMessages> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Content).IsRequired();
        builder.Property(e => e.CreatedOnUtc).IsRequired();

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<OutboxMessages> entity);
}
