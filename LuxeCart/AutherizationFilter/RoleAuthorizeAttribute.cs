using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LuxeCart.AutherizationFilter
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _allowedRoles = roles ?? throw new ArgumentNullException(nameof(roles));
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // Check if the user is authenticated
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            // Get the authentication cookie
            var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
            {
                return false;
            }

            try
            {
                // Decrypt the authentication ticket
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket == null)
                {
                    return false;
                }

                // Get the user's role from the ticket's UserData
                //var userRole = authTicket.UserData;
                if(authTicket.UserData == "Admin")
                {
                    var userRole = "Admin";
                    return _allowedRoles.Contains(userRole);
                }
                else
                {
                    var userRole = "User";
                    return _allowedRoles.Contains(userRole);
                }
                
            }
            catch (Exception)
            {
                // Log the exception (if needed) and deny access
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Redirect to the login page with the ReturnUrl parameter
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // If the user is authenticated but not authorized, show an access denied page
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/AccessDenied.cshtml" // Create this view
                };
            }
            else
            {
                // If the user is not authenticated, redirect to the login page
                var returnUrl = filterContext.HttpContext.Request.Url?.PathAndQuery;
                var loginUrl = FormsAuthentication.LoginUrl + "?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl);
                filterContext.Result = new RedirectResult(loginUrl);
            }
        }
    }
}