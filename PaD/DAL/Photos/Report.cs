using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using PaD.DataContexts;
using PaD.Models;

namespace PaD.DAL.Photos
{
    public class Report
    {
        private IPaDDb _context;

        public int PhotoId { get; set; }
        public string ReportedBy { get; set; }

        public Report(IDbContext databaseContext)
        {
            // allow it to be injected
            _context = databaseContext as IPaDDb;
        }

        public async Task<int> ExecuteAsync()
        {
            ReportedPhoto reportedPhoto = new ReportedPhoto()
            {
                PhotoId = PhotoId,
                ReportedBy = ReportedBy,
                ReportStatus = ReportStatus.New
            };

            _context.ReportedPhoto.Add(reportedPhoto);

            return await _context.SaveChangesAsync();
        }
    }
}