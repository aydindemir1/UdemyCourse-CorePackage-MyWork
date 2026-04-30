using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Cqrs.Query
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
    { }
}
