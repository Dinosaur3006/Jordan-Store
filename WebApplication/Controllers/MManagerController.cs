using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Common;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class MManagerController : Controller
    {
        JordanEntities db = new JordanEntities();
        // GET: MManager
        public ActionResult Index()
        {
            if (Session["Admin"] != null)
            {
                return View(db.Administrators.ToList());
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        [HttpGet]
        public ActionResult Register()
        {

            if (Session["Admin"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        [HttpPost]
        public ActionResult Register(AdminModel model)
        {
            if (ModelState.IsValid)
            {
                var dao = new AdminDAO();


                if (dao.CheckUserName(model.Username))
                {
                    //ModelState.AddModelError("", "UserName already exists.");
                    ViewBag.Error = "Username already exists.";
                }
                else
                {
                    var admin = new Administrator();
                    admin.Username = model.Username;
                    admin.Password = Encryptor.MD5Hash(model.Password);
                    admin.FullName = model.FullName;
                    admin.Role = model.Role = "Manager";
                    admin.Email = model.Email;
                    admin.Phone = model.Phone;

                    var result = dao.Insert(admin);

                    if (result > 0)
                    {
                        ViewBag.Success = "Register Successful.";
                        model = new AdminModel();

                        return RedirectToAction("Index", "MManager");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Register Fail");
                    }
                }



                return View(model);
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
                Administrator manager = db.Administrators.Find(id);
                if (manager == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(manager);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        public ActionResult Edit(int? id = new int?())
        {
            if (Session["Admin"] != null)
            {
                if (id == null)
                {

                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Administrator manager = db.Administrators.Find(id);
                if (manager == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(manager);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Account_ID, Username,Password,FullName,Role,Email,Phone")] Administrator admin)
        {
            if (ModelState.IsValid)
            {
              


                db.Entry(admin).State = EntityState.Modified;
                var result = db.SaveChanges();
                if (result > 0)
                {
                    TempData["EditManager"] = "Edit profile of manager successful.";

                }
                return RedirectToAction("Index");
            }
       

            return View(admin);
        }
        public ActionResult Delete(int? id = new int?())
        {


            if (Session["Admin"] != null)
            {
                if (id == null)
                {

                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Administrator manager = db.Administrators.Find(id);
                if (manager == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(manager);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Administrator manager = db.Administrators.Find(id);
                db.Administrators.Remove(manager);
                var result = db.SaveChanges();
                if (result > 0)
                {
                    TempData["DeleteManager"] = "Delete account of manager successful.";

                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorDeleteManager"] = "Cannot delete this account.";
                return RedirectToAction("Index");
            }
        }
    }
}