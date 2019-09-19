using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helper;
using BugTracker.Models;
using BugTracker.ViewModels;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();

        // GET: Admin
        public ActionResult UserIndex()
        {
            var users = db.Users.Select(u => new UserProfileViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl,
                Email = u.Email
            }).ToList();
            return View(users);
        }
        
        //get Manage roles
        public ActionResult ManageRoles()
        {
            var users = db.Users.Select(u => new UserProfileViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl,
                Email = u.Email
            }).ToList();
            ViewBag.users = new MultiSelectList(db.Users.ToList(), "Id", "FullName");
            ViewBag.Rolename = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(users);
        }
        //post Manage Roles
        [HttpPost]
        public ActionResult ManageRoles(List<string>users, string roleName)
        {
            if (users != null)
            {
                //Lets iterate over the incoming list of users that were selected from the form
                // and remove each of them from whatever role they occupy
                foreach (var userId in users)
                {
                    //Remove them from any role they occupy
                    foreach (var role in roleHelper.ListUserRoles(userId))
                    {
                        roleHelper.RemoveUserFromRole(userId, role);
                    }
                    if (!string.IsNullOrEmpty(roleName))
                    {
                        roleHelper.AddUserToRole(userId, roleName);//call the 'AddUserToRole' method from the 'helper' object and pass it the userid
                                                                   //and the specified 'role'.
                    }
                }
            }
            return RedirectToAction("ManageRoles");
        }
        //get ManageuserProjects
        public ActionResult ManageUserProjects(string userId)
        {
            var myProjects = projectHelper.ListUserProjects(userId).Select(p => p.Id);
           // ViewBag.usersId = userId;
            ViewBag.Projects = new MultiSelectList(db.Projects.ToList(), "Id", "Name", myProjects);
            return View();
        }

        //Post ManageuserProjects
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserProjects(string userId, List<int> projects)
        {
            foreach (var project in projectHelper.ListUserProjects(userId).ToList())                
            {
                //Remove them from any project they are on
                projectHelper.RemoveUserFromProject(userId, project.Id);
                                  
                }
                if (projects!= null)
            { 
                    foreach (var projectId in projects)
                    {
                    
                        projectHelper.AddUserToProject(userId, projectId);
                }
            }
            return RedirectToAction("UserIndex");
            
        }

        //get ManageUserRole
        [HttpGet]
        public ActionResult ManageUserRole(string userId)
        {
            var currentRole = roleHelper.ListUserRoles(userId).FirstOrDefault(); //call the 'ListUserRole' method from the 'helper' object and pass it the userid
                                                                                 //and name it currentRole.

            ViewBag.UserId = userId;
            ViewBag.UserRole = new SelectList(db.Roles.ToList(), "Name", "Name", currentRole);
            return View();
        }
        //post ManageUserRole
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserRole(string userId, string userRole)
        {


            foreach (var role in roleHelper.ListUserRoles(userId))
            {
                roleHelper.RemoveUserFromRole(userId, role);
            }
            //if the incoming role selection is not null assign the user to the seleted one
            if (userRole != null)
            {
                roleHelper.AddUserToRole(userId, userRole); //call the 'AddUserToRole' method from the 'helper' object and pass it the userid
                                                            //and the specified 'role'.
            }

            return RedirectToAction("UserIndex");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectUsers(int projectId, List<string> ProjectManagers, List<string> Developers, List<string> Submitters)
        {
            //Step 1: Remove all users from the project
            foreach (var user in projectHelper.UsersOnProject(projectId).ToList())
            {
                projectHelper.RemoveUserFromProject(user.Id, projectId);
            }

            //Step 2: Adds back all the selected PM's
            if (ProjectManagers != null)
            {
                foreach (var projectManagerId in ProjectManagers)
                {
                    projectHelper.AddUserToProject(projectManagerId, projectId);
                }
            }

            //Step 3: Adds back all the selected Developers
            if (Developers != null)
            {
                foreach (var developerId in Developers)
                {
                    projectHelper.AddUserToProject(developerId, projectId);
                }
            }

            //Step 4: Adds back all the selected Submitters
            if (Submitters != null)
            {
                foreach (var submitterId in Submitters)
                {
                    projectHelper.AddUserToProject(submitterId, projectId);
                }
            }

            //Step 4: Redirect users
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

    }
}
