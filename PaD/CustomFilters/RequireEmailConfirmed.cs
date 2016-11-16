using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;

using PaD.DAL;
using PaD.DataContexts;

namespace PaD.CustomFilters
{
    public class RequireEmailConfirmed : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //var isAuthorized = base.AuthorizeCore(httpContext);
            //if (!isAuthorized)
            //{
            //    return false;
            //}

            // Get the userName parameter for the project that they are trying to view.
            string userName = httpContext.Request.RequestContext.RouteData.Values["userName"] as string;

            // Only show projects for users who have confirmed their accounts. Users can register and log in
            // and add photos to their project, but no one other than themselves can view their project until
            // they confirm their email.
            if (httpContext.User == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
            {
                // If they are not logged in, they can only view this project if the userName is for an authenticated account.
                UserManager userManager = new UserManager();
                if (!userManager.EmailConfirmed(userName))
                {
                    return false;
                }
            }
            else
            {
                // If they are logged in, they can only see this project if it is their own project.
                if (httpContext.User.Identity.Name != userName)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Error",
                                action = "Unconfirmed"
                            })
                        );
        }
    }
}