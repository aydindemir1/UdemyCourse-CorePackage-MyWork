using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Tracing
{
    public class Constants
    {
        public const string CorrelationContextHeaderName = "Correlation-Context";

        public const string TraceParentHeaderName = "traceparent";

        public const string RequestIdHeaderName = "request-id";

        public const string TraceStateHeaderName = "tracestate";
    }
}
