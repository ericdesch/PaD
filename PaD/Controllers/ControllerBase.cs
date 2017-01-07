using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;

using Ninject;
using Fooz.Logging;
using Fooz.Caching;

using PaD.DAL;
using PaD.DataContexts;
using PaD.Infrastructure;
using PaD.ViewModels;
using PaD.CustomFilters;

namespace PaD.Controllers
{
    //[RemoteRequireHttps]
    public abstract class ControllerBase : Controller
    {
        private const string DEFAULT_PROJECT_ID = "DEFAULT_PROJECT_ID";

        protected readonly IDbContext DatabaseContext;
        protected readonly ILoggerProvider Logger;
        protected readonly ICacheProvider Cache;

        public int UserDefaultProjectId
        { 
            get
            {
                string key = GetKey();
                int projId = 0;

                // Store the user's defualt project id in the Session object.
                // Try to get it from the Session object.
                if (Session[key] == null || !int.TryParse(Session[key].ToString(), out projId))
                {
                    // If not already stored in Session or unable to parse it, get it from the database
                    ProjectManager projectManager = new ProjectManager(DatabaseContext, Logger, Cache);
                    projId = projectManager.GetDefaultProjectId(User.Identity.Name);

                    // Store the defulat project id to the Session object for future retrieval.
                    Session[key] = projId;
                }

                return projId;
            }
            set 
            {
                // Add it to the Session object
                string key = GetKey();
                Session[key] = value;
            }
        }

        // Constructor that takes an IDbContext, ILogger, and an ICacheProvider. NInject will create instances for us.
        public ControllerBase(IDbContext dbContext, ILoggerProvider loggerProvider, ICacheProvider cacheProvider)
        {
            DatabaseContext = dbContext;
            Logger = loggerProvider;
            Cache = cacheProvider;
        }

        //public ControllerBase()
        //{
        //    // Get the class that ILogger, ICacheProvider resolves to as bound in NinjectDependencyResolver.
        //    // Do it this way instead of using constructor injection because we don't want the
        //    // controller to have to know anything about contexts, etc.
        //    Logger = (ILoggerProvider)DependencyResolver.Current.GetService(typeof(ILoggerProvider));
        //    Cache = (ICacheProvider)DependencyResolver.Current.GetService(typeof(ICacheProvider));
        //}

        private string GetKey()
        {
            return User.Identity.Name + DEFAULT_PROJECT_ID;
        }

    }
}