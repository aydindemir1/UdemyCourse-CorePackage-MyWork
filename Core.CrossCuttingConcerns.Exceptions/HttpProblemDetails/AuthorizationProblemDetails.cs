using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails
{
    public class AuthorizationProblemDetails : ProblemDetails
    {
        public AuthorizationProblemDetails(string detail)
        {
            Title = "Authorization error";
            Detail = detail;
            Status = StatusCodes.Status401Unauthorized;
            Type = "https://example.com/probs/authorization";
        }
    }
}
