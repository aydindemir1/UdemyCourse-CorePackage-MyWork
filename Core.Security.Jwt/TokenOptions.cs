using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Jwt
{
    public class TokenOptions
    {
        public string Audience { get; set; } //example.com -- my-api-client
        public string Issuer { get; set; }  // my-auth-server
        public int AccessTokenExpiration { get; set; } // 60 (minutes)
        public string SecurityKey { get; set; } // secret key for signing the token
        public int RefreshTokenTTL { get; set; }

        public TokenOptions()
        {
            Audience = string.Empty;
            Issuer = string.Empty;
            SecurityKey = string.Empty;
        }

        public TokenOptions(string audience, string issuer, int accessTokenExpiration, string securityKey, int refreshTokenTTL)
        {
            Audience = audience;
            Issuer = issuer;
            AccessTokenExpiration = accessTokenExpiration;
            SecurityKey = securityKey;
            RefreshTokenTTL = refreshTokenTTL;
        }
    }
}
