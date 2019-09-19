using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helper;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();
        private NotificationHelper notificationHelper = new NotificationHelper();
        private HistoryHelper historyHelper = new HistoryHelper();

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignTicket(int? id)
        {
            UserRolesHelper helper = new UserRolesHelper();
            var ticket = db.Tickets.Find(id);
            var users = helper.UsersInRole("Developer").ToList();
            ViewBag.AssignedToUserId = new SelectList(users, "Id", "FullName", ticket.AssignedToUserId);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignTicket(Ticket model)
        {
            var ticket = db.Tickets.Find(model.Id);

            ticket.AssignedToUserId = model.AssignedToUserId;
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
            db.SaveChanges();

            NotificationHelper.CreateAssignmentNotification(oldTicket, ticket);
            var historyHelper = new HistoryHelper();
            historyHelper.RecordHistory(oldTicket, ticket);
            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id },
                protocol: Request.Url.Scheme);
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user =  db.Users.Find(model.AssignedToUserId);
                msg.Body = "You have been assigned to a new Ticket." + Environment.NewLine +
                    "please click the following link to view details" +
                    "<a href=\"" + callbackUrl + "\">NEW TICKET</a>";
                msg.Destination = user.Email;
                msg.Subject = "Invite to Household";
                await ems.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                await Task.FromResult(0);
            }
            
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Project Manager,Developer,Submitter")]
        public ActionResult DashBoard(int Id)
        {
            var ticket = db.Tickets.Find(Id);
            return View(ticket);
        }

        // GET: Tickets
        [Authorize(Roles = "Admin, Project Manager")]
      
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketType);
            return View(tickets.ToList());

          
        }
        [Authorize(Roles ="Project Manager,Developer,Submitter")]
        //MyIndex wants to fill some view with my tickets only
        public ActionResult MyIndex()
        {
            //First Get the Id of the logged in user
            var userId = User.Identity.GetUserId();
            //then get the roles they occupy
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault(); //firstordefault Returns the first element of a sequence, or a default value if no element is found
            var myTickets = new List<Ticket>();
            //then based on the role name - push different data into the view
            switch(myRole)
            {
                case "Developer":
                    myTickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
                    break;
                case "Submitter":
                    myTickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
                    break;
               
                case "Project Manager":
                    myTickets = db.Users.Find(userId).Projects.SelectMany(t => t.Tickets ).ToList();
                    break;
                
            }     
            return View("Index",myTickets);
        }
       // [Authorize(Roles = "Admin")]
        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);  //View(ticket);
        }

        // GET: Tickets/Create
        //Authorize create tickets to submitters 
       [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();  //collect necessay data
            var myProjects = projectHelper.ListUserProjects(userId);
            ViewBag.ProjectId = new SelectList(myProjects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,ProjectId,TicketTypeId,TicketPriorityId,Description")] Ticket ticket)
        {
           
            if (ModelState.IsValid)
            {
               ticket.TicketStatusId = db.TicketStatus.FirstOrDefault(t => t.StatusName == "New/UnAssigned").Id;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTime.Now;
                
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("MyIndex");
            }
                     
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
           
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        //Developers can edit tickets
        [Authorize(Roles = "Admin, Project Manager,  Developer, Submitter")]
        // [Authorize]
        public ActionResult Edit(int? id)
        {
                 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }                      
            Ticket ticket = db.Tickets.Find(id);
            var allowed = false;
            var userId = User.Identity.GetUserId();

            if (ticket == null)
            {
                return HttpNotFound();
            }
            //Allthese can be deleted if you use Decision helper
           
            //Based on my ro le..can i edit the ticket
            if (User.IsInRole("Developer") && ticket.AssignedToUserId == userId)
                allowed = true;
            else if (User.IsInRole("Submitter") && ticket.OwnerUserId == userId)
                allowed = true;
            else if (User.IsInRole("Project Manager"))
            {
                allowed = true;

            }
            else if (User.IsInRole("Admin"))
            {
                allowed = true;
            }
            //All Above code can be deleted if use Decision helper
            // allowed can be TicketDecsionHelper.TicketIsEdi
            if (allowed)
            {     
                
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "StatusName", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
            }
            else
            {
                return RedirectToAction("AccessViolation", "Admin");
            }
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,OwnerUserId,AssignedToUserId,Title,Description,Created, Updated")] Ticket ticket)
        {
            

            if (ModelState.IsValid)
            {
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property(x => x.TicketStatusId).IsModified = true;
                db.Entry(ticket).Property(x => x.TicketTypeId).IsModified = true;
                db.Entry(ticket).Property(x => x.TicketPriorityId).IsModified = true;
                db.Entry(ticket).Property(x => x.Title).IsModified = true;
                db.Entry(ticket).Property(x => x.Description).IsModified = true;

                if (ticket.AssignedToUserId != null)
                    db.Entry(ticket).Property(x => x.AssignedToUserId).IsModified = true;
                ticket.Updated = DateTime.Now;
                db.SaveChanges();

                NotificationHelper.CreateAssignmentNotification(oldTicket, ticket);
                NotificationHelper.ManageNotifications(oldTicket, ticket);
                //HistoryHelper
                var historyHelper = new HistoryHelper();
                historyHelper.RecordHistory(oldTicket, ticket);

                return RedirectToAction("MyIndex");
            }
            //new edit ends
            ViewBag.AssignedToUserId = new SelectList(db.Users,"Id", "FirstName", ticket.AssignedToUserId);

            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "StatusName", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        [Authorize(Roles = "Project Manager,Developer,Submitter")]
        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
