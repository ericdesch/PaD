using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using Ninject;

using Fooz.Logging;
using Fooz.Caching;

using PaD.Models;
using PaD.DAL.Trackers;
using PaD.ViewModels;
using PaD.DataContexts;
using PaD.Infrastructure;

namespace PaD.DAL
{
    public class ProjectTrackerManager : EntityManagerBase<ProjectTracker>
    {
        #region Constructors
        public ProjectTrackerManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) 
            : base(context, logger, cache)
        { }
        #endregion

        #region IncrementCounter
        public long IncrementCounter(int projectId, int year, int month)
        {
            Logger.Log("ProjectTrackerManager.IncrementCounter({0}, {1}, {2})", projectId, year, month);

            IncrementProjectCounter incrementProjectCounter = new IncrementProjectCounter(DatabaseContext)
            {
                ProjectId = projectId,
                Year = year,
                Month = month
            };

            long newCount = -1;
            try
            {
                newCount = incrementProjectCounter.Execute();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return newCount;
        }

        /// <summary>
        /// Increments the counter for the default project of the passed username for the passed
        /// year and month.
        /// </summary>
        /// <param name="username">The username used to determine the project to track.</param>
        /// <param name="year">Year value to track</param>
        /// <param name="month">Month value to track</param>
        /// <returns>The new count value for the passed parameters or -1 on error</returns>
        public long IncrementCounter(string username, int year, int month)
        {
            Logger.Log("ProjectTrackerManager.IncrementCounter({0}, {1}, {2})", username, year, month);

            // Get the user's default project.
            ProjectManager projectManager = new ProjectManager(DatabaseContext, Logger, Cache);

            try
            {
                var project = projectManager.GetDefault(username);

                // Call the overload that takes projectId as a parameter.
                return IncrementCounter(project.ProjectId, year, month);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return -1;
        }
        #endregion
    }
}