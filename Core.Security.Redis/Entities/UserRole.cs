using Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Redis.Entities
{
    public class UserRole : IRedisEntity
    {
        public string UserId { get; set; }
        public List<string> Roles { get; set; }

        public UserRole()
        {
            Roles = new List<string>();
        }

        public UserRole(string userId, List<string> roles) : this()
        {
            UserId = userId;
            Roles = roles;
        }

        public string GetRedisKey() => $"user_roles:{UserId}";
    }
}
