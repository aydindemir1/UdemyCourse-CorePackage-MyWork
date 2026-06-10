using Core.Abstractions.Cqrs.Command;
using Core.Abstractions.Scheduler;
using Hangfire;

namespace Core.Scheduling.Hangfire.Scheduler
{
    public interface IHangfireScheduler : IScheduler
    {
        string Enqueue<T>(T command, string parentJobId, JobContinuationOptions jobContinuationOptions, string? description = null)
            where T : IInternalCommand;

        string Enqueue(SchedulerSerializedObject schedulerSerializedObject, string parentJobId, JobContinuationOptions jobContinuationOptions, string? description = null);
    }
}
