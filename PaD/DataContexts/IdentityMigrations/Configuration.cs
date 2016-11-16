namespace PaD.DataContexts.IdentityMigrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Reflection;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Globalization;
    using System.Threading.Tasks;

    using PaD.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<IdentityDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"DataContexts\IdentityMigrations";
        }

        protected override void Seed(IdentityDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //// Launch debugger so we can step through seed code.
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            //const string ROLE_ADMINISTRATOR = "Administrator"; // Can administer other users
            //const string ROLE_PROJECT_OWNER = "ProjectOwner"; // Can add/edit/delete projects and photos in those projects
            //const string ROLE_USER = "User"; // Can rate any photo

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            // Test roles to create
            var roleNames = new List<Role>(){ 
                Role.Admin,
                Role.ProjectOwner,
                Role.User
            };

            foreach (Role testRole in roleNames)
            {
                string roleName = testRole.Description();

                if (!roleManager.RoleExists(roleName))
                {
                    var role = new IdentityRole { Name = roleName };
                    roleManager.Create(role);
                }
            }

            // Test users to create
            var testUsers = new[] {
                new {
                    UserName = "eric",
                    Email = "ericdesch@gmail.com",
                    Password = "shadowfax",
                    Roles = new Role[] { Role.Admin, Role.ProjectOwner }
                },
                new {
                    UserName = "bailey",
                    Email = "baileydesch@gmail.com",
                    Password = "shadowfax",
                    Roles = new Role[] { Role.ProjectOwner }
                },
                new {
                    UserName = "dave",
                    Email = "edesch@gmail.com",
                    Password = "shadowfax",
                    Roles = new Role[] { Role.ProjectOwner }
                },
                new {
                    UserName = "zoe",
                    Email = "edesch@gmail.com",
                    Password = "shadowfax",
                    Roles = new Role[] { Role.User }
                }
            };

            foreach (var testUser in testUsers)
            {
                if (userManager.FindByName(testUser.UserName) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = testUser.UserName,
                        Email = testUser.Email,
                        EmailConfirmed = true,
                        IsEnabled = true
                    };
                    var result = userManager.Create(user, testUser.Password);
                    result = userManager.SetLockoutEnabled(user.Id, false);
                    foreach (Role role in testUser.Roles)
                    {
                        userManager.AddToRole(user.Id, role.Description());
                    }
                }
            }
        }
    }
}
