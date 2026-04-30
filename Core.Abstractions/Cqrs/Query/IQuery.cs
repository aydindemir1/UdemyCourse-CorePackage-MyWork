using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Query
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
     where TResponse : notnull
    {
    }
}
