using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Tracing
{
    public class OpenTelemetryOptions
    {
        public IEnumerable<string> Services { get; set; }

        public JaegerOptions JaegerOptions { get; set; }

        public bool Enabled { get; set; }

        public bool AlwaysOnSampler { get; set; } = true;
    }
}
