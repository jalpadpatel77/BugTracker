using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //Use this as my Login page
        public ActionResult Index()
       {
            return View();
        }
        
        //use this for my Register page
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult DemoUser()
        {

            return View();
        }
        public ActionResult DashBoard()
        {
           
            var bugdata = new BugDataViewModel();
            bugdata.myProjects = db.Projects.ToList();
            bugdata.myTickets = db.Tickets.ToList();
            bugdata.myusers = db.Users.ToList();

               return View(bugdata);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();
            return View(model);

           // ViewBag.Message = "Your contact page.";
           // return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p >";
                    var from = $"{model.FromEmail}<jalpadpatel77@gmail.com>";
                    model.Body = "This is a message from your portfolio site.  The name and the email of the contacting person is above.";

                    var email = new MailMessage(from, WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.Subject,
                        Body = string.Format(body, model.FromName, model.FromEmail,
                                             model.Body),

                        IsBodyHtml = true
                    };  //Body = model.Body
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }

    }
}