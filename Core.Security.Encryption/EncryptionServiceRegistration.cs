using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Encryption
{
    public static class EncryptionServiceRegistration
    {
        public static IServiceCollection AddEncryptServices(this IServiceCollection services)
        {
            services.AddScoped<ISigningCredentialsProvider, SigningCredentialsProvider>();
            return services;
        }
    }
}
