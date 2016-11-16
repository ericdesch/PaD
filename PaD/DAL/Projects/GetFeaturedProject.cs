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
    public class GetFeaturedProject
    {
        private IPaDDb _context;

        public GetFeaturedProject() { }
        public GetFeaturedProject(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<ProjectViewModel> ExecuteAsync()
        {
            // Get the featured project. Should only be one.
            var featuredProject = await _context.FeaturedProject
                .Select(f => new ProjectViewModel
                {
                    ProjectId = f.Project.ProjectId,
                    FirstDate = f.Project.Photos.Min(p => p.Date),
                    LastDate = f.Project.Photos.Max(p => p.Date),
                    UserName = f.Project.IdentityUserName,
                    IsDefault = f.Project.IsDefault,
                    Title = f.Project.Title
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (featuredProject == null)
                throw new ArgumentException("No Featured Project found.");

            return featuredProject;
        }
    }
}