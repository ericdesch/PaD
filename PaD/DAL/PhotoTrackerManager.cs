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
    public class PhotoTrackerManager : EntityManagerBase<ProjectTracker>
    {
        #region Constructors
        public PhotoTrackerManager() : base() { }
        public PhotoTrackerManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) : base(context, logger, cache) { }
        #endregion

        public long IncrementCounter(int photoId)
        {
            Logger.Log("PhotoTrackerManager.IncrementCounter({0})", photoId);

            IncrementPhotoCounter incrementPhotoCounter = new IncrementPhotoCounter(DatabaseContext)
            {
                PhotoId = photoId
            };

            long newCount = -1;
            try
            {
                newCount = incrementPhotoCounter.Execute();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return newCount;
        }

        public long IncrementCounter(string username, int year, int month, int day)
        {
            Logger.Log("PhotoTrackerManager.IncrementCounter({0}, {1}, {2}, {3})", username, year, month, day);

            // Get the photo for the passed parameters.
            PhotoManager photoManager = new PhotoManager();

            try
            {
                // Use AsyncHelper to call this from a synchronous function.
                // Hangfire doesn't work with async functions.
                var photo = photoManager.GetDefaultProjectPhotoViewModel(username, year, month, day);

                return IncrementCounter(photo.PhotoViewModel.PhotoId);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return -1;
        }
    }
}