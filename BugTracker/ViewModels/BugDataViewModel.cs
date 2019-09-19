using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.ViewModels
{
    public class BugDataViewModel
    {
        public virtual ICollection<Ticket> myTickets { get; set; }
        public virtual ICollection<ApplicationUser> myusers { get; set; }
        public virtual ICollection<Project> myProjects{ get; set; }

        public string AvatarUrl { get; set; }

        public BugDataViewModel()
        {
            myProjects = new HashSet<Project>();
            myTickets = new HashSet<Ticket>();
            myusers = new HashSet<ApplicationUser>();
        }

    }
}