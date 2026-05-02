using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch
{
    public static class ElasticSearchExtensions
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services)
        {
            services.AddScoped<IElasticSearchService, ElasticSearchService>();
            return services;
        }
    }
}
