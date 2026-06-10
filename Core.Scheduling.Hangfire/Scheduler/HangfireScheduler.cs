using Core.Abstractions.Cqrs.Command;
using Core.Abstractions.Scheduler;
using Core.Scheduling.Hangfire;
using Core.Scheduling.Hangfire.Scheduler;
using Hangfire;
using Hangfire.Storage;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Core.Scheduling.Hangfire.Scheduler
{
    public class HangfireScheduler : IHangfireScheduler
    {
        public string Enqueue<T>(T command, string parentJobId, JobContinuationOptions jobContinuationOptions, string? description = null) where T : IInternalCommand
        {
            var serialized = SerializedObject(command, description);
            return BackgroundJob.ContinueJobWith<CommandProcessorJob>(parentJobId, job => job.Execute(serialized), jobContinuationOptions);
        }

        public string Enqueue(SchedulerSerializedObject schedulerSerializedObject, string parentJobId, JobContinuationOptions jobContinuationOptions, string? description = null)
        {
            return BackgroundJob.ContinueJobWith<CommandProcessorJob>(parentJobId, job => job.Execute(schedulerSerializedObject), jobContinuationOptions);
        }

        public Task ScheduleAsync(SchedulerSerializedObject serializedObject, DateTimeOffset scheduleAt, string? description = null)
        {
            BackgroundJob.Schedule<CommandProcessorJob>(job => job.Execute(serializedObject), scheduleAt);
            return Task.CompletedTask;
        }

        public Task ScheduleAsync(SchedulerSerializedObject serializedObject, TimeSpan delay, string? description = null)
        {
            var runAt = DateTimeOffset.UtcNow.Add(delay);
            return ScheduleAsync(serializedObject, runAt, description);
        }

        public Task ScheduleAsync<TCommand>(TCommand command, DateTimeOffset scheduleAt, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            var serialized = SerializedObject(command, description);
            BackgroundJob.Schedule<CommandProcessorJob>(job => job.Execute(serialized), scheduleAt);
            return Task.CompletedTask;
        }

        public Task ScheduleAsync<TCommand>(TCommand command, TimeSpan delay, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            var runAt = DateTimeOffset.UtcNow.Add(delay);
            return ScheduleAsync(command, runAt, description, cancellationToken);
        }

        public Task ScheduleAsync(Expression<Func<Task>> methodCall, DateTimeOffset scheduleAt, string? description = null, CancellationToken cancellationToken = default)
        {
            BackgroundJob.Schedule(methodCall, scheduleAt);
            return Task.CompletedTask;
        }

        public Task ScheduleAsync(IInternalCommand command, CancellationToken cancellationToken = default)
        {
            var client = new BackgroundJobClient();
            client.Enqueue<CommandProcessorHangfireBridge>(bridge => bridge.Send(command, string.Empty));
            return Task.CompletedTask;
        }

        public async Task ScheduleAsync(IInternalCommand[] commands, CancellationToken cancellationToken = default)
        {
            foreach (var command in commands)
                await ScheduleAsync(command, cancellationToken);
        }

        public Task ScheduleRecurringAsync(SchedulerSerializedObject serializedObject, string name, string cronExpression, string? description = null)
        {
            RecurringJob.AddOrUpdate<CommandProcessorJob>(name, job => job.Execute(serializedObject), cronExpression, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            return Task.CompletedTask;
        }

        public Task ScheduleRecurringAsync<TCommand>(TCommand command, string name, string cronExpression, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand
        {

            var serialized = SerializedObject(command, description);
            return ScheduleRecurringAsync(serialized, name, cronExpression, description);
        }

        public Task<bool> ExistsAsync(string scheduleId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJobs = connection.GetRecurringJobs();
                bool exists = recurringJobs.Any(job => job.Id == scheduleId);
                return Task.FromResult(exists);
            }
        }

        public Task RemoveSchedulerAsync(string scheduleId)
        {
            BackgroundJob.Delete(scheduleId);
            RecurringJob.RemoveIfExists(scheduleId);
            return Task.CompletedTask;
        }


        private SchedulerSerializedObject SerializedObject(object messageObject, string? description)
        {
            return new SchedulerSerializedObject(typeName: messageObject.GetType().FullName, assemblyName: messageObject.GetType().Assembly.GetName().FullName, data: JsonConvert.SerializeObject(messageObject), description: description);
        }
    }
}
