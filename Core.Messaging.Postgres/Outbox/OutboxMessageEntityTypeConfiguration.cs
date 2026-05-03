using Core.Abstractions.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Postgres.Outbox
{
    public class OutboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages", OutboxDataContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.OccurredOn)
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.Data)
                .IsRequired();

            builder.Property(x => x.EventType)
                .IsRequired();

            builder.Property(x => x.EventType)
                .HasMaxLength(50)
                .HasConversion(
                    v => v.ToString(),
                    v => (EventType)Enum.Parse(typeof(EventType), v))
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.ProcessedOn)
                .IsRequired(false);
        }
    }
}
