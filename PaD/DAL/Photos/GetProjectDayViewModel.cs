using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PaD.DataContexts;
using PaD.ViewModels;

namespace PaD.DAL.Photos
{
    public class GetProjectDayViewModel
    {
        private IPaDDb _context;

        public int ProjectId { get; set; }
        public DateTime Date { get; set; }

        public GetProjectDayViewModel(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<DayViewModel> ExecuteAsync()
        {
            var photo = await _context.Photo
                .Where(p => p.Date == Date
                    && p.ProjectId == ProjectId)
                .Select(p => new DayViewModel
                {
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

            return photo;
        }

        public DayViewModel Execute()
        {
            var photo = _context.Photo
                .Where(p => p.Date == Date
                    && p.ProjectId == ProjectId)
                .Select(p => new DayViewModel
                {
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
                .FirstOrDefault();

            return photo;
        }

    }
}