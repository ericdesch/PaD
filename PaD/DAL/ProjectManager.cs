using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using Ninject;
using Fooz.Logging;
using Fooz.Caching;

using PaD.Models;
using PaD.DAL.Projects;
using PaD.ViewModels;
using PaD.DataContexts;
using PaD.Infrastructure;
using System.Data.Entity;

namespace PaD.DAL
{
    public class ProjectManager : EntityManagerBase<Project>
    {
        #region Constructors
        public ProjectManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) 
            : base(context, logger, cache)
        { }
        #endregion

        #region GetDefaultProjectId
        public async Task<int> GetDefaultProjectIdAsync(string userName)
        {
            GetDefaultProjectId getDefaultProjectId = new GetDefaultProjectId(DatabaseContext)
            {
                UserName = userName
            };

            var projectId = await getDefaultProjectId.ExecuteAsync();

            return projectId;
        }

        public int GetDefaultProjectId(string userName)
        {
            GetDefaultProjectId getDefaultProjectId = new GetDefaultProjectId(DatabaseContext)
            {
                UserName = userName
            };

            var projectId = getDefaultProjectId.Execute();

            return projectId;
        }
        #endregion

        #region GetDefault
        public async Task<ProjectViewModel> GetDefaultAsync(string userName)
        {
            GetDefault getDefault = new GetDefault(DatabaseContext)
            {
                UserName = userName
            };

            var project = await getDefault.ExecuteAsync();

            return project;
        }

        public ProjectViewModel GetDefault(string userName)
        {
            GetDefault getDefault = new GetDefault(DatabaseContext)
            {
                UserName = userName
            };

            var project = getDefault.Execute();

            return project;
        }
        #endregion

        #region GetFirstPhotoDate
        /// <summary>
        /// Returns the DateTime of the last Photo in the passed username's default project.
        /// </summary>
        /// <param name="userName">The user name of the project that you want</param>
        /// <returns>The DateTime of the last Photo in the passed username's default project</returns>
        public async Task<DateTime> GetFirstPhotoDateAsync(string userName)
        {
            GetFirstPhotoDate getFirstPhotoDate = new GetFirstPhotoDate(DatabaseContext)
            {
                UserName = userName
            };

            DateTime lastDate = await getFirstPhotoDate.ExecuteAsync();

            return lastDate;
        }

        public DateTime GetFirstPhotoDate(string userName)
        {
            GetFirstPhotoDate getFirstPhotoDate = new GetFirstPhotoDate(DatabaseContext)
            {
                UserName = userName
            };

            DateTime lastDate = getFirstPhotoDate.Execute();

            return lastDate;
        }
        #endregion

        #region GetLastPhotoDate
        /// <summary>
        /// Returns the DateTime of the last Photo in the passed username's default project.
        /// </summary>
        /// <param name="userName">The user name of the project that you want</param>
        /// <returns>The DateTime of the last Photo in the passed username's default project</returns>
        public async Task<DateTime> GetLastPhotoDateAsync(string userName)
        {
            GetLastPhotoDate getLastPhotoDate = new GetLastPhotoDate(DatabaseContext)
            {
                UserName = userName
            };

            DateTime lastDate = await getLastPhotoDate.ExecuteAsync();

            return lastDate;
        }

        public DateTime GetLastPhotoDate(string userName)
        {
            GetLastPhotoDate getLastPhotoDate = new GetLastPhotoDate(DatabaseContext)
            {
                UserName = userName
            };

            DateTime lastDate = getLastPhotoDate.Execute();

            return lastDate;
        }

        public async Task<DateTime> GetLastPhotoDateAsync(int projectId)
        {
            GetLastPhotoDateById getLastPhotoDateById = new GetLastPhotoDateById(DatabaseContext)
            {
                ProjectId = projectId
            };

            DateTime lastDate = await getLastPhotoDateById.ExecuteAsync();

            return lastDate;
        }

        public DateTime GetLastPhotoDate(int projectId)
        {
            GetLastPhotoDateById getLastPhotoDateById = new GetLastPhotoDateById(DatabaseContext)
            {
                ProjectId = projectId
            };

            DateTime lastDate = getLastPhotoDateById.Execute();

            return lastDate;
        }

        #endregion

        #region GetCurrentStreakDays
        /// <summary>
        /// Returns the current active streak for the passed projectId. A streak is considered active if the
        /// most recent photo date for the project is less than 2 days ago.
        /// </summary>
        /// <param name="projectId">The ID of the project for which we want the streak count</param>
        /// <returns>A string formatted to show the length of the current streak.</returns>
        public async Task<string> GetCurrentStreakDaysAsync(int projectId)
        {

            string cacheKey = GetCurrentStreakCacheKey(projectId);
            if (Cache.IsSet(cacheKey))
            {
                return (string)Cache.Get(cacheKey);
            }

            GetCurrentStreakDays getCurrentStreakDays = new GetCurrentStreakDays(DatabaseContext)
            {
                ProjectId = projectId
            };

            int days = await getCurrentStreakDays.ExecuteAsync();

            // Streak is from the last photo date to last photo date - days
            DateTime toDate = await GetLastPhotoDateAsync(projectId);
            DateTime fromDate = toDate.AddDays(-days);

            // Format the streak ("1 year, 3 months, 5 days" or "31 days", etc)
            string stringFormated = GetStreakDaysFormatted(fromDate, toDate);

            System.Runtime.Caching.CacheItemPolicy cachePolicy = new System.Runtime.Caching.CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
            };
            Cache.Set(cacheKey, stringFormated, cachePolicy);

