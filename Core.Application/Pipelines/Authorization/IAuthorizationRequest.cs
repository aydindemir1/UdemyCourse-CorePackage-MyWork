using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Pipelines.Authorization
{
    public interface IAuthorizationRequest : IAuthenticationRequest
    {
        string[] Roles { get; }
    }
}
