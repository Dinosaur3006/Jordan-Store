using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [OrAuthorization(Roles = "Admin")]
    public class MProductController : Controller
    {
        JordanEntities db = new JordanEntities();
        // GET: MProduct
        public ActionResult Index()
        {
            if (Session["Admin"] != null)
            {
                //var Products = db.Products.Include(db.Categories.ToString());
                //var ProductM = db.Products.Include(db.Supplies.ToString());
                var model = db.Products.ToList();
                foreach (var item in model)
                {
                    item.Product_Discount = item.Product_Price * (1 - item.Product_Discount);
                }
                return View(db.Products.ToList());
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
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(product);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult Create()
        {
            if (Session["Admin"] != null)
            {
                ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name");
                ViewBag.Supply_ID = new SelectList(db.Supplies, "Supply_ID", "Supply_Name", "Supply_Address");

                return View();
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        private bool isValidContentType(string contenType)
        {
            return contenType.Equals("image/png") || contenType.Equals("image/jpg") || contenType.Equals("image/jpeg") || contenType.Equals("image/gif");
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_Name,Product_Price,Product_Date,Product_Image," +
            "Product_Description,Product_Discount,Category_ID,Supply_ID, Quantity")] Product product, HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null)
                {
                    if (!isValidContentType(photo.ContentType))
                    {
                        TempData["ErrorImage"] = "The image wrong type, please check again !";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var fileName = new FileInfo(photo.FileName); ;
                        //var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        photo.SaveAs(Server.MapPath("~/Images/" + fileName));
                        //ViewBag.fileName = photo.FileName;
                        product.Product_Image = photo.FileName;

                    }
                }

                else
                {
                    TempData["ErrorImage"] = "The image wrong type, please check again !";
                    return RedirectToAction("Index");
                }
                  
                product.Product_Date = DateTime.Now;
                db.Products.Add(product);
                var result=db.SaveChanges();
                if (result > 0) {
                    TempData["CreateProduct"] = "Create Product Successful";
                   
                }
                return RedirectToAction("Index");

            }
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name", product.Category_ID);
            ViewBag.Supply_ID = new SelectList(db.Supplies, "Supply_ID", "Supply_Name", "Supply_Address");
            return View(product);
    }
        public ActionResult Edit(int? id = new int?())
        {
            if (Session["Admin"] != null)
            {
                if (id == null)
                {

                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
            ViewBag.Supply_ID = new SelectList(db.Supplies, "Supply_ID", "Supply_Name", "Supply_Address");
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name", product.Category_ID);
            return View(product);
            }
            return RedirectToAction("Login", "HomeAdmin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_Name,Product_Price,Product_Date,Product_Image," +
            "Product_Description,Product_Discount,Category_ID,Supply_ID, Quantity, Product_ID")] Product product, HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null)
                {
                    if (!isValidContentType(photo.ContentType))
                    {
                        TempData["ErrorImage"] = "The image wrong type, please check again !";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var fileName = new FileInfo(photo.FileName);
                    photo.SaveAs(Server.MapPath("~/Images/" + fileName));
                    product.Product_Image = photo.FileName;
                    }
                }
               
            
                db.Entry(product).State = EntityState.Modified;
              var result =  db.SaveChanges();
                if (result > 0)
                {
                    TempData["EditProduct"] = "Edit Product Successful";

                }
                return RedirectToAction("Index");
            }  
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name", product.Category_ID);
            ViewBag.Supply_ID = new SelectList(db.Supplies, "Supply_ID", "Supply_Name", "Supply_Address");
           
             return View(product);
        }

        public ActionResult Delete(int? id = new int?())
        {
            
            
            if (Session["Admin"] != null)
            {
                if (id == null)
                {

                    return RedirectToAction("PageError", "HomeAdmin");
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return RedirectToAction("PageError", "HomeAdmin");
                }
                return View(product);
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
                Product product = db.Products.Find(id);
                db.Products.Remove(product);
                var result = db.SaveChanges();
                if (result > 0)
                {
                    TempData["DeleteProduct"] = "Delete Product Successful";

                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorDeleteProduct"] = "Cannot delete this product.";
                return RedirectToAction("Index");
            }
        }

       

    }
}