using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Security.Domain.Constants;
using Core.Security.Extensions;
using Core.Security.Redis.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Pipelines.Authorization
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>, IAuthenticationRequest
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizedRoleService _authorizedRoleService;

        public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor, IAuthorizedRoleService authorizedRoleService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizedRoleService = authorizedRoleService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (!user.Claims.Any())
                throw new AuthorizationException("You are not authenticated");

            if (request is IAuthorizationRequest authorizationRequest)
            {
                var userId = user.GetUserId();
                var userRoles = await _authorizedRoleService.GetRolesAsync(userId.ToString());

                bool isAuthorized = userRoles.Contains(GeneralOperationClaim.Admin) || authorizationRequest.Roles.Any(role => userRoles.Contains(role));

                if (!isAuthorized)
                    throw new AuthorizationException("You are not authorized");
            }

            return await next();
        }
    }
}
