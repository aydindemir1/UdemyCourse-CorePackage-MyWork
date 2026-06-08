using Core.Abstractions.Repositories.Redis;
using Core.Security.Redis.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Redis.Repositories
{
    public interface IAuthorizedRoleRepository : IRedisRepository<UserRole>
    {
    }
}
