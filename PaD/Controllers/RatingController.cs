using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Caching;

using PaD.DAL;
using PaD.Models;
using PaD.DataContexts;
using PaD.Infrastructure;
using PaD.ViewModels;
using PaD.CustomFilters;
using Fooz.Logging;
using Fooz.Caching;

namespace PaD.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.ProjectOwner, Role.User)]
    public class RatingController : ControllerBase
    {
        #region Constructor
        public RatingController(IDbContext dbContext, ILoggerProvider loggerProvider, ICacheProvider cacheProvider) 
            : base(dbContext, loggerProvider, cacheProvider)
        { }
        #endregion

        #region Add
        // POST: /Ratings/Add/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(int photoId, string username, double value)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException();
            }

            RatingsManager ratingsManager = new RatingsManager(DatabaseContext, Logger, Cache);

            // Add the new rating
            double newAverage = await ratingsManager.AddOrUpdateRatingAsync(photoId, username, value);

            // Use data.newAverage for the new average rating for this photo
            var result = new { success = "True", newAverage = newAverage };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
