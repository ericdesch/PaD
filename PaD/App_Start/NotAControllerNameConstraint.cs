using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using PaD.Infrastructure;

namespace PaD
{
    public class NotAControllerNameConstraint : IRouteConstraint
    {
        private const string CONTROLLER_NAMES = "ApplicationState_ControllerNames";

        // Match only if the username in the route is NOT a Controller name (Home, Account, etc).
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string value = values[parameterName].ToString().ToLowerInvariant();

            List<string> controllerNames = ControllerNames.Get();
            var exists = controllerNames.Contains(value);

            return !exists;
        }

    }
}