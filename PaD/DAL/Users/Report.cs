using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using PaD.DataContexts;
using PaD.Models;

namespace PaD.DAL.Users
{
    public class Report
    {
        private IdentityDb _context;

        public string UserName { get; set; }
        public string ReportedBy { get; set; }

        public Report(IdentityDb databaseContext)
        {
            // allow it to be injected
            _context = databaseContext;
        }

        public async Task<int> ExecuteAsync()
        {
            ReportedUser reportedUser = new ReportedUser() {
                UserName = UserName,
                ReportedBy = ReportedBy,
                ReportStatus = ReportStatus.New
            };

            _context.ReportedUser.Add(reportedUser);

            return await _context.SaveChangesAsync();
        }
    }
}