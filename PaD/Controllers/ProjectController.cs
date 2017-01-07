using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using PaD.DAL;
using PaD.ViewModels;
using PaD.Infrastructure;
using PaD.Models;
using PaD.CustomFilters;
using PaD.DataContexts;
using Fooz.Logging;
using Fooz.Caching;

namespace PaD.Controllers
{
    //[RemoteRequireHttps]
    [AuthorizeRoles(Role.Admin, Role.User)]
    public class ProjectController : ControllerBase
    {
        #region Constructor
        public ProjectController(IDbContext dbContext, ILoggerProvider loggerProvider, ICacheProvider cacheProvider) 
            : base(dbContext, loggerProvider, cacheProvider)
        { }
        #endregion

        #region Create
        public ActionResult Create()
        {
            // Only allow one project for each user.
            if (UserDefaultProjectId > 0)
            {
                return View("Error");
            }

            ProjectViewModel viewModel = new ProjectViewModel();
            viewModel.UserName = User.Identity.Name;

            return View(viewModel);
        }

        // POST: Project/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Title,UserName")] ProjectViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Always default; there can only be one for now.
            viewModel.IsDefault = true;

            ProjectManager projectManager = new ProjectManager(DatabaseContext, Logger, Cache);
            Project project = await projectManager.AddAsync(viewModel);

            // Update the user's UserDefaultProjectId
            UserDefaultProjectId = project.ProjectId;

            // Add the user to the ProjectOwner role
            UserManager userManager = new UserManager();
            userManager.AddToRole(viewModel.UserName, Role.ProjectOwner);

            // Redirect to the month view so they can start adding photos.
            return RedirectToAction("Index", "Month", new { @username = viewModel.UserName });

        }
        #endregion

        #region Update
        //[AuthorizeRoles(Role.Admin, Role.ProjectOwner)]

        #endregion
    }
}