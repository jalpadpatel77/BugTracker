namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Configuration;
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            #region roleManager
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            #endregion

            //I need to create a few users that will eventually occupy the roles of either Admin,Pm,dev or Sub

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            #region Seed Admin, PM, Dev, Sub
            if (!context.Users.Any(u => u.Email == "JPatel@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "JPatel@Mailinator.com",
                    Email = "JPatel@Mailinator.com",
                    FirstName = "Jalpa",
                    LastName = "Patel",
                    DisplayName = "Jpatel"
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "ProjectManager@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ProjectManager@Mailinator.com",
                    Email = "ProjectManager@Mailinator.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "Twich"
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "Developer@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Developer@Mailinator.com",
                    Email = "Developer@Mailinator.com",
                    FirstName = "Bob",
                    LastName = "Smith",
                    DisplayName = "BSmith"
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "Submitter@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Submitter@Mailinator.com",
                    Email = "Submitter@Mailinator.com",
                    FirstName = "Jay",
                    LastName = "Ray",
                    DisplayName = "JRay"
                }, "Abc&123!");
            }

            //Introduce Demo users

            if (!context.Users.Any(u => u.Email == "DemoAdmin@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoAdmin@Mailinator.com",
                    Email = "DemoAdmin@Mailinator.com",
                    FirstName = "DemoA",
                    LastName = "Admin",
                    DisplayName = "DemoAdmin"
                }, WebConfigurationManager.AppSettings["DemoUserPassword"]);
            }
            if (!context.Users.Any(u => u.Email == "DemoProjectManager@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoProjectManager@Mailinator.com",
                    Email = "DemoProjectManager@Mailinator.com",
                    FirstName = "DemoP",
                    LastName = "ProjectManager",
                    DisplayName = "DemoPM"
                }, WebConfigurationManager.AppSettings["DemoUserPassword"]);
            }
            if (!context.Users.Any(u => u.Email == "DemoDeveloper@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoDeveloper@Mailinator.com",
                    Email = "DemoDeveloper@Mailinator.com",
                    FirstName = "DemoD",
                    LastName = "Developer",
                    DisplayName = "DemoDev"
                }, WebConfigurationManager.AppSettings["DemoUserPassword"]);
            }
            if (!context.Users.Any(u => u.Email == "DemoSubmitter@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoSubmitter@Mailinator.com",
                    Email = "DemoSubmitter@Mailinator.com",
                    FirstName = "DemoS",
                    LastName = "Submitter",
                    DisplayName = "DemoSub"
                }, WebConfigurationManager.AppSettings["DemoUserPassword"]);
            }

            context.SaveChanges();

            #endregion

            #region Role Assignment

            var userId = userManager.FindByEmail("JPatel@Mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

            userId = userManager.FindByEmail("DemoAdmin@Mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

            userId = userManager.FindByEmail("ProjectManager@Mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            userId = userManager.FindByEmail("DemoProjectManager@Mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            userId = userManager.FindByEmail("Developer@Mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            userId = userManager.FindByEmail("DemoDeveloper@Mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");


            userId = userManager.FindByEmail("Submitter@Mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            userId = userManager.FindByEmail("DemoSubmitter@Mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            context.SaveChanges();
            #endregion
        }
    }
}
