using PaD.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;

namespace PaD
{
    public class UserNameContraint : IRouteConstraint
    {
        private const string USERNAMES = "ApplicationState_UserNames";

        // Match only if the username passed in the route is an actual user in the database.
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string value = values[parameterName].ToString().ToLowerInvariant();

            List<string> userNames = GetUserNames();
            var exists = userNames.Contains(value);

            return exists;
        }

        private List<string> GetUserNames()
        {
            List<string> userNames = new List<string>();

            // If the list is already in the Application object, get it.
            // Otherwise build the list and store it in the Application object for future calls.
            if (HttpContext.Current.Application[USERNAMES] != null)
            {
                userNames = (List<string>)HttpContext.Current.Application[USERNAMES];
            }
            else
            {
                var users = Membership.GetAllUsers();

                // For each type that is a subtype of Controller, add it to the list.
                foreach (MembershipUser user in users)
                {
                    userNames.Add(user.UserName.ToLowerInvariant());
                }

                HttpContext.Current.Application[USERNAMES] = userNames;
            }

            return userNames;

        }
    }
}