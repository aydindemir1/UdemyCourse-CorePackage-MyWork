using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Command
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
    {
    }

    public interface ICommand : ICommand<Unit> { }
}
