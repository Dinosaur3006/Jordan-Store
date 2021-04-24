using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
namespace WebApplication.Controllers
{
    [OrAuthorization(Roles = "Admin")]
    public class MSupplyController : Controller
    {
        // GET: MSupply
        JordanEntities db = new JordanEntities();
        public ActionResult Index()
        {
            if (Session["Admin"] != null)
            {
                return View(db.Supplies.ToList());
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult Details(int id)
        {

            if (Session["Admin"] != null)
            {
                if (id == null)
                {

                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Supply supply = db.Supplies.Find(id);
                if (supply == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(supply);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        public ActionResult Create()
        {
            if (Session["Admin"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Supply_Name,Supply_Address")] Supply supply)
        {
            if (ModelState.IsValid)
            {
                db.Supplies.Add(supply);
                var result = db.SaveChanges();
                if (result > 0)
                {
                    TempData["CreateSupply"] = "Create supply successful.";
                }
                return RedirectToAction("Index");
            }

            return View(supply);
        }

        public ActionResult Edit(int id)
        {
            if (Session["Admin"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Supply supply = db.Supplies.Find(id);
                if (supply == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(supply);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Supply_ID,Supply_Name,Supply_Address")] Supply supply)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supply).State = EntityState.Modified;
               var result= db.SaveChanges();
                if (result > 0)
                {
                    TempData["EditSupply"] = "Edit supply successful.";
                }
                return RedirectToAction("Index");
            }
            return View(supply);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int id)
        {
            if (Session["Admin"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Supply supply = db.Supplies.Find(id);
                if (supply == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(supply);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Supply supply = db.Supplies.Find(id);
                db.Supplies.Remove(supply);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["ErrorDeleteSupply"] = "Cannot delete this supply.";
                return RedirectToAction("Index");
            }
        }
    }
}