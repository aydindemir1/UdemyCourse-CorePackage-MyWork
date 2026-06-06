using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Hashing
{
    public static class HashingServiceRegistration
    {
        public static IServiceCollection AddHashingServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            return services;
        }
    }
}
