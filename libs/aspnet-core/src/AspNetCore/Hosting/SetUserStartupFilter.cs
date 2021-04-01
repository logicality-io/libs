using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Logicality.AspNetCore.Hosting
{
    /// <summary>
    /// Provides a mechanism to set the User on the HttpContext. Primary use case is for
    /// injecting a user into an aspnet core application to test or satisfy authorization
    /// concerns.
    /// </summary>
    public class SetUserStartupFilter : IStartupFilter
    {
        private readonly Func<HttpContext, ClaimsPrincipal> _getClaimsPrincipal;

        /// <summary>
        /// Creates a new instance of <see cref="SetUserStartupFilter"/>
        /// </summary>
        /// <param name="getClaimsPrincipal">A func to get a claims principal. Called on each request.</param>
        public SetUserStartupFilter(Func<HttpContext, ClaimsPrincipal> getClaimsPrincipal)
        {
            _getClaimsPrincipal = getClaimsPrincipal;
        }

        ///<inheritdoc/>
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.Use((context, next2) =>
                {
                    var user = _getClaimsPrincipal(context);
                    context.User = user;
                    return next2();
                });
                next(builder);
            };
        }
    }
}
