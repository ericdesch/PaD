using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PaD.Models;
using PaD.ViewModels;
using PaD.DataContexts;

namespace PaD.DAL.Months
{
    public class GetDefaultProjectMonthViewModel
    {
        private IPaDDb _context;

        public string UserName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public GetDefaultProjectMonthViewModel(IDbContext databaseContext)
        {
            // allow it to be injected
            _context = databaseContext as IPaDDb;
        }

        public async Task<MonthViewModel> ExecuteAsync()
        {
            DateTime startDate = new DateTime(Year, Month, 1);
            DateTime endDate = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));

            var project = await _context.Project
                .Where(p => p.IdentityUserName == UserName && p.IsDefault)
                // TODO: EntityFramework 7 is supposed to support .Include into projections.
                // For now have to explicitly select into types to get the Thumbnail
                // to eager load.
                // Need to eager load Photos.Thumbnail
                //.Include(p => p.Photos.Select(t => t.Thumbnail))
                .Select(p => new MonthViewModel
                {
                    ProjectId = p.ProjectId,
                    Title = p.Title,
                    Month = Month,
                    Year = Year,
                    UserName = UserName,
                    PhotoViewModels = p.Photos
                        .Select(q => new PhotoViewModel
                        {
                            Alt = q.Alt,
                            Date = (DateTime?)q.Date ?? DateTime.MinValue,
                            IsPhotoOfTheMonth = q.IsPhotoOfTheMonth,
                            IsPhotoOfTheYear = q.IsPhotoOfTheYear,
                            PhotoId = q.PhotoId,
                            Tags = q.Tags,
                            ThumbnailImage = new ImageViewModel
                                {
                                    Bytes = q.ThumbnailImage.Bytes,
                                    ContentType = q.ThumbnailImage.ContentType
                                },
                            Title = q.Title
                        })
                        .Where(a => a.Date >= startDate && a.Date <= endDate)
                        .OrderBy(a => a.Date).ToList(),
                    FirstDate = (DateTime?)p.Photos.Min(a => a.Date) ?? DateTime.MinValue,
                    LastDate = (DateTime?)p.Photos.Max(a => a.Date) ?? DateTime.MinValue,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return project;
        }
    }
}