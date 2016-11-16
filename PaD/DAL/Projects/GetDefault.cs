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
    public class GetDefault
    {
        private IPaDDb _context;

        public string UserName { get; set; }

        public GetDefault() { }
        public GetDefault(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<ProjectViewModel> ExecuteAsync()
        {
            // Get default project for user.
            var defaultProject = await _context.Project
                .Where(p => p.IdentityUserName == UserName && p.IsDefault == true)
                .Select(p => new ProjectViewModel
                {
                    ProjectId = p.ProjectId,
                    Title = p.Title,
                    FirstDate = (DateTime?)p.Photos.Min(a => a.Date) ?? DateTime.MinValue,
                    LastDate = (DateTime?)p.Photos.Max(a => a.Date) ?? DateTime.MinValue,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return defaultProject;
        }

        public ProjectViewModel Execute()
        {
            // Get default project for user.
            var defaultProject = _context.Project
                .Where(p => p.IdentityUserName == UserName && p.IsDefault == true)
                .Select(p => new ProjectViewModel
                {
                    ProjectId = p.ProjectId,
                    Title = p.Title,
                    FirstDate = (DateTime?)p.Photos.Min(a => a.Date) ?? DateTime.MinValue,
                    LastDate = (DateTime?)p.Photos.Max(a => a.Date) ?? DateTime.MinValue,
                })
                .AsNoTracking()
                .FirstOrDefault();

            return defaultProject;
        }

    }
}