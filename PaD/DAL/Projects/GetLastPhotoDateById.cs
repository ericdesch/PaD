using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PaD.Models;
using PaD.DataContexts;

namespace PaD.DAL.Projects
{
    public class GetLastPhotoDateById
    {
        private IPaDDb _context;

        public int ProjectId { get; set; }

        public GetLastPhotoDateById() { }
        public GetLastPhotoDateById(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<DateTime> ExecuteAsync()
        {
            // Get the max date for the Photo whose parent project has the specified UserName and isDefault == true.
            // Have to use a Projection to get the Max value from the Photos collection.
            var result = await _context.Project
                .Where(p => p.ProjectId == ProjectId)
                .Select(d => new
                {
                    Date = (DateTime?)d.Photos.Max(c => c.Date) ?? DateTime.MinValue
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return DateTime.MinValue;
            }

            return result.Date;
        }

        public DateTime Execute()
        {
            // Get the max date for the Photo whose parent project has the specified UserName and isDefault == true.
            // Have to use a Projection to get the Max value from the Photos collection.
            var result = _context.Project
                .Where(p => p.ProjectId == ProjectId)
                .Select(d => new
                {
                    Date = (DateTime?)d.Photos.Max(c => c.Date) ?? DateTime.MinValue
                })
                .AsNoTracking()
                .FirstOrDefault();

            if (result == null)
            {
                return DateTime.MinValue;
            }

            return result.Date;
        }

    }
}