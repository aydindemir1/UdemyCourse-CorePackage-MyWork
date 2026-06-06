using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.EmailAuthenticator
{
    public interface IEmailAuthenticator
    {
        Task<string> CreateEmailActivationKey();

        Task<string> CreateEmailActivationCode();
    }
}
