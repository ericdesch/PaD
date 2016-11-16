using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;

namespace PaD.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // TODO: track these so we can later delete abandoned accounts.
        //public virtual DateTime LastLoginDate { get; set; }
        //public virtual DateTime RegistrationDate { get; set; }
        [DefaultValue(true)]
        public bool IsEnabled { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            
            // Add custom user claims here

            return userIdentity;
        }

        //private int GetUserDefaultProjectId(string name)
        //{
        //    int projId = 0;

        //    PaD.DAL.ProjectManager projectManager = new PaD.DAL.ProjectManager();
        //    var project = projectManager.Find(p => p.IdentityUserName == name && p.IsDefault);

        //    if (project != null)
        //    {
        //        projId = project.ProjectId;
        //    }

        //    return projId;
        //}

    }
}