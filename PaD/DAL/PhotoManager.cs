using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

using PagedList;

using Fooz.Logging;
using Fooz.Caching;

using PaD.Models;
using PaD.DAL.Photos;
using PaD.DAL.EntityBase;
using PaD.DataContexts;
using PaD.ViewModels;
using PaD.Infrastructure;

namespace PaD.DAL
{
    public class PhotoManager : EntityManagerBase<Photo>
    {
        #region Constructors
        public PhotoManager() : base() { }
        public PhotoManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) : base(context, logger, cache) { }
        #endregion

        #region GetDefaultProjectPhotoViewModel
        public async Task<DayViewModel> GetDefaultProjectPhotoViewModelAsync(string username, int year, int month, int day)
        {
            // Get the default project for this userName
            ProjectManager projectManager = new ProjectManager();
            ProjectViewModel defaultProject = await projectManager.GetDefaultAsync(username);
            if (defaultProject == null)
            {
                return null;
            }

            DateTime date = new DateTime(year, month, day);

            // Use the ProjectId from the user's default project to get the DayViewModel
            GetProjectDayViewModel getProjectDayViewModelAsync = new GetProjectDayViewModel(DatabaseContext)
            {
                ProjectId = defaultProject.ProjectId,
                Date = date
            };

            DayViewModel projectDayViewModel = await getProjectDayViewModelAsync.ExecuteAsync();

            // Also set these properties
            projectDayViewModel.Username = username;
            projectDayViewModel.ProjectFirstDate = defaultProject.FirstDate;
            projectDayViewModel.ProjectLastDate = defaultProject.LastDate;
            projectDayViewModel.ProjectCurrentDate = date;

            return projectDayViewModel;
        }

        public DayViewModel GetDefaultProjectPhotoViewModel(string username, int year, int month, int day)
        {
            // Get the default project for this userName
            ProjectManager projectManager = new ProjectManager();
            ProjectViewModel defaultProject = projectManager.GetDefault(username);
            if (defaultProject == null)
            {
                return null;
            }

            DateTime date = new DateTime(year, month, day);

            GetProjectDayViewModel getProjectDayViewModel = new GetProjectDayViewModel(DatabaseContext)
            {
                ProjectId = defaultProject.ProjectId,
                Date = date
            };

            DayViewModel projectDayViewModel = getProjectDayViewModel.Execute();

            projectDayViewModel.Username = username;
            projectDayViewModel.ProjectFirstDate = defaultProject.FirstDate;
            projectDayViewModel.ProjectLastDate = defaultProject.LastDate;
            projectDayViewModel.ProjectCurrentDate = date;

            return projectDayViewModel;
        }
        #endregion

        #region GetPhotoViewModel
        public async Task<PhotoViewModel> GetPhotoViewModelAsync(string authenticatedUserName, int projectId, int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);

            GetPhotoViewModel getPhotoViewModel = new GetPhotoViewModel(DatabaseContext)
            {
                AuthenticatedUser = authenticatedUserName,
                ProjectId = projectId,
                Year = year,
                Month = month,
                Day = day
            };

            PhotoViewModel viewModel = await getPhotoViewModel.ExecuteAsync();

