using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Resiliency.Retry
{
    public static class HttpPolicyBuilders
    {
        public static PolicyBuilder<HttpResponseMessage> GetBaseBuilder()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable);
        }
    }
}
