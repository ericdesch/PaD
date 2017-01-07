using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using Fooz.Logging;
using Fooz.Caching;

using PaD.Models;
using PaD.DAL.Days;
using PaD.DAL.EntityBase;
using PaD.DataContexts;
using PaD.ViewModels;
using PaD.Infrastructure;

namespace PaD.DAL
{
    public class DayManager : EntityManagerBase<Photo>
    {
        #region Constructors
        public DayManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) 
            : base(context, logger, cache)
        { }
        #endregion

        #region GetDefaultProjectDayViewModel
        public async Task<DayViewModel> GetDefaultProjectDayViewModelAsync(string userName, int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);

            GetDefaultProjectDayViewModel getDefaultProjectDayViewModel = new GetDefaultProjectDayViewModel(DatabaseContext)
            {
                UserName = userName,
                Date = date
            };

            var projectDayViewModel = await getDefaultProjectDayViewModel.ExecuteAsync();

            return projectDayViewModel;
        }
        #endregion
    }
}