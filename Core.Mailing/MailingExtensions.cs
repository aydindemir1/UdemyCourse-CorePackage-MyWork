using Core.Mailing.MailKitImplementation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Mailing
{
    public static class MailingExtensions
    {
        public static IServiceCollection AddMailing(this IServiceCollection services)
        {
            services.AddScoped<IMailService, MailKitMailService>();
            return services;
        }
    }
}
