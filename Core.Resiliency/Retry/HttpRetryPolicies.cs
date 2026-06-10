using Core.Resiliency.Configs;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Resiliency.Retry
{
    public static class HttpRetryPolicies
    {
        public static AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy(ILogger logger, ICircuitBreakerPolicyOptions retryPolicyConfig)
        {
            return HttpPolicyBuilders.GetBaseBuilder().WaitAndRetryAsync(retryPolicyConfig.RetryCount, ComputeDuration, (result, timeSpan, retryCount, context) =>
            {
                OnHttpRetry(result, timeSpan, retryCount, context, logger);
            });
        }

        private static void OnHttpRetry(DelegateResult<HttpResponseMessage> result, TimeSpan timeSpan, int retryCount, Context context, ILogger logger)
        {
            if (result.Result != null)
            {
                logger.LogWarning("Retrying HTTP request. Status code: {StatusCode}, Retry count: {RetryCount}", result.Result.StatusCode, retryCount);
            }
            else
            {
                logger.LogWarning("The request failed due to a network error. A second attempt will be made after {TimeSpan} and {Retry Count}.", timeSpan, retryCount);
            }
        }

        private static TimeSpan ComputeDuration(int input)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, input)) + TimeSpan.FromMicroseconds(new Random().Next(0, 100));
        }
    }
}
