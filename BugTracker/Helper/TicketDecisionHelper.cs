 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Helper
{
    public class TicketDecisionHelper
    {

        private static ApplicationDbContext db = new ApplicationDbContext();
        private static UserRolesHelper roleHelper = new UserRolesHelper();
        private static ProjectsHelper projectHelper = new ProjectsHelper();
        public static bool TicketIsEditableByUser(Ticket ticket)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            switch (myRole)
            {
                case "Developer":
                    return ticket.AssignedToUserId == userId;
                case "Submitter":
                    return ticket.OwnerUserId == userId;
                case "Manager":
                    var myProjects = projectHelper.ListUserProjects(userId);
                    foreach (var project in myProjects)
                    {
                        foreach (var projticket in project.Tickets)
                        {
                            if (projticket.Id == ticket.Id)
                                return true;
                        }
                    }
                    return false;
                case "Admin":
                    return true;
                default:
                    return false;
            }
        }

        public int CountMyTickets(string userId)
        {
            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            var userRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var ticketCount = 0;
            switch (userRole)
            {
                case "Admin":
                   ticketCount = db.Tickets.Count();
                    break;
                case "Developer":
                    ticketCount = db.Tickets.Where(t => t.AssignedToUserId == userId).Count();
                    break;
                case "Submitter":
                    ticketCount = db.Tickets.Where(t => t.OwnerUserId == userId).Count();
                    break;
                case "Project Manager":
                    ticketCount = user.Projects.SelectMany(p => p.Tickets).ToList().Count();
                    break;
                default:
                    break;
            };
            return ticketCount;
        }

        public int CountMyImmediateTickets(string userId)
        {
            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            var userRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var ticketCount = 0;
            switch (userRole)
            {
                case "Admin":
                    ticketCount = db.Tickets.Where(t => t.TicketPriority.Name == "Immediate").Count();
                    break;
                case "Developer":
                    ticketCount = db.Tickets.Where(t => t.AssignedToUserId == userId && t.TicketPriority.Name == "Immediate").Count();
                    break;
                case "Submitter":
                    ticketCount = db.Tickets.Where(t => t.OwnerUserId == userId && t.TicketPriority.Name == "Immediate").Count();
                    break;
                case "Project Manager":
                    ticketCount = user.Projects.SelectMany(p => p.Tickets).Where(t => t.TicketPriority.Name == "Immediate").ToList().Count();
                    break;
                default:
                    break;
            };
            return ticketCount;
        }

    }


}
