using Core.Abstractions.Cqrs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Core.Cqrs
{
    public static class CqrsExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, Assembly[]? assemblies = null, Action<IServiceCollection> doMoreActions = null)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies ?? new[] { Assembly.GetCallingAssembly() });
                cfg.Lifetime = ServiceLifetime.Scoped;
            });

            services.AddScoped<ICqrsProcessor, CqrsProcessor>();

            doMoreActions?.Invoke(services);
            return services;
        }
    }
}
