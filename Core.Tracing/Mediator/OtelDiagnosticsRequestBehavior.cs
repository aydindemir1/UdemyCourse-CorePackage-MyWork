using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Core.Tracing.Mediator
{
    public class OtelDiagnosticsRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<OtelDiagnosticsRequestBehavior<TRequest, TResponse>> _logger;

        private static readonly ActivitySource _activitySource = new(OtelMediatrOptions.OtelMediatrName);
        public OtelDiagnosticsRequestBehavior(IHttpContextAccessor httpContextAccessor, ILogger<OtelDiagnosticsRequestBehavior<TRequest, TResponse>> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var handlerName = $"{typeof(TRequest).Name}Handler";

            var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;

            const string prefix = nameof(OtelDiagnosticsRequestBehavior<TRequest, TResponse>);

            _logger.LogInformation("[{Prefix}:{Handler}] Executing request {Request} with TraceId={TraceId}", prefix, handlerName, typeof(TRequest).Name, traceId);

            using var activity = _activitySource.StartActivity($"{OtelMediatrOptions.OtelMediatrName}.{handlerName}", ActivityKind.Internal);

            activity?.AddTag("request.type", typeof(TRequest).FullName)?
                .AddTag("response.type", typeof(TResponse).FullName)?
                .AddTag("trace.id", traceId);


            try
            {
                var result = await next();

                activity?.AddEvent(new ActivityEvent("HandledSuccessfully"));

                return result;
            }
            catch (Exception ex)
            {
                activity?.RecordException(ex);

                _logger.LogError(ex, "[{Prefix}:{Handler}] Exception occurred: {Error}",
                    prefix, handlerName, ex.Message);

                throw;
            }
        }
    }
}
