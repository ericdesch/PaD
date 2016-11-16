using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using PaD.DataContexts;
using PaD.Models;

namespace PaD.DAL.Projects
{
    public class Report
    {
        private IPaDDb _context;

        public int ProjectId { get; set; }
        public string ReportedBy { get; set; }

        public Report(IDbContext databaseContext)
        {
            // allow it to be injected
            _context = databaseContext as IPaDDb;
        }

        public async Task<int> ExecuteAsync()
        {
            ReportedProject reportedProject = new ReportedProject()
            {
                ProjectId = ProjectId,
                ReportedBy = ReportedBy,
                ReportStatus = ReportStatus.New
            };

            _context.ReportedProject.Add(reportedProject);

            return await _context.SaveChangesAsync();
        }
    }
}