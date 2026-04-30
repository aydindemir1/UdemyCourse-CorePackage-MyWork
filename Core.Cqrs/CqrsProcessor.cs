using Core.Abstractions.Cqrs;
using Core.Abstractions.Cqrs.Command;
using Core.Abstractions.Cqrs.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Cqrs
{
    public class CqrsProcessor : ICqrsProcessor
    {
        private readonly IMediator _mediator;

        public CqrsProcessor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public  Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellation = default)
        {
            return _mediator.Send(command, cancellation);
        }

        public Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellation = default)
        {
            return _mediator.Send(query, cancellation);
        }
    }
}
