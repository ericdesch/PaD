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
using PaD.CustomFilters;
using PaD.DataContexts;
using Fooz.Caching;

namespace PaD.Controllers
{
    public class MonthController : ControllerBase
    {
        #region Constructor
        public MonthController(IDbContext dbContext, ILoggerProvider loggerProvider, ICacheProvider cacheProvider) 
            : base(dbContext, loggerProvider, cacheProvider)
        { }
        #endregion

        #region Index
        // GET: /username/year/month
        //[RequireEmailConfirmed]
        public async Task<ActionResult> Index(string userName, int year = 0, int month = 0)
        {
            MonthManager monthManager = new MonthManager(DatabaseContext, Logger, Cache);
            MonthViewModel viewModel = null;

            // Validate the passed date
            bool redirectToDate = false;
            DateTime validDate = monthManager.ValidatePhotoDate(userName, year, month, out redirectToDate);

            // If passed year and month were modified to make the validDate, redirect to the validated year and month.
            if (redirectToDate)
            {
                Response.RedirectToRoute("MonthView", new { @userName = userName, @year = validDate.Year, @month = validDate.Month });
            }

            // Get the project for the validated year/month. Important to use validated date, it will give you DateTime.Now
            // if passed year and month are 0.
            try
            {
                viewModel = await monthManager.GetDefaultProjectMonthViewModelAsync(userName, validDate.Year, validDate.Month);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw ex;
            }
       
            if (viewModel.PhotoViewModels == null || viewModel.PhotoViewModels.Count == 0)
            {
                // No photos for this project yet. Default dates to today.
                DateTime now = DateTime.Now;

                viewModel.FirstDate = new DateTime(now.Year, now.Month, 1);
                viewModel.LastDate = now;
                viewModel.Year = now.Year;
                viewModel.Month = now.Month;
            }

            // The title for the page
            if (year ==0 && month == 0)
            {
                ViewBag.Title = userName; // default page for this user, so just show username
            }
            else
            {
                DateTime yearMonth = new DateTime(year, month, 1);
                ViewBag.Title = yearMonth.ToString("y"); // year month date pattern.  "June 2009" in US
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Calendar", viewModel);
            }

            return View(viewModel);
        }
        #endregion
    }
}