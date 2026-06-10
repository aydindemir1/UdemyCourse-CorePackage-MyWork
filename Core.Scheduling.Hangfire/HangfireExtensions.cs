using Core.Abstractions.Scheduler;
using Core.Scheduling.Hangfire.Scheduler;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Hangfire.SqlServer;
using Hangfire.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Core.Scheduling.Hangfire
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfireScheduler(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(nameof(HangfireMessageSchedulerOptions)).Get<HangfireMessageSchedulerOptions>();

            if (!options.UseInMemoryStorage)
            {
                services.TryAddSingleton(new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromMilliseconds(100),
                    UseRecommendedIsolationLevel = true,
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(1),

                });
            }

            services.TryAddSingleton<IBackgroundJobFactory>(provider =>
            new CustomBackgroundJobFactory(new BackgroundJobFactory(provider.GetRequiredService<IJobFilterProvider>()), provider.GetRequiredService<ILogger<CustomBackgroundJobFactory>>()));


            services.TryAddSingleton<IBackgroundJobPerformer>(provider =>
            new CustomBackgroundJobPerformer(new BackgroundJobPerformer(
                provider.GetRequiredService<IJobFilterProvider>(),
                provider.GetRequiredService<JobActivator>(),
                TaskScheduler.Default),
                provider.GetRequiredService<ILogger<CustomBackgroundJobPerformer>>())
            );

            services.TryAddSingleton<IBackgroundJobStateChanger>(provider =>
              new CustomBackgroundJobStateChanger(new BackgroundJobStateChanger(provider.GetRequiredService<IJobFilterProvider>()), provider.GetRequiredService<ILogger<CustomBackgroundJobStateChanger>>()));

            services.AddHangfire((provider, config) =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170).UseSimpleAssemblyNameTypeSerializer();

                if (options.UseInMemoryStorage)
                    config.UseMemoryStorage();
                else
                    config.UseSqlServerStorage(options.ConnectionString, provider.GetRequiredService<SqlServerStorageOptions>());
            });

            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.StopTimeout = TimeSpan.FromSeconds(15);

                serverOptions.ShutdownTimeout = TimeSpan.FromSeconds(30);

                serverOptions.Queues = new[] { "default", "app2_queue" };
            });

            services.AddScoped<IScheduler, HangfireScheduler>();
            services.AddScoped<IHangfireScheduler, HangfireScheduler>();
            services.AddScoped<ICommandScheduler, HangfireScheduler>();

            return services;



        }

        public static IApplicationBuilder UseHangfireScheduler(this IApplicationBuilder app)
        {
            return app.UseHangfireDashboard("/mydashboard");
        }
    }
}
