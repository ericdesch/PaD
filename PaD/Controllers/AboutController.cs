using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaD.Controllers
{
    public class AboutController : Controller
    {
        // GET: About
        public ActionResult Index()
        {
            return View();
        }

        // GET: About/Terms
        public ActionResult Terms()
        {
            return View();
        }

        // GET: About/Privacy
        public ActionResult Privacy()
        {
            return View();
        }

        // GET: About/Contact
        public ActionResult Contact()
        {
            return View();
        }
    }
}