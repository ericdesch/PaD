using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;

using Ninject;
using Fooz.Logging;
using Fooz.Caching;

using PaD.DAL;
using PaD.Infrastructure;
using PaD.DataContexts;

namespace PaD.CustomFilters
{
    public class TrackPageViewAttribute : ActionFilterAttribute
    {
        #region Member Variables
        private readonly IDbContext _databaseContext;
        private readonly ILoggerProvider _logger;
        private readonly ICacheProvider _cache;
        #endregion

        #region Constructors
        //public TrackPageViewAttribute()
        //{
        //    // Get the classes that IDbContext, ILogger, and ICacheProvider resolve to as bound in NinjectDependencyResolver.
        //    _databaseContext = (IDbContext)DependencyResolver.Current.GetService(typeof(IDbContext));
        //    _logger = (ILoggerProvider)DependencyResolver.Current.GetService(typeof(ILoggerProvider));
        //    _cache = (ICacheProvider)DependencyResolver.Current.GetService(typeof(ICacheProvider));
        //}

        public TrackPageViewAttribute(IDbContext databaseContext, ILoggerProvider logger, ICacheProvider cache)
        {
            // Allow injecting
            _databaseContext = databaseContext;
            _logger = logger;
            _cache = cache;
        }
        #endregion

        #region OnActionExecuting
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            if (controller ==  "Month" && action == "Index")
            {
                TrackProject(filterContext);
            }
            else if (controller == "Day" && action == "Index")
            {
                TrackDay(filterContext);
            }
        }
        #endregion

        #region TrackProject
        private void TrackProject(ActionExecutingContext filterContext)
        {
            // Get the values we care about the route.
            var routeValues = filterContext.RequestContext.RouteData.Values;
            string userName = routeValues["username"] as string;
            int year;
            int.TryParse(routeValues["year"] as string, out year); // year = 0 if this fails
            int month;
            int.TryParse(routeValues["month"] as string, out month);

            // If no date is passed in, get the project's last photo's date
            if (year == 0 && month == 0)
            {
                ProjectManager projectManager = new ProjectManager(_databaseContext, _logger, _cache);
                DateTime date = projectManager.GetLastPhotoDate(userName);

                if (date == DateTime.MinValue)
                {
                    year = DateTime.Now.Year;
                    month = DateTime.Now.Month;
                }
                else
                {
                    year = date.Year;
                    month = date.Month;
                }
            }

            // Track the page
            try
            {
                ProjectTrackerManager tracker = new ProjectTrackerManager(_databaseContext, _logger, _cache);
                tracker.IncrementCounter(userName, year, month);
            }
            catch (Exception ex)
            {
                // Log the exception. Don't worry about re-trying; not worth sacrificing the user experience
                // if we miss a page count.
                _logger.Log(LogLevel.Error, ex, "Unable to track photo view. username:{0}, year:{1}, month:{2}.", userName, year);
            }
        }
        #endregion

        #region TrackDay
        private void TrackDay(ActionExecutingContext filterContext)
        {
            // Get the values we care about the route.
            var routeValues = filterContext.RequestContext.RouteData.Values;
            string userName = routeValues["username"] as string;
            int year;
            int.TryParse(routeValues["year"] as string, out year);
            int month;
            int.TryParse(routeValues["month"] as string, out month);
            int day;
            int.TryParse(routeValues["day"] as string, out day);

            // Track the page
            try
            {
                PhotoTrackerManager tracker = new PhotoTrackerManager(_databaseContext, _logger, _cache);
                tracker.IncrementCounter(userName, year, month, day);
            }
            catch (Exception ex)
            {
                // Log the exception. Don't worry about re-trying; not worth sacrificing the user experience
                // if we miss a page count.
                _logger.Log(LogLevel.Error, ex, "Unable to track photo view. username:{0}, year:{1}, month:{2}, day:{3}.", userName, year, month, day);
            }
        }
        #endregion
    }
}