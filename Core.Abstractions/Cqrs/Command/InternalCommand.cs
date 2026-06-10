using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Command
{
    public abstract class InternalCommand : IInternalCommand
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public DateTimeOffset OccuredOn { get; protected set; } = DateTimeOffset.UtcNow;

        public string CommandType => GetType().AssemblyQualifiedName;
    }
}
