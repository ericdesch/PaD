using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.SqlClient;

using PaD.Models;
using PaD.ViewModels;
using PaD.DataContexts;

namespace PaD.DAL.Projects
{
    public class GetHighestRankedPhoto
    {
        private IPaDDb _context;

        public int ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public GetHighestRankedPhoto() { }
        public GetHighestRankedPhoto(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<Photo> ExecuteAsync()
        {
            // Call the spGetHighestRankedPhotoId stored procedure to get the highest ranked photo.
            // Stored proceedures are created with migrations.
            var sqlParams = new object[] {
                new SqlParameter {
                    ParameterName = "projectId",
                    Value = ProjectId
                },
                new SqlParameter {
                    ParameterName = "startDate",
                    Value = StartDate
                },
                new SqlParameter {
                    ParameterName = "endDate",
                    Value = EndDate
                }
            };

            var results = await _context.Database.SqlQuery<RankedPhotoId>(
                "spGetHighestRankedPhotoId @projectId, @startDate, @endDate",
                sqlParams).SingleOrDefaultAsync();

            if (results == null)
                return null;

            int photoId = results.PhotoId;

            Photo photo = _context.Photo.Where(p => p.PhotoId == photoId).FirstOrDefault();

            return photo;
        }
    }

    class RankedPhotoId
    {
        public int PhotoId { get; set; }
        public double Rank { get; set; }
    }
}