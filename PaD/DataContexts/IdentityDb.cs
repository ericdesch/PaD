using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PaD.Models;
using System.Data.Entity;

namespace PaD.DataContexts
{
    public class IdentityDb : IdentityDbContext<ApplicationUser>
    {
        public IdentityDb()
            : base("DefaultConnection")
        {
        }

        // Tell EF which entities to use
        public DbSet<ReportedUser> ReportedUser { get; set; }

        public static IdentityDb Create()
        {
            return new IdentityDb();
        }
    }
}