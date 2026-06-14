using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Core.Tracing
{
    public static class ActivityExtensions
    {
        public static void RecordException(this Activity activity, Exception exception)
        {
            if (activity == null || exception == null) return;

            activity?.AddTag("otel.status_code", "ERROR");

            activity?.AddTag("otel.status_description", exception.Message);

            activity?.AddEvent(new ActivityEvent("exception", default, new ActivityTagsCollection
        {
            {"exception.type",exception.GetType().FullName },

            {"exception.message",exception.Message},

            {"exception.stacktrace",exception.StackTrace ?? "N/A" }
        }));
        }
    }
}
