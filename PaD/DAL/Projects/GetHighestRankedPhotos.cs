using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Entity;

using PaD.Models;
using PaD.ViewModels;
using PaD.DataContexts;

namespace PaD.DAL.Projects
{
    public class GetHighestRankedPhotoIds
    {
        private IPaDDb _context;

        public int NumberOfPhotos { get; set; }

        public GetHighestRankedPhotoIds() { }
        public GetHighestRankedPhotoIds(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<List<Photo>> ExecuteAsync()
        {
            // Call the spGetHighestRankedPhotoId stored procedure to get the highest ranked photo.
            // Stored proceedures are created with migrations.
            var sqlParam = new SqlParameter {
                ParameterName = "numberOfPhotos",
                Value = NumberOfPhotos
            };

            var results = await _context.Database.SqlQuery<RankedPhotoId>(
                "spGetHighestRankedPhotoIds @numberOfPhotos",
                sqlParam).ToListAsync();

            if (results == null || results.Count == 0)
                return null;

            List<Photo> photos = new List<Photo>();

            foreach (RankedPhotoId rankedPhotoId in results)
            {
                Photo photo = _context.Photo
                    .Include(p => p.ThumbnailImage)
                    .Where(p => p.PhotoId == rankedPhotoId.PhotoId)
                    .FirstOrDefault();

                photos.Add(photo);
            }

            return photos;
        }
    }
}