using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Repositories
{
    public interface IQuery<T>
    {
        IQueryable<T> Query();
    }
}
