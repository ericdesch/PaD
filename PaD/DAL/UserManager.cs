using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

using PaD.Models;
using PaD.DAL.Ratings;
using PaD.DAL.EntityBase;
using PaD.DataContexts;
using PaD.ViewModels;
using PaD.Infrastructure;
using PaD.DAL.Users;

namespace PaD.DAL
{
    public class UserManager : EntityManagerBase<ApplicationUser>
    {
        private IdentityDb _identityDb;

        #region Constructors
        public UserManager() : base() 
        {
            _identityDb = new IdentityDb();
        }

        //// Constructor that takes an IDbContext and an ILogger. NInject will create instances for us.
        //public UserManager(IDbContext databaseContext, ILoggerProvider logger) : base(databaseContext, logger) { }
        #endregion

        /// <summary>
        /// Determine if the email address for the passed userName has been confirmed.
        /// </summary>
        /// <param name="userName">The userName to check</param>
        /// <returns>True if the account for the passed userName has been confirmed. False the the account for the passed
        /// userName has not been confirmed or if the account for the passed username is not found.</returns>
        public bool EmailConfirmed(string userName)
        {
            var userStore = new UserStore<ApplicationUser>(_identityDb);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var applicationUser =  userManager.FindByName(userName);
            
            if (applicationUser == null)
                return false;

            return applicationUser.EmailConfirmed;
        }

        public IdentityResult AddToRole(string userName, Role role)
        {
            var userStore = new UserStore<ApplicationUser>(_identityDb);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var user = userManager.FindByName(userName);

            return userManager.AddToRole(user.Id, role.Description());
        }

        public async Task<int> ReportUser(string userName, string reportedBy)
        {
            Report report = new Report(_identityDb)
            {
                UserName = userName,
                ReportedBy = reportedBy
            };

            return await report.ExecuteAsync();
        }
    }
}