            return stringFormated;
        }

        private string GetStreakDaysFormatted(DateTime fromDate, DateTime toDate)
        {
            DateTimeSpan dateTimeSpan = DateTimeSpan.CompareDates(fromDate, toDate);

            return (dateTimeSpan.Years > 0 ? dateTimeSpan.Years.ToString() + " years, " : "") +
               (dateTimeSpan.Months > 0 ? dateTimeSpan.Months.ToString() + " months, " : "") +
               dateTimeSpan.Days.ToString() + " days"; // Always include days, even if it is 0
        }
        #endregion

        #region GetHighestRankedPhoto(s)
        public async Task<Photo> GetHighestRankedPhotoAsync(int projectId, DateTime startDate, DateTime endDate)
        {
            GetHighestRankedPhoto getHighestRankedPhoto = new GetHighestRankedPhoto(DatabaseContext)
            {
                ProjectId = projectId,
                StartDate = startDate,
                EndDate = endDate
            };

            Photo photo = await getHighestRankedPhoto.ExecuteAsync();

            return photo;
        }

        public async Task<List<PhotoViewModel>> GetHighestRankedPhotosAsync(int numberOfPhotos)
        {
            string cacheKey = GetHighestRankedPhotosCacheKey(numberOfPhotos);
            if (Cache.IsSet(cacheKey))
            {
                return (List<PhotoViewModel>)Cache.Get(cacheKey);
            }

            GetHighestRankedPhotoIds getHighestRankedPhotos = new GetHighestRankedPhotoIds(DatabaseContext)
            {
                NumberOfPhotos = numberOfPhotos
            };

            List<Photo> photos = await getHighestRankedPhotos.ExecuteAsync();

            if (photos == null)
                return null;

            List<PhotoViewModel> viewModels = new List<PhotoViewModel>();

            foreach (Photo photo in photos)
            {
                PhotoViewModel viewModel = new PhotoViewModel(photo);
                viewModels.Add(viewModel);
            }

            System.Runtime.Caching.CacheItemPolicy cachePolicy = new System.Runtime.Caching.CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
            };
            Cache.Set(cacheKey, viewModels, cachePolicy);

            return viewModels;
        }
        #endregion

        #region GetFeauterdProject
        public async Task<FeaturedProjectViewModel> GetFeaturedProjectAsync()
        {
            string cacheKey = GetFeaturedProjectCacheKey();
            if (Cache.IsSet(cacheKey))
            {
                return (FeaturedProjectViewModel)Cache.Get(cacheKey);
            }

            // Get the featured project. Should only be one.
            GetFeaturedProject getFeaturedProject = new GetFeaturedProject(DatabaseContext);
            ProjectViewModel featuredProject = await getFeaturedProject.ExecuteAsync();

            // Get the latest photo of the month photo for this project to use as the thumbnail
            Photo photo = await ((IPaDDb)DatabaseContext).Photo.Where(p => p.IsPhotoOfTheMonth).OrderByDescending(p => p.Date).FirstOrDefaultAsync();
            if (photo == null)
            {
                // If no latest photo of the month, use the highest ranked photo.
                photo = await GetHighestRankedPhotoAsync(featuredProject.ProjectId, featuredProject.FirstDate, featuredProject.LastDate);
            }

            if (photo == null)
                throw new ArgumentException("No thumbnail found for featured project.");

            PhotoViewModel photoViewModel = new PhotoViewModel(photo);

            FeaturedProjectViewModel viewModel = new FeaturedProjectViewModel()
            {
                ProjectId = featuredProject.ProjectId,
                Title = featuredProject.Title,
                UserName = featuredProject.UserName,
                ThumbnailImage = photoViewModel.ThumbnailImage,
                ThumbnailTitle = photo.Title
            };

            System.Runtime.Caching.CacheItemPolicy cachePolicy = new System.Runtime.Caching.CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(30)
            };
            Cache.Set(cacheKey, viewModel, cachePolicy);

            return viewModel;
        }
        #endregion

        #region Add
        public async Task<Project> AddAsync(ProjectViewModel viewModel)
        {
            Project project = new Project(viewModel);

            Project addedProject= this.Add(project);
            await this.SaveChangesAsync();

            return addedProject;
        }
        #endregion

        #region Update
        public async Task UpdateAsync(ProjectViewModel viewModel)
        {
            // Build a project object from the passed viewModel
            Project project = new Project(viewModel);

            // Set the new object's state to modified so EF knows to update it.
            DatabaseContext.Entry(project).State = EntityState.Modified;

            // Save the changes to the database.
            await DatabaseContext.SaveChangesAsync();
        }
        #endregion

        #region InvalidateCachedCurrentStreakDays
        public void InvalidateCachedCurrentStreakDays(int projectId)
        {
            string key = GetCurrentStreakCacheKey(projectId);

            if (Cache.IsSet(key))
                Cache.Invalidate(key);
        }
        #endregion

        #region Report
        public async Task<int> ReportAsync(int projectId, string reportedBy)
        {
            Report report = new Report(DatabaseContext)
            {
                ProjectId = projectId,
                ReportedBy = reportedBy
            };

            int id = await report.ExecuteAsync();

            return id;
        }
        #endregion

        #region Private Methods
        private string GetHighestRankedPhotosCacheKey(int numberOfPhotos)
        {
            return "CACHE_HIGHEST_RANKED_PHOTOS_" + numberOfPhotos;
        }
        private string GetFeaturedProjectCacheKey()
        {
            return "CACHE_FEATURED_PROJECT";
        }
        private string GetCurrentStreakCacheKey(int projectId)
        {
            return "CACHE_CURRENT_STREAK_" + projectId;
        }
        #endregion
    }
}