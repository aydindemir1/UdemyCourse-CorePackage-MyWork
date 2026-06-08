using Core.Persistence.Repositories.Redis;
using Core.Security.Redis.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Redis.Repositories
{
    public class AuthorizedRoleRepository : RedisRepositoryBase<UserRole>, IAuthorizedRoleRepository
    {
        public AuthorizedRoleRepository(IConnectionMultiplexer redis) : base(redis)
        {
        }
    }
}
