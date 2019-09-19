using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Helper
{
    public class HistoryHelper: CommonHelper 
    {
        public void RecordHistory(Ticket oldTicket, Ticket newTicket)
        {
            foreach (var propObj1 in newTicket.GetType().GetProperties())
            {
                //If the current property is NOT one of the properties I am interested in then move on...
                var trackedProperties = WebConfigurationManager.AppSettings["TrackedHistoryProperties"].Split(',').ToList();
                if (!trackedProperties.Contains(propObj1.Name))
                    continue;

                //Else generate a TicketHistory record
                var oldTicketProp = oldTicket.GetType().GetProperty(propObj1.Name);
                var newTicketProp = newTicket.GetType().GetProperty(propObj1.Name);

                //record the current and old values of the property
                var newPropValue = (propObj1.GetValue(newTicket, null)??"").ToString();
                var oldPropValue =(propObj1.GetValue(oldTicket, null)??"").ToString();
               
                //if both properties are empty there is nothing to record

                if (newPropValue != oldPropValue  )
                {
                    var newHistory = new TicketHistory
                    {
                        PropertyName = propObj1.Name,
                        OldValue = Utilities.MakeReadable(propObj1.Name, oldPropValue),
                        NewValue = Utilities.MakeReadable(propObj1.Name, newPropValue),
                        Updated = newTicket.Updated.GetValueOrDefault(),
                        TicketId = newTicket.Id,
                        UserId = HttpContext.Current.User.Identity.GetUserId()
                    };
                    db.TicketHistories.Add(newHistory);
                }
                db.SaveChanges();
            }

        }
    }

}
