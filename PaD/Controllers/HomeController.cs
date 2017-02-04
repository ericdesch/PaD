using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Reflection;

using PaD.Models;
using PaD.ViewModels;
using PaD.Infrastructure;
using PaD.DataContexts;
using PaD.DAL;
using Fooz.Logging;
using Fooz.Caching;
using PaD.CustomFilters;

namespace PaD.Controllers
{
    public class HomeController : ControllerBase
    {
        #region Constructor
        public HomeController(IDbContext dbContext, ILoggerProvider loggerProvider, ICacheProvider cacheProvider) 
            : base(dbContext, loggerProvider, cacheProvider)
        { }
        #endregion

        public async Task<ActionResult> Index()
        {
            ProjectManager projectManager = new ProjectManager(DatabaseContext, Logger, Cache);
            HomeIndexViewModel viewModel = new HomeIndexViewModel();

            viewModel.TopPhotos = await projectManager.GetHighestRankedPhotosAsync(5);
            viewModel.FeaturedProject = await projectManager.GetFeaturedProjectAsync();
            viewModel.CurrentStreak = await projectManager.GetCurrentStreakDaysAsync(UserDefaultProjectId);

            return View(viewModel);
        }

        // GET: /Home/Search
        public async Task<ActionResult> Search()
        {
            // check for pushstate arguments
            if (Request.QueryString.AllKeys.Contains("queryString"))
            {
                string queryString = Request.QueryString["queryString"];
                string userName = Request.QueryString["userName"];
                bool isPhotoOfTheMonth = false;
                bool.TryParse(Request.QueryString["isPhotoOfTheMonth"], out isPhotoOfTheMonth);
                bool isPhotoOfTheYear = false;
                bool.TryParse(Request.QueryString["isPhotoOfTheYear"], out isPhotoOfTheYear);
                int page = 1;
                int.TryParse(Request.QueryString["page"], out page);

                var photos = await PerformSearch(queryString, userName, isPhotoOfTheMonth, isPhotoOfTheYear, page);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PhotoList", photos);
                }

                return View(photos);
            }

            return View();
        }

        // POST: /Home/Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(string queryString, string userName, bool isPhotoOfTheMonth, bool isPhotoOfTheYear, int page = 1)
        {
            var photos = await PerformSearch(queryString, userName, isPhotoOfTheMonth, isPhotoOfTheYear, page);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_PhotoList", photos);
            }

            return View(photos);
        }
        private async Task<PagedList.IPagedList<PhotoViewModel>> PerformSearch(string queryString, string userName, bool isPhotoOfTheMonth, bool isPhotoOfTheYear, int page)
        {
            int pageSize = 10;

            PhotoManager photoManager = new PhotoManager(DatabaseContext, Logger, Cache);
            var photos = await photoManager.SearchAsync(queryString, userName, isPhotoOfTheMonth, isPhotoOfTheYear, page, pageSize);

            return photos;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}