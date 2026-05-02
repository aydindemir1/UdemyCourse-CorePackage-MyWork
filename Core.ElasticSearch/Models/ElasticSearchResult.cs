using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch.Models
{
    public class ElasticSearchResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public static ElasticSearchResult SuccessResult(string msg = "Success") =>
            new() { Success = true, Message = msg };

        public static ElasticSearchResult Fail(string msg) =>
            new() { Success = false, Message = msg };

        public static ElasticSearchResult FromResponse(IResponse response) =>
            response.IsValid ? SuccessResult()
                : Fail(response.ServerError?.Error.Reason ?? "Unknown error");
    }
}
