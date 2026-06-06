using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.EmailAuthenticator
{
    public static class EmailAuthenticatorServiceRegistration
    {
        public static IServiceCollection AddEmailAuthenticatorServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailAuthenticator, EmailAuthenticatorManager>();
            return services;
        }
    }
}
