using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Fooz.Logging;

using PaD.DAL;
using PaD.ViewModels;
using PaD.Infrastructure;

namespace PaD.Controllers
{
    public class DayController : ControllerBase
    {
        #region Index
        // GET: /username/year/month/day
        public async Task<ActionResult> Index(string username, int year, int month, int day)
        {
            //Logger.Log("GET: /{0}/{1}/{2}/{3}", username, year, month, day);

            // Get the photo for the passed parameters.
            DayManager dayManager = new DayManager();

            DayViewModel viewModel = null;
            try
            {
                viewModel = await dayManager.GetDefaultProjectDayViewModelAsync(username, year, month, day);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw ex;
            }

            // If the User is authenticated, get their rating value for this photo and add it to the view model.
            // DayViewModel will be null if no photo for this date, so skip this if it is null.
            viewModel.AuthenticatedUserRating = 0;
            if (Request.IsAuthenticated && viewModel.PhotoViewModel != null)
            {
                try
                {
                    RatingsManager ratingsManager = new RatingsManager();
                    viewModel.AuthenticatedUserRating = await ratingsManager.GetUserRatingValueAsync(viewModel.PhotoViewModel.PhotoId, User.Identity.Name);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    throw ex;
                }
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Day", viewModel);
            }

            return View(viewModel);
        }
        #endregion
    }
}