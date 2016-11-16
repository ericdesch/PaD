using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PagedList;
using PagedList.EntityFramework;

using PaD.Models;
using PaD.ViewModels;
using PaD.DataContexts;

namespace PaD.DAL.Photos
{
    public class Search
    {
        private IPaDDb _context;

        public string QueryString { get; set; }
        public string UserName { get; set; }
        public bool IsPhotoOfTheMonth { get; set; }
        public bool IsPhotoOfTheYear { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public Search(IDbContext databaseContext)
        {
            // allow it to be injected
            _context = databaseContext as IPaDDb;
        }

        public async Task<IPagedList<PhotoViewModel>> ExecuteAsync()
        {
            var photos =  _context.Photo
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
                    UserName = p.Project.IdentityUserName,
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
                    AveRating = p.PhotoRatings.Where(r => r.PhotoId == p.PhotoId).Average(r => (double?)r.Value) ?? 0
                })
                .OrderByDescending(p => p.Date)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(QueryString))
            {
                photos = photos.Where(p => p.Title.ToLower().Contains(QueryString.ToLower()) ||
                    p.Tags.ToLower().Contains(QueryString.ToLower()));
            }

            if (!String.IsNullOrEmpty(UserName))
            {
                photos = photos.Where(p => p.UserName == UserName);
            }

            if (IsPhotoOfTheMonth)
            {
                photos = photos.Where(p => p.IsPhotoOfTheMonth);
            }

            if (IsPhotoOfTheYear)
            {
                photos = photos.Where(p => p.IsPhotoOfTheYear);
            }

            var photoList = await photos.ToPagedListAsync<PhotoViewModel>(Page, PageSize);

            return photoList;
        }

        public IPagedList<PhotoViewModel> Execute()
        {
            var photos = _context.Photo
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
                    UserName = p.Project.IdentityUserName,
                    PhotoImage = new ImageViewModel
                    {
                        Bytes = p.PhotoImage.Bytes,
                        ContentType = p.PhotoImage.ContentType
                    },
                    ThumbnailImage = new ImageViewModel
                    {
                        Bytes = p.ThumbnailImage.Bytes,
                        ContentType = p.ThumbnailImage.ContentType
                    },
                    AveRating = p.PhotoRatings.Where(r => r.PhotoId == p.PhotoId).Average(r => (double?)r.Value) ?? 0
                })
                .OrderByDescending(p => p.Date)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(QueryString))
            {
                photos = photos.Where(p => p.Title.ToLower().Contains(QueryString.ToLower()) ||
                    p.Tags.ToLower().Contains(QueryString.ToLower()));
            }

            if (!String.IsNullOrEmpty(UserName))
            {
                photos = photos.Where(p => p.UserName == UserName);
            }

            if (IsPhotoOfTheMonth)
            {
                photos = photos.Where(p => p.IsPhotoOfTheMonth);
            }

            if (IsPhotoOfTheYear)
            {
                photos = photos.Where(p => p.IsPhotoOfTheYear);
            }

            var photoList = photos.ToPagedList<PhotoViewModel>(Page, PageSize);

            return photoList;
        }

    }
}