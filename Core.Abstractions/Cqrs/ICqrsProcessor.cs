using Core.Abstractions.Cqrs.Command;
using Core.Abstractions.Cqrs.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs
{
    public interface ICqrsProcessor
    {
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellation = default);
        Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellation = default);
    }
}
