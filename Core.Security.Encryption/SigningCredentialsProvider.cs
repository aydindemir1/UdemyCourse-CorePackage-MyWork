using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Encryption
{
    public class SigningCredentialsProvider : ISigningCredentialsProvider
    {
        private readonly IConfiguration _configuration;

        public SigningCredentialsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SigningCredentials GetSigningCredentials()
        {
            var securityKeyString = _configuration.GetValue<string>("TokenOptions:SecurityKey");
            if (string.IsNullOrEmpty(securityKeyString))
                throw new InvalidOperationException("Security key is not configured.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyString));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        }
    }
}
