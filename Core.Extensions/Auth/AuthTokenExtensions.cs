using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extensions.Auth
{
    public static class AuthTokenExtensions
    {
        public static IHttpClientBuilder AddAuthTokenHandler(this IHttpClientBuilder builder)
        {
            builder.Services.AddTransient<AuthTokenDelegatingHandler>();

            builder.AddHttpMessageHandler<AuthTokenDelegatingHandler>();
            return builder;

        }
    }
}
