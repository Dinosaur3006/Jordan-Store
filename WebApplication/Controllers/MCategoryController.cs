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
    public class MCategoryController : Controller
    {
        // GET: MCategory
        JordanEntities db = new JordanEntities();

        public ActionResult Index()
        {
            if (Session["Admin"] != null)
            {
                return View(db.Categories.ToList());
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
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(category);
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
        public ActionResult Create([Bind(Include = "Category_Name")] Category category)
        {
            if (ModelState.IsValid)
            {if(category.Category_Name == "" || category.Category_Name == null)
                {
                    TempData["FailCreate"] = "Create item fail, please try again !";
                    return View();
                }
                db.Categories.Add(category);
                var result = db.SaveChanges();
                if (result > 0)
                {
                    TempData["Createcate"] = "Create category successful.";
                }
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Edit(int? id = new int?())
        {
            if (Session["Admin"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(category);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Category_ID,Category_Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                var result = db.SaveChanges();
                if (result > 0)
                {
                    TempData["EditCate"] = "Edit category successful.";
                }
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Admin"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
               Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(category);
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
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorDeleteCate"] = "Cannot delete this category.";
                return RedirectToAction("Index");
            }
        }
    }
}