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
            Response.Cookies["userId"].Expires = DateTime.Now.AddDays(-1);
            Session["userId"] = null;
            ViewBag.Loginned = null;
            return RedirectToAction("Products", "products");
        }



    }
}