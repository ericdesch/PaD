using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Runtime.Caching;

using Ninject;
using Fooz.Logging;
using Fooz.Caching;

using PaD.Models;
using PaD.DAL.Months;
using PaD.DAL.EntityBase;
using PaD.DataContexts;
using PaD.ViewModels;
using PaD.Infrastructure;

namespace PaD.DAL
{
    public class MonthManager : EntityManagerBase<Project>
    {
        #region Constructors
        public MonthManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) 
            : base(context, logger, cache)
        { }
        #endregion

        #region GetDefaultProjectMonthViewModel
        public async Task<MonthViewModel> GetDefaultProjectMonthViewModelAsync(string userName, int year, int month)
        {
            ProjectManager projectManager = new ProjectManager(DatabaseContext, Logger, Cache);

            MonthViewModel viewModel = GetCachedMonthViewModel(userName, year, month);
            if (viewModel != null)
            {
                return viewModel;
            }

            GetDefaultProjectMonthViewModel getDefaultProjectMonthViewModel = new GetDefaultProjectMonthViewModel(DatabaseContext)
            {
                UserName = userName,
                Year = year,
                Month = month
            };

            viewModel = await getDefaultProjectMonthViewModel.ExecuteAsync();

            // If no project for this userName, instantiate a viewModel with only UserName set.
            if (viewModel == null)
            {
                viewModel = new MonthViewModel()
                {
                    UserName = userName,
                };
            }

            SetCachedMonthViewModel(userName, year, month, viewModel);

            return viewModel;
        }
        #endregion

        public void InvalidateCachedMonthViewModel(string userName, int year, int month)
        {
            string key = GetCacheKey(userName, year, month);

            if (Cache.IsSet(key))
                Cache.Invalidate(key);

        }

        private MonthViewModel GetCachedMonthViewModel(string userName, int year, int month)
        {
            string key = GetCacheKey(userName, year, month);

            if (Cache.IsSet(key))
                return Cache.Get(key) as MonthViewModel;
            else
                return null;
        }

        private void SetCachedMonthViewModel(string userName, int year, int month, MonthViewModel viewModel)
        {
            string key = GetCacheKey(userName, year, month);
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan(0, 1, 0, 0); // 1 hour

            Cache.Set(key, viewModel, policy);
        }

        private string GetCacheKey(string userName, int year, int month)
        {
            return string.Format("{0}-{1}-{2}", userName, year, month);
        }

        #region ValidatePhotoDate
        /// <summary>
        /// Returns the date to use when showing the Month view. If passed year and month
        /// are legit, return that date. If not, get the last photo date for this user's project.
        /// If that date is not legit, default to today.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DateTime ValidatePhotoDate(string userName, int year, int month, out bool redirectToDate)
        {
            redirectToDate = false;

            // Today
            DateTime nowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            ProjectManager projectManager = new ProjectManager(DatabaseContext, Logger, Cache);
            DateTime lastDate = projectManager.GetLastPhotoDate(userName);

            if (year == 0 && month == 0)
            {
                // Passing 0 for year and month is valid. Use lastPhotoDate if it is valid.
                if (lastDate != DateTime.MinValue)
                {
                    year = lastDate.Year;
                    month = lastDate.Month;
                }
                else
                {
                    // If lastPhotoDate is not valid, use nowDate.
                    year = nowDate.Year;
                    month = nowDate.Month;
                }
            }

            // Date we are trying to validate
            DateTime passedDate = new DateTime();

            try
            {
                passedDate = new DateTime(year, month, 1);
            }
            catch
            {
                // If unable to instantiate a DateTime with passed arguments,
                // default to Now.
                if (year != nowDate.Year && month != nowDate.Month)
                {
                    redirectToDate = true;
                }

                return nowDate;
            }

            // Get the passed user's default project
            var project = projectManager.Find(p => p.IdentityUserName == userName && p.IsDefault);

            if (project == null || project.Photos == null || project.Photos.Count == 0)
            {
                // If no project or no photos for this user, default to nowDate and set redirectToDate to true if 
                // passed date isn't dateNow.
                if (year != nowDate.Year && month != nowDate.Month)
                {
                    redirectToDate = true;
                }

                return nowDate;
            }

            // Make sure the passed date is within the range of valid dates
            // Get the project's first photo date. We already got the last photo date.
            DateTime firstDate = projectManager.GetFirstPhotoDate(userName);

            // If no first or last date found, return Now
            if (firstDate == DateTime.MinValue || lastDate == DateTime.MinValue)
            {
                if (year != nowDate.Year && month != nowDate.Month)
                {
                    redirectToDate = true;
                }

                return nowDate;
            }

            // If passed date is less than the project's first date, use the first date.
            if (firstDate != DateTime.MinValue && passedDate < firstDate)
            {
                redirectToDate = true;
                return firstDate;
            }

            // If passed date is more than the project's last date, use the last date.
            if (lastDate != DateTime.MinValue && passedDate > lastDate)
            {
                redirectToDate = true;
                return lastDate;
            }

            // If we got to here, passed args are valid
            return passedDate;
        }
        #endregion
    }
}