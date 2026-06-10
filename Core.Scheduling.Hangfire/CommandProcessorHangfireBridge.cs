using Core.Abstractions.Cqrs;
using Core.Abstractions.Cqrs.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Scheduling.Hangfire
{
    public class CommandProcessorHangfireBridge
    {
        private readonly ICqrsProcessor _cqrsProcessor;

        public CommandProcessorHangfireBridge(ICqrsProcessor cqrsProcessor)
        {
            _cqrsProcessor = cqrsProcessor;
        }

        public Task Send(string jobName, IInternalCommand command)
        {
            return _cqrsProcessor.SendAsync(command);
        }

        public Task Send(IInternalCommand command, string description = "")
        {
            return _cqrsProcessor.SendAsync(command);
        }
    }
}
