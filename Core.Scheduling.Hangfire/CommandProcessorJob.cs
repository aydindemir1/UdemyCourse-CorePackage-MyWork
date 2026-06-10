using Core.Abstractions.Cqrs;
using Core.Abstractions.Scheduler;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Scheduling.Hangfire
{
    public class CommandProcessorJob
    {
        private readonly ICqrsProcessor _cqrsProcessor;

        public CommandProcessorJob(ICqrsProcessor cqrsProcessor)
        {
            _cqrsProcessor = cqrsProcessor;
        }

        public async Task Execute(SchedulerSerializedObject schedulerSerializedObject)
        {
            if (schedulerSerializedObject == null)
                return;
            await _cqrsProcessor.SendSchedulerObject(schedulerSerializedObject);
        }
    }
}
