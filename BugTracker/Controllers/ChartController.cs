using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helper;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using static BugTracker.ViewModels.ChartViewModel;



namespace BugTracker.Controllers

{

    public class ChartController : Controller

    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projectHelper = new ProjectsHelper();
        // GET: Chart

        public JsonResult TicketStatusChartData()

        {

            var dataSet = new List<MorrisBarChartData>();
            var ticketStatuses = db.TicketStatus.ToList();
            var userId = User.Identity.GetUserId();
            var Projts = projectHelper.ListUserProjects(userId);
            var Ticts = new List<Ticket>();
            if (User.IsInRole("Admin"))

            {
                foreach (var ticketStatus in ticketStatuses)
                {
                    var value = db.TicketStatus.Find(ticketStatus.Id).Tickets.Count();
                    dataSet.Add(new MorrisBarChartData { label = ticketStatus.StatusName, value = value });

                }
            }
            else if (User.IsInRole("Project Manager"))
            {
                foreach (var project in Projts)
                {
                    foreach (var ticket in project.Tickets)
                    {
                        Ticts.Add(ticket);
                    }
                }
                foreach (var ticketStatus in ticketStatuses)
                {
                    dataSet.Add(new MorrisBarChartData
                    {
                        label = ticketStatus.StatusName,
                        value = Ticts.Where(t => t.TicketStatus.StatusName == ticketStatus.StatusName).Count()
                    });
                }     
            }
            else if (User.IsInRole("Developer"))
            {
                foreach (var ticketStatus in ticketStatuses)
                {
                    var value = db.TicketStatus.Find(ticketStatus.Id).Tickets.Where(t => t.AssignedToUserId == userId).Count();
                    dataSet.Add(new MorrisBarChartData { label = ticketStatus.StatusName, value = value });
                }
            }
            else if (User.IsInRole("Submitter"))
            {
                foreach (var ticketStatus in ticketStatuses)
                {
                    var value = db.TicketStatus.Find(ticketStatus.Id).Tickets.Where(t => t.OwnerUserId == userId).Count();
                    dataSet.Add(new MorrisBarChartData { label = ticketStatus.StatusName, value = value });
                }
            }
            return Json(dataSet);
        }

        public JsonResult TicketTypeData()

        {

            var dataSet = new List<MorrisBarChartData>();
            var ticketTypes = db.TicketTypes.ToList();
            var userId = User.Identity.GetUserId();
            var Projts = projectHelper.ListUserProjects(userId);
            var Tics = new List<Ticket>();
            if (User.IsInRole("Admin"))

            {
                foreach (var Type in ticketTypes)
                {
                    var value = db.TicketTypes.Find(Type.Id).Tickets.Count();
                    dataSet.Add(new MorrisBarChartData { label = Type.Name, value = value });

                }
            }
            else if (User.IsInRole("Project Manager"))
            {
                foreach (var project in Projts)
                {
                    foreach (var ticket in project.Tickets)
                    {
                        Tics.Add(ticket);
                    }
                }
                foreach (var Type in ticketTypes)
                {
                    dataSet.Add(new MorrisBarChartData
                    {
                        label = Type.Name,
                        value = Tics.Where(t => t.TicketType.Name == Type.Name).Count()
                    });
                }
            }
            else if (User.IsInRole("Developer"))
            {
                foreach (var Type in ticketTypes)
                {
                    var value = db.TicketTypes.Find(Type.Id).Tickets.Where(t => t.AssignedToUserId == userId).Count();
                    dataSet.Add(new MorrisBarChartData { label = Type.Name, value = value });
                }
            }
            else if (User.IsInRole("Submitter"))
            {
                foreach (var Type in ticketTypes)
                {
                    var value = db.TicketTypes.Find(Type.Id).Tickets.Where(t => t.OwnerUserId == userId).Count();
                    dataSet.Add(new MorrisBarChartData { label = Type.Name, value = value });
                }
            }
            return Json(dataSet);
        }

        //PieChart

        //public JsonResult TicketTypeData()

        //{

        //    var dataSet = new List<PieChartData>();

        //    foreach (var ticketType in db.TicketTypes.ToList())

        //    {

        //        var value = db.TicketTypes.Find(ticketType.Id).Tickets.Count();

        //        dataSet.Add(new PieChartData { label = ticketType.Name, value = ticketType.Tickets.Count() });

        //    }

        //    return Json(dataSet);

        //}





    }

}