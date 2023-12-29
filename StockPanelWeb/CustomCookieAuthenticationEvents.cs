using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace StockPanelWeb
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {

            if (context.Request.Path.StartsWithSegments("/Access") &&
                context.Request.Cookies.ContainsKey(".AspNetCore.Application.Id"))
            {
                // The user is already authenticated with a valid cookie.
                // Skip the redirection to the login page and continue with the request.
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            // The user is not authenticated, perform the default redirection to the login page.
            context.RedirectUri = "/Access/Login";
            return base.RedirectToLogin(context);
        }
    }
}



