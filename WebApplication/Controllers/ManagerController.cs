using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using System.Data.Entity;
using Common;

namespace WebApplication.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        JordanEntities db = new JordanEntities();
        [OrAuthorization(Roles = "Manager")]
        public ActionResult Index()
        {
            if (Session["Manager"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult PageError()
        {
            return View();
        }
        public ActionResult Product()
        {
            if (Session["Manager"] != null)
            {

                var model = db.Products.ToList();

                return View(db.Products.ToList());
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult Details(int? id = new int?())
        {

            if (Session["Manager"] != null)
            {
                if (id == null)
                {

                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return RedirectToAction("PageError", "Manager");
                }
                return View(product);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult CheckOrders()
        {

            if (Session["Manager"] != null)
            {
                var order = db.Orders.ToList();
                return View(order.Where(x => x.Status == "Pending" || x.Status == "Processing"));

            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult DetailsOrders(int? id = new int?())
        {
            if (Session["Manager"] != null)
            {
                if (id == null)
                {

                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Order detailsOrders = db.Orders.Find(id);

                if (detailsOrders == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(detailsOrders);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        [HttpPost]
        public ActionResult EditStatus([Bind(Include = "Order_Date,Customer_ID,Status,Order_ID")] string submit, Order order)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Accepted":

                        order.Status = "Processing";
                        db.Entry(order).State = EntityState.Modified;
                        var data = db.Orders.Include(x => x.Customer);
                        var check = data.Where(x => x.Order_ID == order.Order_ID).ToList();
                        foreach (var item in check)
                        {
                            if (item.Customer_ID == item.Customer.Customer_ID)
                            {
                                TempData["customerus"] = item.Customer.Username;
                                TempData["emailcus"] = item.Customer.Email;
                                TempData["fullnamecus"] = item.Customer.Customer_Name;
                            }
                            else
                            {
                                return View("CheckOrders");
                            }
                        }
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            TempData["statusaccepted"] = "This order has been accepted";

                        }
                        string content = System.IO.File.ReadAllText(Server.MapPath("~/Theme/client/template/approve.html"));
                        content = content.Replace("{{FullName}}", TempData["fullnamecus"].ToString());
                        content = content.Replace("{{Username}}", TempData["customerus"].ToString());
                        var toEmail = TempData["emailcus"].ToString();
                        if(toEmail != null && toEmail != "")
                        {
                            new MailHelper().SendMail(toEmail, "Jordan Sneaker Receipt", content);
                        }
                        return RedirectToAction("CheckOrders");
                        break;
                    case "Completed":
                        order.Status = "Completed";
                        db.Entry(order).State = EntityState.Modified;
                        var data1 = db.Orders.Include(x => x.Customer);
                        var check1 = data1.Where(x => x.Order_ID == order.Order_ID).ToList();
                        foreach (var item in check1)
                        {
                            if (item.Customer_ID == item.Customer.Customer_ID)
                            {
                                TempData["customerus"] = item.Customer.Username;
                                TempData["emailcus"] = item.Customer.Email;
                                TempData["fullnamecus"] = item.Customer.Customer_Name;
                            }
                            else
                            {
                                return View("CheckOrders");
                            }
                        }
                        var result1 = db.SaveChanges();
                        if (result1 > 0)
                        {
                            TempData["statuscompleted"] = "This order has been completed";

                        }
                        string content1 = System.IO.File.ReadAllText(Server.MapPath("~/Theme/client/template/complete.html"));
                        content = content1.Replace("{{FullName}}", TempData["fullnamecus"].ToString());
                        content = content.Replace("{{Username}}", TempData["customerus"].ToString());
                        var toEmail1 = TempData["emailcus"].ToString();
                        if (toEmail1 != null && toEmail1 != "")
                        {
                            new MailHelper().SendMail(toEmail1, "Jordan Sneaker Receipt", content);
                        }

                        return RedirectToAction("CheckOrders");
                        break;
                    case "Rejected":
                        order.Status = "Rejected";
                        db.Entry(order).State = EntityState.Modified;
                        TempData["OrderID"] = order.Order_ID;

                        return RedirectToAction("RejectedOrders");
                        break;
                    default:
                        throw new Exception();

                }

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        public ActionResult RejectedOrders()
        {
            try
            {
                if (Session["Manager"] != null)
                {
                    int id = (int)TempData["OrderID"];
                    if (id == null)
                    {

                        return RedirectToAction("PageError", "HomeAdmin");
                    }

                    Order detailsOrders = db.Orders.Find(id);

                    if (detailsOrders == null)
                    {
                        return RedirectToAction("PageError", "HomeAdmin");
                    }
                    return View(detailsOrders);
                }
                return RedirectToAction("Login", "HomeAdmin");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Page");
            }
        }
        [HttpPost]
        public ActionResult RejectedOrders([Bind(Include = "Order_Date,Customer_ID,Status,Note,Order_ID")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Status = "Rejected";
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                var data2 = db.Orders.Include(x => x.Customer);
                var check2 = data2.Where(x => x.Order_ID == order.Order_ID).ToList();
                foreach (var item in check2)
                {
                    if (item.Customer_ID == item.Customer.Customer_ID)
                    {
                        TempData["customerus"] = item.Customer.Username;
                        TempData["emailcus"] = item.Customer.Email;
                        TempData["fullnamecus"] = item.Customer.Customer_Name;
                    }
                    else
                    {
                        return View("CheckOrders");
                    }
                }
                TempData["note"] = order.Note;
                var result2 = db.SaveChanges();
                string content2 = System.IO.File.ReadAllText(Server.MapPath("~/Theme/client/template/rejected.html"));
                content2 = content2.Replace("{{FullName}}", TempData["fullnamecus"].ToString());
                content2 = content2.Replace("{{Username}}", TempData["customerus"].ToString());
                content2 = content2.Replace("{{Note}}", TempData["note"].ToString());
                var toEmail2 = TempData["emailcus"].ToString();
                if (toEmail2 != null && toEmail2 != "")
                {
                    new MailHelper().SendMail(toEmail2, "Jordan Sneaker Receipt", content2);
                }
                return RedirectToAction("CheckOrders");
            }
            return View("CheckOrders");
        }

        public ActionResult CustomerDetails(int? id = new int?())
        {

            if (Session["Manager"] != null)
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

        public ActionResult ManageOrders()
        {

            if (Session["Manager"] != null)
            {
                var order = db.Orders.ToList();
                return View(order.Where(x => x.Status == "Completed"));

            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult OrdersRejected()
        {

            if (Session["Manager"] != null)
            {
                var order = db.Orders.ToList();
                return View(order.Where(x => x.Status == "Rejected"));

            }
            return RedirectToAction("Login", "HomeAdmin");
        }
    }
}