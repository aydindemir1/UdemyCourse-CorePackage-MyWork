using Hangfire;
using Hangfire.Client;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Scheduling.Hangfire
{
    public class CustomBackgroundJobFactory : IBackgroundJobFactory
    {
        private readonly IBackgroundJobFactory _inner;

        private readonly ILogger<CustomBackgroundJobFactory> _logger;

        public CustomBackgroundJobFactory(IBackgroundJobFactory inner, ILogger<CustomBackgroundJobFactory> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public IStateMachine StateMachine => _inner.StateMachine;

        public BackgroundJob Create(CreateContext context)
        {
            _logger.LogInformation("[Hangfire] Job Create {JobType}.{Method} | State : {State}", context.Job.Type.FullName, context.Job.Method.Name, context.InitialState?.Name);
            return _inner.Create(context);
        }
    }
}
