using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
namespace WebApplication.Controllers
{
    [OrAuthorization(Roles = "Admin")]
    public class MCustomerController : Controller
    {
        JordanEntities db = new JordanEntities();
        // GET: MCustomer
        public ActionResult Index()
        {
            if (Session["Admin"] != null)
            {
                return View(db.Customers.ToList());
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult Details(int? id = new int?())
        {

            if (Session["Admin"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Customer customer = db.Customers.Find(id);
                if (customer == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(customer);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

    }
}