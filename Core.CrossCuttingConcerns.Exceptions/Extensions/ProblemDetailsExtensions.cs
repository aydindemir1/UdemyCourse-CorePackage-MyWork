using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Core.CrossCuttingConcerns.Exceptions.Extensions
{
    public static class ProblemDetailsExtensions
    {
        public static string AsJson<TProblemDetails>(this TProblemDetails problemDetails)
            where TProblemDetails : ProblemDetails => JsonSerializer.Serialize(problemDetails);
    }
}
