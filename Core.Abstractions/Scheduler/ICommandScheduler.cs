using Core.Abstractions.Cqrs.Command;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Abstractions.Scheduler
{
    public interface ICommandScheduler
    {
        Task ScheduleAsync<TCommand>(TCommand command, DateTimeOffset scheduleAt, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand;

        Task ScheduleAsync<TCommand>(TCommand command, TimeSpan delay, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand;


        Task ScheduleRecurringAsync<TCommand>(TCommand command, string name, string cronExpression, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand;

        Task ScheduleAsync(Expression<Func<Task>> methodCall, DateTimeOffset scheduleAt, string? description = null, CancellationToken cancellationToken = default);

        Task ScheduleAsync(IInternalCommand command, CancellationToken cancellationToken = default);

        Task ScheduleAsync(IInternalCommand[] commands, CancellationToken cancellationToken = default);
    }
}
