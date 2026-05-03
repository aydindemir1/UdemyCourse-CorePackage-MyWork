using Core.Abstractions.Messaging.Outbox;
using Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Core.Messaging.Postgres.Outbox
{
    public class OutboxDataContext : EfDbContextBase
    {
        public const string DefaultSchema = "messaging"; // Varsayılan şema adı

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public OutboxDataContext(DbContextOptions<OutboxDataContext> options //, IDomainEventDispatcher? domainEventDispatcher = null
                                                                             ) : base(options//, domainEventDispatcher
                                                                                             
                                                                                             ) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp"); // UUID üreticisi eklentisi
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
