using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Command
{
    public interface ICreateCommand<out TResponse> : ICommand<TResponse>
    where TResponse : notnull
    {
    }

    public interface ICreateCommand : ICommand<Unit> { }
}
