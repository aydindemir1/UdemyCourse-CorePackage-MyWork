using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Scheduling.Hangfire
{
    public class CustomBackgroundJobPerformer : IBackgroundJobPerformer
    {
        private readonly IBackgroundJobPerformer _inner;

        private readonly ILogger<CustomBackgroundJobPerformer> _logger;

        public CustomBackgroundJobPerformer(IBackgroundJobPerformer inner, ILogger<CustomBackgroundJobPerformer> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public object Perform(PerformContext context)
        {
            _logger.LogInformation("[Hangfire] Job Perform ID : {JobId} | {JobType}.{Method}", context.BackgroundJob.Id, context.BackgroundJob.Job.Type.FullName, context.BackgroundJob.Job.Method.Name);
            return _inner.Perform(context);
        }
    }
}
