using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AddPController : Controller
    {
        JordanEntities db = new JordanEntities();
        // GET: AddP
        public ActionResult AddressPro(int CategoryId = 0)
        {
            if (CategoryId != 0)
            {
                var model = db.Categories.Where(p => p.Category_ID == CategoryId);
              
                return View(model);
            }
            return View();
        }

        public ActionResult AddressDetail(int id)
        {
            var model = db.Products.Find(id);
            return View(model);
        }
    }
}