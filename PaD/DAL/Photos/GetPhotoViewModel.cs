using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PaD.Models;
using PaD.ViewModels;
using PaD.DataContexts;

namespace PaD.DAL.Photos
{
    public class GetPhotoViewModel
    {
        private IPaDDb _context;

        public string AuthenticatedUser { get; set; }
        public int ProjectId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }

        public GetPhotoViewModel(IDbContext databaseContext)
        {
            // allow it to be injected
            _context = databaseContext as IPaDDb;
        }

        public async Task<PhotoViewModel> ExecuteAsync()
        {
            DateTime date = new DateTime(Year, Month, Day);

            var photo = await _context.Photo
                .Where(p => p.ProjectId == ProjectId && p.Date == date)
                .Select(p => new PhotoViewModel
                {
                    ProjectId = p.ProjectId,
                    PhotoId = p.PhotoId,
                    Title = p.Title,
                    Alt = p.Alt,
                    Tags = p.Tags,
                    Date = p.Date,
                    IsPhotoOfTheMonth = p.IsPhotoOfTheMonth,
                    IsPhotoOfTheYear = p.IsPhotoOfTheYear,
                    PhotoImage = new ImageViewModel
                    {
                        Bytes = p.PhotoImage.Bytes,
                        ContentType = p.PhotoImage.ContentType
                    } ,
                    ThumbnailImage = new ImageViewModel
                    {
                        Bytes = p.ThumbnailImage.Bytes,
                        ContentType = p.ThumbnailImage.ContentType
                    },
                    AuthenticatedUserName = AuthenticatedUser,
                    AuthenticatedUserRating = p.PhotoRatings.Where(r => r.IdentityUserName == AuthenticatedUser && r.PhotoId == p.PhotoId).FirstOrDefault().Value,
                    AveRating = p.PhotoRatings.Where(r => r.PhotoId == p.PhotoId).Average(r => (double?)r.Value) ?? 0
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return photo;
        }
    }
}