            return viewModel;
        }
        #endregion

        #region GetPhotoEditViewModel
        public async Task<PhotoEditViewModel> GetPhotoEditViewModelAsync(string authenticatedUserName, int projectId, int year, int month, int day)
        {
            // Get a PhotoViewModel then use the copy construtor to make a PhotoEditViewModel
            var viewModel = await GetPhotoViewModelAsync(authenticatedUserName, projectId, year, month, day);
            PhotoEditViewModel editViewModel = new PhotoEditViewModel(viewModel);

            return editViewModel;
        }
        #endregion

        #region Add
        public async Task<Photo> AddAsync(PhotoCreateViewModel viewModel)
        {
            // Copy the postedFile into the Photo
            if (viewModel.PostedFile != null)
            {
                // Covert InputStream to it's RawFormat. Jpeg, PNG, GIF, etc.
                byte[] bytes = GetPostedFileBytes(viewModel.PostedFile);
                ImageFormat imageFormat = GetPostedFileFormat(viewModel.PostedFile);

                viewModel.PhotoImage = new ImageViewModel()
                {
                    Bytes = bytes,
                    ContentType = imageFormat.GetMimeType()
                };
            }

            Photo photo = new Photo(viewModel);

            // Add the rating
            photo.PhotoRatings = new List<PhotoRating>();
            photo.PhotoRatings.Add(new PhotoRating()
            {
                IdentityUserName = viewModel.AuthenticatedUserName,
                Value = viewModel.AuthenticatedUserRating
            });

            // If this is marked as IsPhotoOfTheMonth or IsPhotoOfTheYear, make sure 
            // other photos for the same month or year are cleared of the IsPhotoOfTheMonth
            // and IsPhotoOfTheYear flags.
            // Do this before inserting the new Photo to avoid unqiue constraints on PhotoId
            // and these flags.
            if (photo.IsPhotoOfTheMonth)
            {
                await ClearPhotoOfTheMonthAsync(photo.ProjectId, photo.Date.Year, photo.Date.Month);
            }
            if (photo.IsPhotoOfTheYear)
            {
                await ClearPhotoOfTheYearAsync(photo.ProjectId, photo.Date.Year);
            }

            // Add the photo.
            Photo addedPhoto = this.Add(photo);

            // Invalidate the CachedMonthViewModel for this user's year/month
            MonthManager monthManager = new MonthManager();
            monthManager.InvalidateCachedMonthViewModel(viewModel.AuthenticatedUserName, viewModel.Date.Year, viewModel.Date.Month);

            return addedPhoto;
        }
        #endregion

        #region Update
        public async Task UpdateAsync(Photo photo, PhotoEditViewModel viewModel)
        {

            using (var transaction = DatabaseContext.Database.BeginTransaction())
            {
                try
                {
                    // If this is marked as IsPhotoOfTheMonth or IsPhotoOfTheYear, make sure 
                    // other photos for the same month or year are cleared of the IsPhotoOfTheMonth
                    // and IsPhotoOfTheYear flags.
                    // Do this before updating the Photo to avoid unqiue constraints on PhotoId
                    // and these flags.
                    if (viewModel.IsPhotoOfTheMonth)
                    {
                        await ClearPhotoOfTheMonthAsync(photo.ProjectId, photo.Date.Year, photo.Date.Month);
                    }
                    if (viewModel.IsPhotoOfTheYear)
                    {
                        await ClearPhotoOfTheYearAsync(photo.ProjectId, photo.Date.Year);
                    }

                    // Update the Photo
                    photo.Alt = viewModel.Alt;
                    photo.Date = viewModel.Date;
                    photo.IsPhotoOfTheMonth = viewModel.IsPhotoOfTheMonth;
                    photo.IsPhotoOfTheYear = viewModel.IsPhotoOfTheYear;
                    photo.Tags = viewModel.Tags;
                    photo.Title = viewModel.Title;

                    DatabaseContext.Entry(photo).State = EntityState.Modified;

                    // Update the rating if has changed
                    RatingsManager ratingsManager = new RatingsManager();
                    PhotoRating rating = await ratingsManager.FindAsync(c => c.IdentityUserName == viewModel.AuthenticatedUserName &&
                        c.PhotoId == viewModel.PhotoId);
                    if (rating != null && viewModel.AuthenticatedUserRating != rating.Value)
                    {
                        rating.Value = viewModel.AuthenticatedUserRating;
                        DatabaseContext.Entry(rating).State = EntityState.Modified;
                    }

                    // Update the PhotoImage and ThumbnailImage if a there is a PostedFile
                    if (viewModel.PostedFile != null)
                    {
                        byte[] bytes = GetPostedFileBytes(viewModel.PostedFile);
                        ImageFormat imageFormat = GetPostedFileFormat(viewModel.PostedFile);

                        photo.PhotoImage.Bytes = bytes;
                        photo.PhotoImage.ContentType = imageFormat.GetMimeType();

                        // Re-generate the thumbnail
                        photo.GenerateThumbnail();

                        PhotoImageManager photoImageManager = new PhotoImageManager();
                        DatabaseContext.Entry(photo.PhotoImage).State = EntityState.Modified;

                        ThumbnailImageManager thumbnailImageManager = new ThumbnailImageManager();
                        DatabaseContext.Entry(photo.ThumbnailImage).State = EntityState.Modified;
                    }

                    await DatabaseContext.SaveChangesAsync();

                    transaction.Commit();

                    // Invalidate the CachedMonthViewModel for this user's year/month
                    MonthManager monthManager = new MonthManager();
                    monthManager.InvalidateCachedMonthViewModel(viewModel.AuthenticatedUserName, viewModel.Date.Year, viewModel.Date.Month);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }
        #endregion

        #region Delete
        public new async Task DeleteAsync(Photo photo)
        {
            await base.DeleteAsync(photo);

            // Invalidate the CachedMonthViewModel for this user's year/month
            string userName = HttpContext.Current.User.Identity.Name;

            MonthManager monthManager = new MonthManager();
            monthManager.InvalidateCachedMonthViewModel(userName, photo.Date.Year, photo.Date.Month);
        }
        #endregion

        #region Search
        public async Task<IPagedList<PhotoViewModel>> SearchAsync(string queryString, string userName, bool isPhotoOfTheMonth, bool isPhotoOfTheYear, int page, int pageSize)
        {

            Search search = new Search(DatabaseContext)
            {
                QueryString = queryString,
                UserName = userName,
                IsPhotoOfTheMonth = isPhotoOfTheMonth,
                IsPhotoOfTheYear = isPhotoOfTheYear,
                Page = page,
                PageSize = pageSize
            };

            IPagedList<PhotoViewModel> photos = await search.ExecuteAsync();

            return photos;
        }

        public IPagedList<PhotoViewModel> Search(string queryString, string userName, bool isPhotoOfTheMonth, bool isPhotoOfTheYear, int page, int pageSize)
        {

            Search search = new Search(DatabaseContext)
                {
                    QueryString = queryString,
                    UserName = userName,
                    IsPhotoOfTheMonth = isPhotoOfTheMonth,
                    IsPhotoOfTheYear = isPhotoOfTheYear,
                    Page = page,
                    PageSize = pageSize
                };

            IPagedList<PhotoViewModel> photos = search.Execute();

            return photos;
        }
        #endregion

        #region ClearPhotoOfTheMonth, ClearPhotoOfTheYear
        private async Task ClearPhotoOfTheMonthAsync(int projectId, int year, int month)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            ClearPhotoOfTheMonth clearPhotoOfTheMonth = new ClearPhotoOfTheMonth(DatabaseContext)
            {
                ProjectId = projectId,
                StartDate = startDate,
                EndDate = endDate
            };

            await clearPhotoOfTheMonth.ExecuteAsync();
        }

        private async Task ClearPhotoOfTheYearAsync(int projectId, int year)
        {
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));

            ClearPhotoOfTheYear clearPhotoOfTheYear = new ClearPhotoOfTheYear(DatabaseContext)
            {
                ProjectId = projectId,
                StartDate = startDate,
                EndDate = endDate
            };

            await clearPhotoOfTheYear.ExecuteAsync();
        }
        #endregion

        #region GetPostedFileBytes
        // TODO: make these extensions of HttpPostedFileBase
        private byte[] GetPostedFileBytes(HttpPostedFileBase postedFile)
        {
            if (postedFile == null)
                throw new ArgumentException("postedFile cannot be null");

            ImageFormat imageFormat = GetPostedFileFormat(postedFile);
            return GetPostedFileBytes(postedFile, imageFormat);
        }

        private byte[] GetPostedFileBytes(HttpPostedFileBase postedFile, ImageFormat imageFormat)
        {
            if (postedFile == null)
                throw new ArgumentException("postedFile cannot be null");

            byte[] bytes;

            // Get the InputStream as a Bitmap so we can convert it.
            using (Bitmap bitmap = (Bitmap)Bitmap.FromStream(postedFile.InputStream))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // If imageFormat is null, use the bitmap's RawFormat
                    if (imageFormat == null)
                    {
                        imageFormat = bitmap.RawFormat;
                    }

                    // Save it in it's raw format (Jpeg, PNG, GIF, etc.)
                    bitmap.Save(ms, imageFormat);
                    bytes = ms.ToArray();
                }
            }

            return bytes;
        }
        #endregion

        #region GetPostedFileFormat
        // TODO: make this an extension of HttpPostedFileBase
        private ImageFormat GetPostedFileFormat(HttpPostedFileBase postedFile)
        {
            if (postedFile == null)
                throw new ArgumentException("postedFile cannot be null");

            ImageFormat imageFormat;
            using (Bitmap bitmap = (Bitmap)Bitmap.FromStream(postedFile.InputStream))
            {
                imageFormat = bitmap.RawFormat;
            }

            return imageFormat;
        }
        #endregion

        #region Report
        public async Task<int> ReportAsync(int photoId, string reportedBy)
        {
            Report report = new Report(DatabaseContext)
            {
                PhotoId = photoId,
                ReportedBy = reportedBy
            };

            int id = await report.ExecuteAsync();

            return id;
        }
        #endregion
    }
}