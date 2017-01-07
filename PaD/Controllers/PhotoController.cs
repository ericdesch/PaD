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
    //[RemoteRequireHttps]
    [AuthorizeRoles(Role.Admin, Role.ProjectOwner)]
    public class PhotoController : ControllerBase
    {
        #region Constructor
        public PhotoController(IDbContext dbContext, ILoggerProvider loggerProvider, ICacheProvider cacheProvider) 
            : base(dbContext, loggerProvider, cacheProvider)
        { }
        #endregion

        #region Index
        // GET: Photos
        //public async Task<ActionResult> Index()
        //{
        //    PhotoManager photoManager = new PhotoManager();
        //    var photos = await photoManager.FindAllAsync(p => p.ProjectId == UserDefaultProjectId);
        //    var orderdPhotos = photos.OrderByDescending(p => p.Date);

        //    return View(orderdPhotos);
        //}
        #endregion

        #region Details
        // GET: /year/month/day
        //public async Task<ActionResult> Details(int year, int month, int day)
        //{
        //    // Get the photo for the passed parameters.
        //    PhotoManager photoManager = new PhotoManager();

        //    PhotoViewModel viewModel = null;
        //    try
        //    {
        //        viewModel = await photoManager.GetPhotoViewModelAsync(User.Identity.Name, UserDefaultProjectId, year, month, day);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex);
        //        throw ex;
        //    }

        //    return View(viewModel);
        //}
        #endregion

        #region Create
        public async Task<ActionResult> Create(int? year, int? month, int? day)
        {
            PhotoCreateViewModel viewModel = new PhotoCreateViewModel();

            viewModel.Date = await GetDefaultCreateDateAsync(year, month, day);
            viewModel.AuthenticatedUserName = User.Identity.Name;
            // Default rating is 3
            viewModel.AuthenticatedUserRating = 3;

            return View(viewModel);
        }

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AuthenticatedUserName,AuthenticatedUserRating,PostedFile,Date,Title,Alt,Tags,IsPhotoOfTheMonth,IsPhotoOfTheYear")] PhotoCreateViewModel viewModel)
        {
            PhotoManager photoManager = new PhotoManager(DatabaseContext, Logger, Cache);

            // Server side validation to make sure we don't violate unqiue constraint on photo date and project id.
            Photo photo = await photoManager.FindAsync(p => p.Date == viewModel.Date && p.ProjectId == UserDefaultProjectId);
            if (photo != null)
            {
                ModelState.AddModelError("Date", "There is already a photo for that date.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Set the ProjectId to be the logged-in user's default ProjectId
            viewModel.ProjectId = UserDefaultProjectId;

            Photo addedPhoto = await photoManager.AddAsync(viewModel);

            // Redirect to the month view for the added photo.
            return RedirectToAction("Index", "Month", new { @username = User.Identity.Name, @year = addedPhoto.Date.Year, @month = addedPhoto.Date.Month });

        }
        #endregion

        #region Edit
        // GET: Photos/Edit/2015/12/31
        public async Task<ActionResult> Edit(int? year, int? month, int? day)
        {
            if (year == null || month == null || day == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the photo for the logged-in user's default project on this date.
            PhotoManager photoManager = new PhotoManager(DatabaseContext, Logger, Cache);
            var viewModel = await photoManager.GetPhotoEditViewModelAsync(User.Identity.Name, UserDefaultProjectId, (int)year, (int)month, (int)day);

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }

        // POST: Photos/Edit/2015/12/31
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AuthenticatedUserRating,PhotoId,ProjectId,PostedFile,Date,Title,Alt,Tags,IsPhotoOfTheMonth,IsPhotoOfTheYear")] PhotoEditViewModel viewModel)
        {
            // Get the Photo that we are editing
            PhotoManager photoManager = new PhotoManager(DatabaseContext, Logger, Cache);
            Photo photo = await photoManager.GetAsync(viewModel.PhotoId);

            if (photo == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            viewModel.AuthenticatedUserName = User.Identity.Name;

            await photoManager.UpdateAsync(photo, viewModel);

            // Redirect to the month view for the edited photo.
            return RedirectToAction("Index", "Month", new { @username = User.Identity.Name, @year = photo.Date.Year, @month = photo.Date.Month });

        }
        #endregion

        #region Delete
        // GET: Photos/Delete/2015/12/31
        public async Task<ActionResult> Delete(int? year, int? month, int? day)
        {

            if (year == null || month == null || day == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DateTime date = new DateTime((int)year, (int)month, (int)day);

            // Get the photo for the logged-in user's default project on this date.
            int projectId = UserDefaultProjectId;
            PhotoManager photoManger = new PhotoManager(DatabaseContext, Logger, Cache);
            var photo = await photoManger.FindAsync(p => p.Date == date && p.ProjectId == projectId);

            if (photo == null)
            {
                return HttpNotFound();
            }

            // Return the view with this photo to allow the user to confirm the delete.
            return View(photo);
        }

        // POST: Photos/Delete/2015/12/31
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);

            // Get the photo for the logged-in user's default project on this date.
            PhotoManager photoManger = new PhotoManager(DatabaseContext, Logger, Cache);
            var photo = await photoManger.FindAsync(p => p.Date == date && p.ProjectId == UserDefaultProjectId);

            if (photo == null)
            {
                return HttpNotFound();
            }

            // and delete it.
            await photoManger.DeleteAsync(photo);

            // Redirect to the month view for the deleted photo.
            return RedirectToAction("Index", "Month", new { @username = User.Identity.Name, @year = photo.Date.Year, @month = photo.Date.Month });
        }
        #endregion

        #region Private Methods
        private async Task<DateTime> GetDefaultCreateDateAsync(int? year, int? month, int? day)
        {
            DateTime date = new DateTime();
            string dateString = string.Format("{0}/{1}/{2}", year, month, day);

            // If passed date is invalid or no date is passed, use day after the project's last photo date.
            if ((!DateTime.TryParse(dateString, out date)) ||
                (year == null && month == null && day == null))
            {
                string username = User.Identity.Name;

                ProjectManager projectManager = new ProjectManager(DatabaseContext, Logger, Cache);
                date = await projectManager.GetLastPhotoDateAsync(username);
                date = date.AddDays(1);
            }

            return date;
        }
        #endregion
    }
}
