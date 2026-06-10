using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Scheduler
{
    public interface IScheduler : ICommandScheduler
    {
        Task ScheduleAsync(SchedulerSerializedObject serializedObject, DateTimeOffset scheduleAt, string? description = null);

        Task ScheduleAsync(SchedulerSerializedObject serializedObject, TimeSpan delay, string? description = null);

        Task ScheduleRecurringAsync(SchedulerSerializedObject serializedObject, string name, string cronExpression, string? description = null);

        Task RemoveSchedulerAsync(string scheduleId);

        Task<bool> ExistsAsync(string scheduleId);

    }
}
