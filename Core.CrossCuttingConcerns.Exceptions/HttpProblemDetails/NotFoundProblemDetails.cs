using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails
{
    public class NotFoundProblemDetails : ProblemDetails
    {
        public NotFoundProblemDetails(string detail)
        {
            Title = "Not Found";
            Detail = detail;
            Status = StatusCodes.Status404NotFound;
            Type = "https://example.com/probs/notfound";
        }
    }
}
