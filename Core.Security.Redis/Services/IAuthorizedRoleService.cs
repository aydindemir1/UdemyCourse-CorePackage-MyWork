using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Redis.Services
{
    public interface IAuthorizedRoleService
    {
        Task<List<string>> GetRolesAsync(string userId);
        Task AddRolesAsync(string userId, IReadOnlyList<string> roles);
        Task DeleteRolesAsync(string userId);
        Task ClearAsync();
    }
}
