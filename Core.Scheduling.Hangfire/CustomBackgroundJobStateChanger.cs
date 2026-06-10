using Hangfire.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Scheduling.Hangfire
{
    public class CustomBackgroundJobStateChanger : IBackgroundJobStateChanger
    {
        private readonly IBackgroundJobStateChanger _inner;

        private readonly ILogger<CustomBackgroundJobStateChanger> _logger;

        public CustomBackgroundJobStateChanger(IBackgroundJobStateChanger inner, ILogger<CustomBackgroundJobStateChanger> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public IState ChangeState(StateChangeContext context)
        {
            _logger.LogInformation("[Hangfire] Job State Change ID : {JobId} New State : {State}", context.BackgroundJobId, context.NewState.Name);
            return _inner.ChangeState(context);
        }
    }
}
