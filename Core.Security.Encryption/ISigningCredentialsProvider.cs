using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Encryption
{
    public interface ISigningCredentialsProvider
    {
        SigningCredentials GetSigningCredentials();
    }
}
