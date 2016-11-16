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
    public class ClearPhotoOfTheYear
    {
        private IPaDDb _context;

        public int ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ClearPhotoOfTheYear(IDbContext databaseContext)
        {
            // allow it to be injected
            _context = databaseContext as IPaDDb;
        }

        public async Task ExecuteAsync()
        {
            _context.Photo
                .Where(p => p.ProjectId == ProjectId && p.Date >= StartDate && p.Date <= EndDate)
                .ToList()
                .ForEach(p => p.IsPhotoOfTheYear = false);

            await _context.SaveChangesAsync();
        }
    }
}