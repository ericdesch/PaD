using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PaD
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Force URLs to be lowercase
            routes.LowercaseUrls = true;

            // Match /{username} routes
            routes.MapRoute(
                name: "MonthView",
                url: "{username}/{year}/{month}",
                defaults: new
                {
                    controller = "Month",
                    action = "Index",
                    year = UrlParameter.Optional,
                    month = UrlParameter.Optional
                },
                constraints: new
                {
                    username = new NotAControllerNameConstraint(), // Make sure the username is not a controller name (Home, Account, Photos, etc)
                    year = @"\d*", // The * matches 0 or more or decimal values making it optional. UrlParameter.Optional does not work for this.
                    month = @"\d*"
                }
            );

            routes.MapRoute(
                name: "DayView",
                url: "{username}/{year}/{month}/{day}",
                defaults: new
                {
                    username = new NotAControllerNameConstraint(),
                    controller = "Day",
                    action = "Index"
                },
                constraints: new
                {
                    year = @"\d+",
                    month = @"\d+",
                    day = @"\d+"
                }
            );

            // Match routes for logged-in users to add/edit/delete
            routes.MapRoute(
                name: "ControllerByDate",
                url: "{controller}/{action}/{year}/{month}/{day}",
                defaults: new
                {
                    year = UrlParameter.Optional,
                    month = UrlParameter.Optional,
                    day = UrlParameter.Optional
                },
                constraints: new
                {
                    year = @"\d*", // The * matches 0 or more or decimal values making it optional. UrlParameter.Optional does not work for this.
                    month = @"\d*",
                    day = @"\d*"
                }
            );

            // Default route.
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

        }
    }
}
