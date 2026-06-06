using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Core.Security.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddEmail(this ICollection<Claim> claims, string email) =>
            claims.Add(new Claim(ClaimTypes.Email, email));

        public static void AddNameIdentifier(this ICollection<Claim> claims, string nameIdentifier) =>
            claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));

    }
}
