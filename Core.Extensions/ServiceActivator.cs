using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extensions
{
    public static class ServiceActivator
    {
        private static IServiceProvider? _serviceProvider;

        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetScopedService<T>()
        {
            using var scope = GetScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }
        public static T? GetService<T>() => _serviceProvider.GetService<T>();

        public static T GetRequiredService<T>() => _serviceProvider!.GetRequiredService<T>();


        public static object GetRequiredService(Type type) => _serviceProvider!.GetRequiredService(type);


        public static IEnumerable<T> GetServices<T>() => _serviceProvider.GetServices<T>();

        public static object? GetService(Type type) => _serviceProvider?.GetService(type);


        public static IEnumerable<object> GetServices(Type type) => _serviceProvider!.GetServices(type);

        private static IServiceScope GetScope(IServiceProvider? serviceProvider = null)
        {
            var provider = serviceProvider ?? _serviceProvider
                ?? throw new InvalidOperationException("Service provider has not been configured.");

            return provider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }
    }
}
