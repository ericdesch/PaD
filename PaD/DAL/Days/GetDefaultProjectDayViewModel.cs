using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity.Core;
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;

using PaD.Models;
using PaD.ViewModels;
using PaD.DataContexts;

namespace PaD.DAL.Days
{
    public class GetDefaultProjectDayViewModel
    {
        private IPaDDb _context;

        public string UserName { get; set; }
        public DateTime Date { get; set; }

        public GetDefaultProjectDayViewModel() { }
        public GetDefaultProjectDayViewModel(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<DayViewModel> ExecuteAsync()
        {
            // Call the spIncrementPhotoCounter stored proceedure to increment this value.
            // Stored proceedures are created with migrations.
            var userNameParam = new SqlParameter
            {
                ParameterName = "userName",
                Value = UserName
            };
            var dateParam = new SqlParameter
            {
                ParameterName = "date",
                Value = Date
            };

            var results = _context.Database.SqlQuery<ProjectDay>(
                "spGetDefaultProjectDayViewModel @userName, @date",
                userNameParam,
                dateParam);

            ProjectDay projectDay = null;
            try
            {
                projectDay = await results.SingleAsync();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            if (projectDay == null)
            {
                throw new ArgumentException(string.Format("No data found for the passed username/date: {0}/{1}", UserName, Date));
            }

            // Now build a DayViewModel from the ProjectDay class 
            DayViewModel dayView = new DayViewModel()
            {
                Username = projectDay.Username,
                ProjectFirstDate = projectDay.FirstDate,
                ProjectLastDate = projectDay.LastDate,
                ProjectCurrentDate = Date,
            };

            // There may not be a PhotoViewModel, so only add PhotoViewModel and its
            // children if it is there.
            if (projectDay.PhotoId != null)
            {
                dayView.PhotoViewModel = new PhotoViewModel()
                {
                    PhotoId = projectDay.PhotoId ?? 0,
                    Title = projectDay.PhotoTitle,
                    Alt = projectDay.Alt,
                    Date = projectDay.Date ?? DateTime.MinValue,
                    IsPhotoOfTheMonth = projectDay.IsPhotoOfTheMonth ?? false,
                    IsPhotoOfTheYear = projectDay.IsPhotoOfTheYear ?? false,
                    Tags = projectDay.Tags,
                    PhotoImage = new ImageViewModel()
                    {
                        Bytes = projectDay.Bytes ?? new byte[0],
                        ContentType = projectDay.ContentType
                    },
                    AveRating = projectDay.AveRating ?? 0
                };
            }
            else
            {
                dayView.PhotoViewModel = null;
            }

            return dayView;
        }

        // Only used within this method as the return type to spGetDefaultProjectDayViewModel
        private class ProjectDay
        {
            public int ProjectId { get; set; }
            public string Username { get; set; }
            public string ProjectTitle { get; set; }
            public bool IsDefault { get; set; }
            public DateTime FirstDate { get; set; }
            public DateTime LastDate { get; set; }
            public Nullable<int> PhotoId { get; set; }
            public Nullable<DateTime> Date { get; set; }
            public string PhotoTitle { get; set; }
            public string Alt { get; set; }
            public string Tags { get; set; }
            public Nullable<bool> IsPhotoOfTheMonth { get; set; }
            public Nullable<bool> IsPhotoOfTheYear { get; set; }
            public Nullable<double> AveRating { get; set; }
            public byte[] Bytes { get; set; }
            public string ContentType { get; set; }
        }

        public async Task<DayViewModel> ExecuteAsyncXXX()
        {
            var dayView = await _context.Photo
                .Where(p => p.Project.IdentityUserName == UserName
                    && p.Date == Date
                    && p.Project.IsDefault)
                .Select(p => new DayViewModel
                {
                    Username = UserName,
                    ProjectFirstDate = p.Project.Photos.Min(a => a.Date),
                    ProjectLastDate = p.Project.Photos.Max(a => a.Date),
                    PhotoViewModel = new PhotoViewModel
                    {
                        Alt = p.Alt,
                        Date = p.Date,
                        IsPhotoOfTheMonth = p.IsPhotoOfTheMonth,
                        IsPhotoOfTheYear = p.IsPhotoOfTheYear,
                        PhotoId = p.PhotoId,
                        PhotoImage = new ImageViewModel
                        {
                            Bytes = p.PhotoImage.Bytes,
                            ContentType = p.PhotoImage.ContentType
                        },
                        Tags = p.Tags,
                        Title = p.Title
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return dayView;
        }


    }
}