using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Command
{
    public interface IDeleteCommand<out TResponse> : ICommand<TResponse>
   where TResponse : notnull
    {
    }
    public interface IDeleteCommand : ICommand<Unit> { }
}
