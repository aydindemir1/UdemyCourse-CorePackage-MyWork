using Core.Security.Redis.Entities;
using Core.Security.Redis.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Redis.Services
{
    public class AuthorizedRoleManager : IAuthorizedRoleService
    {

        private readonly IAuthorizedRoleRepository _repository;

        public AuthorizedRoleManager(IAuthorizedRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<string>> GetRolesAsync(string userId)
        {
            var key = new UserRole() { UserId = userId }.GetRedisKey();
            var userRoles = await _repository.GetAsync(key);
            return userRoles?.Roles ?? new List<string>();
        }

        public async Task AddRolesAsync(string userId, IReadOnlyList<string> roles)
        {
            UserRole role = new UserRole(userId, roles.ToList());
            await _repository.SetAsync(role);
        }

        public async Task DeleteRolesAsync(string userId)
        {
            var key = new UserRole() { UserId = userId }.GetRedisKey();
            await _repository.DeleteAsync(key);
        }

        public async Task ClearAsync()
        {
            var roles = await _repository.GetAllAsync("user_roles:*");
            foreach (var role in roles)
            {
                await _repository.DeleteAsync(role.GetRedisKey());
            }
        }
    }
}
