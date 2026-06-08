using Core.Abstractions.Cqrs;
using Core.Security.Domain.Entities;
using Core.Security.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;


namespace Core.WebApi
{
    [Route("api/controller")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private ICqrsProcessor _cqrsProcessor;

        protected ICqrsProcessor CqrsProcessor => _cqrsProcessor ??= HttpContext.RequestServices.GetService<ICqrsProcessor>() ?? throw new InvalidOperationException("ICqrsProcessor cannot be retrieved from request services");


        protected string getIpAddress()
        {
            string ipAddress = Request.Headers.ContainsKey("x-Forwarded-For") ? Request.Headers["x-Forwarded-For"].ToString() : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
            return ipAddress;
        }

        protected void setRefreshTokenFromCookie(RefreshToken refreshToken)
        {
            CookieOptions cookieOptions = new CookieOptions() { HttpOnly = true, Expires = refreshToken.Expires, Secure = true, SameSite = SameSiteMode.Strict };
            Response.Cookies.Append(key: "refreshToken", refreshToken.Token, cookieOptions);
        }

        protected string getRefreshTokenFromCookie()
        {
            return Request.Cookies["refreshToken"] ?? throw new ArgumentException("Refresh token is not found");
        }

        protected Guid getUserIdFromRequest()
        {
            var userId = HttpContext.User.GetUserId();
            return userId;
        }
    }
}
