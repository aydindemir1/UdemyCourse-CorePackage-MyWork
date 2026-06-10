using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Command
{
    public interface IInternalCommand : ICommand
    {
        Guid Id { get; }
        DateTimeOffset OccuredOn { get; }
        string CommandType { get; }
    }
}
