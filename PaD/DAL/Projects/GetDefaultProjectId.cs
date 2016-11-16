using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PaD.Models;
using PaD.ViewModels;
using PaD.DataContexts;

namespace PaD.DAL.Projects
{
    public class GetDefaultProjectId
    {
        private IPaDDb _context;

        public string UserName { get; set; }

        public GetDefaultProjectId() { }
        public GetDefaultProjectId(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<int> ExecuteAsync()
        {
            // Get default project for user.
            var defaultProjectId = await _context.Project
                .Where(p => p.IdentityUserName == UserName && p.IsDefault == true)
                .Select(p => p.ProjectId)
                .FirstOrDefaultAsync();

            return defaultProjectId;
        }

        public int Execute()
        {
            // Get default project for user.
            var defaultProjectId = _context.Project
                .Where(p => p.IdentityUserName == UserName && p.IsDefault == true)
                .Select(p => p.ProjectId)
                .FirstOrDefault();

            return defaultProjectId;
        }

    }
}