using PaD.DAL;
using PaD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PaD.Controllers
{
    public class ThumbnailController : Controller
    {
        // GET: Thumbnail
        public ActionResult Index()
        {
            return View();
        }
    }
}