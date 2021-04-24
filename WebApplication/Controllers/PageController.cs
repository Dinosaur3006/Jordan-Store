using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult Error()
        {
            return View();
        }
    }
}