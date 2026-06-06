using Core.Security.Domain.Entities;
using Core.Security.Encryption;
using Core.Security.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security.Jwt
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly TokenOptions _tokenOptions;
        private readonly ISigningCredentialsProvider _signingCredentialsProvider;
        private DateTime _accessTokenExpiration;

        public JwtTokenGenerator(IConfiguration configuration, ISigningCredentialsProvider signingCredentialsProvider)
        {
            _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>() ?? throw new InvalidOperationException("TokenOptions configuration section is missing or invalid.");
            _signingCredentialsProvider = signingCredentialsProvider ?? throw new ArgumentNullException(nameof(signingCredentialsProvider));
        }

        public AccessToken CreateToken(User user)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            SigningCredentials signingCredentials = _signingCredentialsProvider.GetSigningCredentials();

            JwtSecurityToken jwtSecurityToken = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(jwtSecurityToken);

            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public RefreshToken CreateRefreshToken(User user, string ipAddress)
        {
            return new RefreshToken
            {
                UserId = user.Id,
                Token = RandomRefreshToken(),
                Expires = DateTime.Now.AddDays(_tokenOptions.RefreshTokenTTL),
                CreatedByIp = ipAddress
            };
        }

        private JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials)
        {
            return new JwtSecurityToken(issuer: tokenOptions.Issuer, audience: tokenOptions.Audience, expires: _accessTokenExpiration, notBefore: DateTime.Now, claims: SetClaims(user), signingCredentials: signingCredentials);
        }

        private IEnumerable<Claim> SetClaims(User user)
        {
            List<Claim> claims = new List<Claim>();
            claims.AddNameIdentifier(user.Id.ToString());
            claims.AddEmail(user.Email);
            return claims;
        }

        private string RandomRefreshToken()
        {
            byte[] numberByte = new byte[32];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }


    }
}
