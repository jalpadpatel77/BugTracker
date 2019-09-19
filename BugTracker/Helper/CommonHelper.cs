using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.Helper
{
    public abstract class CommonHelper
    {
        protected static ApplicationDbContext db = new ApplicationDbContext();
        protected static UserRolesHelper roleHelper = new UserRolesHelper();
        protected static ProjectsHelper ProjectHelper = new ProjectsHelper();

       
    }
}