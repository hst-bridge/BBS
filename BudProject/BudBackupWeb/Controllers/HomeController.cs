using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace budbackup.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {
        public void Index()
        {
            ViewData["Message"] = "ASP.NET MVC へようこそ";
            Redirect("Account/index");

            //return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
