using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain
{
    public interface IRedisEntity
    {
        string GetRedisKey();
    }
}
