using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Helper;
using BugTracker.Models;

namespace BugTracker.Helper
{
    public class ProjectsHelper:CommonHelper
    {
       // private ApplicationDbContext db = new ApplicationDbContext();
       //private UserRolesHelper roleHelper = new UserRolesHelper();
        public List<string> UsersInRoleOnProject(int projectId, string roleName)
             
        {
            var people = new List<string>();
            foreach(var user in UsersOnProject(projectId).ToList())
            {
                if(roleHelper.IsUserInRole(user.Id, roleName))
                    {
                    people.Add(user.FullName);
                }
            }
            return people;
        }
        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u=>u.Id == userId);
            return (flag);
        }
        public ICollection<Project>ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return (projects);
        }

        public void AddUserToProject(string userId, int projectId)
        {
            if(!IsUserOnProject(userId, projectId))
                {
                Project proj = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);
                proj.Users.Add(newUser);
                db.SaveChanges();
             }
        }
        public void RemoveUserFromProject(string userId, int projectId)
        {
            if(IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var delUser = db.Users.Find(userId);

                proj.Users.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified;
                //db.SaveChanges();
            }             
        }

        public ICollection<ApplicationUser>UsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }
        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();

        }
        public ICollection<Project> ProjectsMissingRoles()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var badProjects = new List<Project>();
            var allProjects = db.Projects.ToList();
            foreach (var project in allProjects)
            {
                if (UsersInRoleOnProject(project.Id, "Manager").Count() == 0 ||
                   UsersInRoleOnProject(project.Id, "Developer").Count() == 0 ||
                   UsersInRoleOnProject(project.Id, "Submitter").Count() == 0)
                {
                    badProjects.Add(project);
                }
            }
            return badProjects;

        }
    }
}