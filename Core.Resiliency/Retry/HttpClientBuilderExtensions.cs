using Core.Resiliency.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Resiliency.Retry
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddCustomPolicyHandlers(this IHttpClientBuilder builder, Func<IHttpClientBuilder, IHttpClientBuilder>? policyHandlerBuilder = null)
        {

            AsyncPolicyWrap<HttpResponseMessage> policy = null;

            var result = builder.AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IConfiguration>().GetSection("PolicyOptions").Get<PolicyOptions>();

                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                var retryLogger = loggerFactory.CreateLogger("PollyHttpRetryPoliciesLogger");

                var cbLogger = loggerFactory.CreateLogger("PollyHttpCircuitBreakerPoliciesLogger");

                var retryPolicy = HttpRetryPolicies.GetHttpRetryPolicy(retryLogger, options);

                var circuitBreakerPolicy = HttpCircuitBreakerPolicies.GetHttpCircuitBreakerPolicy(cbLogger, options);

                policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
                return policy;
            });

            if (policyHandlerBuilder is not null)
                result = policyHandlerBuilder.Invoke(result);
            return result;

        }
    }
}
