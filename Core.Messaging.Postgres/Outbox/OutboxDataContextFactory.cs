using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Postgres.Outbox
{
    public class OutboxDataContextFactory : IDesignTimeDbContextFactory<OutboxDataContext>
    {
        public OutboxDataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OutboxDataContext>();

            // Buradaki connection string önemli değil, sadece Migration üretmek için Npgsql'i tanıtıyoruz.
            optionsBuilder.UseNpgsql("Host=localhost;Database=dummy_db;Username=postgres;Password=postgres")
                .UseSnakeCaseNamingConvention();

            return new OutboxDataContext(optionsBuilder.Options);
        }
    }
}
