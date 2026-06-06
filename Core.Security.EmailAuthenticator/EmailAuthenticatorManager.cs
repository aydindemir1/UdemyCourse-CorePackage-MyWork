using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security.EmailAuthenticator
{
    public class EmailAuthenticatorManager : IEmailAuthenticator
    {
        public Task<string> CreateEmailActivationCode()
        {
            int codeNumber = RandomNumberGenerator.GetInt32(0, 1_000_000);

            string code = codeNumber.ToString().PadLeft(6, '0');
            return Task.FromResult(code);
        }

        public Task<string> CreateEmailActivationKey()
        {
            string key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return Task.FromResult(key);
        }
    }
}
