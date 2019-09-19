using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Helper
{
    public class NotificationHelper: CommonHelper
    {
        public static void ManageNotifications(Ticket oldTicket, Ticket newTicket)
        {
            CreateAssignmentNotification(oldTicket, newTicket);
            CreateChangeNotification(oldTicket, newTicket);
        }
        public static void CreateAssignmentNotification(Ticket oldTicket, Ticket newTicket)
        {
            var noChange = (oldTicket.AssignedToUserId == newTicket.AssignedToUserId);
            var assignment = (string.IsNullOrEmpty(oldTicket.AssignedToUserId));
            var unassignment = (string.IsNullOrEmpty(newTicket.AssignedToUserId));

            if (noChange)
            {
                return;
            }
            if (assignment)
            {
                GenerateAssignmentNotification(oldTicket, newTicket);
            }
            else if (unassignment)
            {
                GenerateUnAssignmentNotification(oldTicket, newTicket);
            }
            else
            {
                GenerateAssignmentNotification(oldTicket, newTicket);
                GenerateUnAssignmentNotification(oldTicket, newTicket);
            }
        }
        private static void GenerateUnAssignmentNotification(Ticket oldTicket, Ticket newTicket)
        {
            var notification = new TicketNotification
            {
                Created = DateTime.Now,
                Subject = $"You are unassigned from Ticket Id{newTicket.Id} on {DateTime.Now}",
                Read = false,
                RecipientId = oldTicket.AssignedToUserId,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                NotificationBody = $"Mark Read, if you have read this notification",
                TicketId = newTicket.Id
            };
            db.TicketNotifications.Add(notification);
           db.SaveChanges();
        }
        private static void GenerateAssignmentNotification(Ticket oldTicket, Ticket newTicket)
        {
            var notification = new TicketNotification
            {
                Created = DateTime.Now,
                Subject = $"You are assigned to Ticket Id{newTicket.Id} on {DateTime.Now}",
                Read = false,
                RecipientId = newTicket.AssignedToUserId,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                NotificationBody = $"Mark Read, if you have read this notification",
                TicketId = newTicket.Id
            };
            db.TicketNotifications.Add(notification);
           db.SaveChanges();

        }

        private static void CreateChangeNotification(Ticket oldTicket, Ticket newTicket)
        {
            var messageBody = new StringBuilder();
            foreach (var property in WebConfigurationManager.AppSettings["TrackedTicketProperties"].Split(','))
            {
                var oldValue = oldTicket.GetType().GetProperty(property).GetValue(oldTicket, null).ToString();
                var newValue = newTicket.GetType().GetProperty(property).GetValue(newTicket, null).ToString();

                if (oldValue != newValue)
                {
                    messageBody.AppendLine(new string('-', 45));
                    messageBody.AppendLine($"A change was made to Property:{property}.");
                    messageBody.AppendLine($"The old value was : {oldValue.ToString()}");
                    messageBody.AppendLine($"The new value is:{newValue.ToString()}");
                }
            }
            if (!string.IsNullOrEmpty(messageBody.ToString()))
            {
                var message = new StringBuilder();
                //message.AppendLine($"The Following changes were made to one of your Tickets on{newTicket.Updated}");
                message.AppendLine($"Changes were made to Ticket Id:{newTicket.Id} on {newTicket.Updated.GetValueOrDefault()}");
                message.AppendLine(messageBody.ToString());
                var senderId = HttpContext.Current.User.Identity.GetUserId();

                var notification = new TicketNotification
                {
                    TicketId = newTicket.Id,
                    Created = DateTime.Now,
                    Subject = $"A change has occured on Ticket{newTicket.Id}",
                    RecipientId = newTicket.AssignedToUserId,
                    SenderId = senderId,
                    NotificationBody = message.ToString(),
                    Read = false
                };
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
            }
        }

        public static int GetNewUserNotificationCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return db.TicketNotifications.Where(t => t.RecipientId == userId && !t.Read).Count();
        }

        public static int GetAllUserNotificationCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return db.TicketNotifications.Where(t => t.RecipientId == userId).Count();
        }

        public static List<TicketNotification> GetUnreadUserNotifications()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return db.TicketNotifications.Where(t => t.RecipientId == userId && !t.Read).ToList();

        }

        
    }
}