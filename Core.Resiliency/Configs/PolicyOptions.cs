using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Resiliency.Configs
{
    public class PolicyOptions : ICircuitBreakerPolicyOptions, ITimeoutPolicyOptions
    {
        public int TimeoutDuration { get; set; }
        public int RetryCount { get; set; }
        public int BreakDuration { get; set; }
    }
}
