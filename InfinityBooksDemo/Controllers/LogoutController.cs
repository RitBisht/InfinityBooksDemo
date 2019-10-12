using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfinityBooksDemo.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult CleanDetails()
        {
            Request.Cookies["userId"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("UserLogin", "Login");
        }



    }
}