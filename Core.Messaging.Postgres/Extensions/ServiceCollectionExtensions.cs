using Core.Abstractions.Messaging.Outbox;
using Core.Messaging.Outbox;
using Core.Messaging.Postgres.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Core.Messaging.Postgres.Extensions
{
    // PostgreSQL için messaging servislerini ekleyen extension
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            AddOutbox(services, configuration); // Outbox yapılandırmasını uygula
            return services;
        }

        private static void AddOutbox(IServiceCollection services, IConfiguration configuration)
        {
            var outboxOption = configuration.GetSection("OutboxOptions").Get<OutboxOptions>();

            services.Configure<OutboxOptions>(configuration.GetSection("OutboxOptions"));

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // Timestamp uyumluluğu

            // DbContext’i PostgreSQL için ayarla
            services.AddDbContext<OutboxDataContext>(options =>
            {
                options.UseNpgsql(outboxOption?.ConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name); // Migration içeren assembly
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); // Retry ayarları
                }).UseSnakeCaseNamingConvention().ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)); // snake_case kullan
            });

            // Outbox servisini DI container’a ekle
            services.AddScoped<IOutboxService, EfOutboxService<OutboxDataContext>>();
        }
    }
}
