    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Areas.Controllers
{
    public class HomeAdminController : Controller
    {
        // GET: Controllers/Home
        JordanEntities db = new JordanEntities();
        public ActionResult Index()
        {
            var user = Session["Admin"];
            if (Session["Admin"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
       
        [AllowAnonymous]
        public ActionResult Login()
        {
            Session.RemoveAll();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                string pass = Encript.MD5Hash(password);

                var obj = db.Administrators.Where(a => a.Username.Equals(username) && a.Password.Equals(pass) && a.Role.Equals("Admin")).FirstOrDefault();
                var obj1 = db.Administrators.Where(a => a.Username.Equals(username) && a.Password.Equals(pass) && a.Role.Equals("Manager")).FirstOrDefault();

                if (obj != null)    
                {
                    Session["Admin"] = obj.Username.ToString();
                    ////Session["IAdmin"] = obj.image;
                    return RedirectToAction("Index");
                }
                if (obj1 != null)
                {
                    Session["Manager"] = obj1.Username.ToString();
               
                    return RedirectToAction("Index","Manager");
                }
            }
            ViewBag.Error = "Wrong username or password, try again.";
         
            return View("Login");
        }
        public ActionResult Logout()
        {
            if (Session["Admin"] != null /*|| Session["User"] != null*/)
            {
                Session["Admin"] = null;
                return RedirectToAction("/");
                //Session["User"] = null;
            }
            return View("Login");
        }
        public ActionResult PageError()
        {
            return View();
        }
        //static WADEntities1 pe = new WADEntities1();
        //ArrayList xValue = new ArrayList();
        //ArrayList yValue = new ArrayList();
        //
        // GET: /Home/

        //public ActionResult ColumnChart()
        //{
        //    ViewBag.store_id = new SelectList(db.tblStores, "store_id", "store_name");

        //    var order = db.tblOrderDetails.Include(t => t.tblOrder).Include(t => t.tblProduct);

        //    order.ToList().ForEach(x => xValue.Add(x.tblOrder.order_date).ToString("dd/MM"));
        //    order.ToList().ForEach(y => yValue.Add(y.quantity).ToString("#,##"));

        //    new Chart(width: 800, height: 600)
        //    .AddTitle("Chart for report")
        //    .AddSeries("Default", chartType: "column", xValue: xValue, yValues: yValue)
        //            .Write("bmp");

        //    return null;
        //}
    }
}