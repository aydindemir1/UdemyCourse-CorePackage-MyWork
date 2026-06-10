using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Resiliency.Configs
{
    public interface ITimeoutPolicyOptions
    {
        int TimeoutDuration { get; set; }
    }
}
