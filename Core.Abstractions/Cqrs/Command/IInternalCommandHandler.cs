using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Command
{
    public interface IInternalCommandHandler<in TCommand> : IRequestHandler<TCommand, Unit>
    where TCommand : IInternalCommand
    {

    }
}
