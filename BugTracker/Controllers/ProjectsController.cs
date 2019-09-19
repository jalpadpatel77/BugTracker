using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helper;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{

    [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
    [RequireHttps]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        public ActionResult MyProjects()
        {
            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);
            var myProjects = currentUser.Projects.ToList();

            return View("Index", myProjects);
           
        }

        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }         
            //Give details view
            var allProjectManagers = roleHelper.UsersInRole("Project Manager");
            var currentProjectManagers = projectHelper.UsersInRoleOnProject(project.Id, "Project Manager");
            ViewBag.ProjectManagers = new MultiSelectList(allProjectManagers, "Id", "FullName", currentProjectManagers);

            var allSubmitters = roleHelper.UsersInRole("Submitter");
            var currentSubmitters = projectHelper.UsersInRoleOnProject(project.Id, "Submitter");
            ViewBag.Submitters = new MultiSelectList(allSubmitters, "Id", "FullName", currentSubmitters);

            var allDevelopers = roleHelper.UsersInRole("Developer");
            var currentDevelopers = projectHelper.UsersInRoleOnProject(project.Id, "Developer");
            ViewBag.Developers = new MultiSelectList(allDevelopers, "Id", "FullName", currentDevelopers);

            return View(project);   //otherwise, return the 'Details' view with the project that had an id that matches the id passed to this method
        }
        [Authorize(Roles = "Admin, Project Manager")]
        // GET: Projects/Create
        public ActionResult Create()   //'Create' action method called when the user submits a request to the server that indicates that
                                       //the user want to create a new project...go to the 'Create' action in the 'Projects' controller
        {
            return View();  //The 'Create' view in the 'Projects' folder is returrned
        }

        // POST: Projects/Create
       
        [HttpPost]   //indicates that this method only handles POST requests, 
        [ValidateAntiForgeryToken]    //prevents fake post requests somehow
        public ActionResult Create([Bind(Include = "Id,Name,Description,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project); //go to the 'Projects' table in the database and add this 'project' with all of its properties and
                //their values.
                project.Created = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
                //This invokes the 'Index' action method, instead of returning a view. Go look at the 
                //'index' action method in this controller to see what happens next.
            }

            return View(project);  //is something went wrong with the binding then the method returns the 'Create' view with the data that was
            //passed to it....'projec
        }
        [Authorize(Roles = "Admin, Project Manager")]
        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)//This action method takes an integer as a parameter.That integer is named 'id'.
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
        [Authorize(Roles = "Admin")]
        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
