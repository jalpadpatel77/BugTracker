using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Ticket
    {
       // [StringLength(200, ErrorMessage = "Title must be between {2} and {1} characters long.", MinimumLength = 5)]
        public int Id { get; set; }

      //  [StringLength(50, ErrorMessage= "Title must be between {2} and {1} characters long.", MinimumLength = 5)]
        public string Title { get; set; }

        [Display(Name = "Project Name")]
        public int ProjectId { get; set; }

        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }

        [Display(Name = "Ticket Status")]
        public int TicketStatusId { get; set; }

        [Display(Name = "Ticket Priority")]
        public int TicketPriorityId { get; set; }

        [Display(Name = "Submitter")]
        public string OwnerUserId { get; set; }

        [Display(Name = "Developer")]
        public string AssignedToUserId { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        //nav properties
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual Project Project { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }

        //Constructor
        public Ticket()
        {
            TicketComments = new HashSet<TicketComment>();
            TicketHistories = new HashSet<TicketHistory>();
            TicketNotifications = new HashSet<TicketNotification>();
            TicketAttachments = new HashSet<TicketAttachment>();
        }
    }
}