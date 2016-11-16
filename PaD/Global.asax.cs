using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using PaD.DataContexts;
using Fooz.Logging;

namespace PaD
{
    public class MvcApplication : System.Web.HttpApplication //Ninject.Web.Common.NinjectHttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Set up automatic migrations
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<IdentityDb, PaD.DataContexts.IdentityMigrations.Configuration>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PaDDb, PaD.DataContexts.PaDMigrations.Configuration>());
        }

        protected void Application_Error()
        {
            ILoggerProvider logger = (ILoggerProvider)DependencyResolver.Current.GetService(typeof(ILoggerProvider));
            var exception = Server.GetLastError();

            logger.Log(exception);

            //HttpContext httpContext = HttpContext.Current;
            //if (httpContext != null)
            //{
            //    RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
            //    /* when the request is ajax the system can automatically handle a mistake with a JSON response. then overwrites the default response */
            //    if (requestContext.HttpContext.Request.IsAjaxRequest())
            //    {
            //        httpContext.Response.Clear();
            //        string controllerName = requestContext.RouteData.GetRequiredString("controller");
            //        IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            //        IController controller = factory.CreateController(requestContext, controllerName);
            //        ControllerContext controllerContext = new ControllerContext(requestContext, (ControllerBase)controller);

            //        JsonResult jsonResult = new JsonResult();
            //        jsonResult.Data = new { success = false, serverError = "500" };
            //        jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //        jsonResult.ExecuteResult(controllerContext);
            //        httpContext.Response.End();
            //    }
            //    else
            //    {
            //        httpContext.Response.Redirect("~/Error");
            //    }
            //}
        }
    }
}
