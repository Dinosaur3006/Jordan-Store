using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using PagedList;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        // GET: Homes
        JordanEntities db = new JordanEntities();
        public ActionResult Index()
        {
            var model = db.Products.Where(c => c.Category.Category_Name == "Collection");
      
            return View(model);
        }
       
        public ActionResult Construction()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
       
        public ActionResult Category()
        {
            
            return View(db.Categories.ToList());
        }

        public ActionResult Discount()
        {

            var model = db.Products.Where(p => p.Product_Discount > 0 & p.Product_Discount !=null);
          
            return View( model);
        }
        public ActionResult PGear()
        {
            var model = db.Products.Where(c => c.Category.Category_Name == "Collection");
       
          
            return View(model);
        }
        public ActionResult Helmet()
        {
            var model = db.Products.Where(c => c.Category.Category_Name == "Collection");
          

            return View(model);
        }
        public ActionResult About()
        {
            return View();
        }
 

        public ActionResult Search()
        {
            var name = Request["term"];
            var data = db.Products.Where(p => p.Product_Name.Contains(name)).Select(p => p.Product_Name).ToList();
       
            return Json(data, JsonRequestBehavior.AllowGet);
    
        }
        

    }
}