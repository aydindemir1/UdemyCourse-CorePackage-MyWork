using Core.Security.Redis.Repositories;
using Core.Security.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Redis
{
    public static class SecurityRedisServiceRegistrationExtensions
    {
        public static IServiceCollection AddRedisSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthorizedRoleRepository, AuthorizedRoleRepository>();
            services.AddScoped<IAuthorizedRoleService, AuthorizedRoleManager>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var redisConnectionString = config.GetConnectionString("AuthRoleConnection");
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
            return services;
        }
    }
}
