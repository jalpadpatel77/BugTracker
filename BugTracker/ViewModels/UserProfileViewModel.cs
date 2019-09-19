using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }

        [MaxLength(50,ErrorMessage ="First Name cannot be greater than 50 characters")]
        [MinLength(1, ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Last Name cannot be greater than 50 characters")]
        [MinLength(1, ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(50, ErrorMessage = "Display Name cannot be greater than 50 characters")]
        [MinLength(1, ErrorMessage = "Display Name is required")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Avatar path")]
        public string AvatarUrl { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public HttpPostedFileBase Avatar { get; set; }
               
    }
}