using Core.Security.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Jwt
{
    public interface ITokenGenerator
    {
        AccessToken CreateToken(User user);

        RefreshToken CreateRefreshToken(User user, string ipAddress);
    }
}
