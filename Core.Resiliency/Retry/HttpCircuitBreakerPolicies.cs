using Core.Resiliency.Configs;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Resiliency.Retry
{
    public class HttpCircuitBreakerPolicies
    {
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy(ILogger logger, ICircuitBreakerPolicyOptions circuitBreakerPolicyOptions)
        {
            return HttpPolicyBuilders.GetBaseBuilder().CircuitBreakerAsync(circuitBreakerPolicyOptions.RetryCount, TimeSpan.FromSeconds(circuitBreakerPolicyOptions.BreakDuration), (result, breakDuration) =>
            {
                OnHttpBreak(result, breakDuration, circuitBreakerPolicyOptions.RetryCount, logger);
            },
            () =>
            {
                OnHttpReset(logger);
            });
        }

        private static void OnHttpBreak(DelegateResult<HttpResponseMessage> result, TimeSpan breakDuration, int retryCount, ILogger logger)
        {
            logger.LogInformation("Service closed due to continuous errors (circuit breaker). Requests will not be accepted during {BreakDuration}. Error Threshold: {RetryCount}", breakDuration, retryCount);
        }

        public static void OnHttpReset(ILogger logger)
        {
            logger.LogInformation("Service Reopened (Circuit breaker). Traffic flow has returned to normal");
        }
    }
}
