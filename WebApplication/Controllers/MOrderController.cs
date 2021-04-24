using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using System.Data.Entity;

namespace WebApplication.Controllers
{
    [OrAuthorization(Roles = "Admin")]
    public class MOrderController : Controller
    {
        JordanEntities db = new JordanEntities();
        // GET: MOrder
        public ActionResult Index()
        {
            if (Session["Admin"] != null)
            {
                var od = db.OrderDetails;
              
                var order = db.OrderDetails.Include(x=> x.Product).Include(x => x.Order);

                return View(order.ToList());
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

       

    }
}