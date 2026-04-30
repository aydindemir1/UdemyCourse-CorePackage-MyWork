using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Command
{
    public interface IUpdateCommand<out TResponse> : ICommand<TResponse>
   where TResponse : notnull
    {
    }
    public interface IUpdateCommand : ICommand<Unit> { }
}
