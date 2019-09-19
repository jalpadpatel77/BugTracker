using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helper;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class UserProfileViewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserProfile
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult EditProfile()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myUserViewModel = new UserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            };
            return View(myUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserProfileViewModel user, HttpPostedFileBase Avatar)
        {
            var newUser = db.Users.Find(user.Id);

            newUser.FirstName = user.FirstName;
            newUser.LastName = user.LastName;
            newUser.DisplayName = user.DisplayName;
            newUser.Email = user.Email;
            newUser.UserName = user.Email;

            if(ImageHelpers.IsWebFriendlyImage(user.Avatar))
            {
                var fileName = Path.GetFileName(user.Avatar.FileName);
                user.Avatar.SaveAs(Path.Combine(Server.MapPath("~/Avatars/"), fileName));
                user.AvatarUrl = "/Avatars/" + fileName;
              
            }
            newUser.AvatarUrl = user.AvatarUrl;
            db.SaveChanges();

            return RedirectToAction("DashBoard","Home");
        }
        [Authorize]
        public ActionResult MyProfile()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myUserViewModel = new UserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            };
            return View(myUserViewModel);
        }
    }
}