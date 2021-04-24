using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using PagedList;

namespace WebApplication.Controllers
{
    public class ProductController : Controller
    {
        JordanEntities db = new JordanEntities();
        // GET: Product
        public ActionResult Product()
        {
            return View();
        }
        public ActionResult OtherCategory()
        {
            return View(db.Categories.ToList());
        }
        public ActionResult Category(int? page,string sort, int CategoryId=0)
        {
            //var dao = db.Products.ToList();
            //var model1 = dao.ToPagedList(page, pageSize);
            int pageSize = 8;
            int pageNumber = (page ?? 1);
          
            var model = db.Products.Where(p => p.Category_ID == CategoryId).OrderByDescending(x => x.Product_Price);
            if (CategoryId == 0)
            {
                return RedirectToAction("Error", "Page");
            }
            if (CategoryId!=0)
            {
                
                if (sort == "h->l")
                {
                    var model1 = model.OrderByDescending(a => a.Product_Price);
                    return View(model1.ToPagedList(pageNumber, pageSize));
                }
                else if (sort == "l->h")
                {
                    var model1 = model.OrderBy(a => a.Product_Price);
                    return View(model1.ToPagedList(pageNumber, pageSize));
                }
                return View(model.ToPagedList(pageNumber, pageSize));
            }
            
            return View();
        }
        public ActionResult Detail(int? id)
        {
            var vote = db.Votes.ToList();
            if (id == null)
            {
                return RedirectToAction("Error", "Page");
            }


            var model = db.Products.Find(id);
            TempData["IDDetails"] = id;
            foreach(var test in vote)
            {
                if (model.Product_ID == test.Product_ID)
                {
                    ViewBag.Rated = "Your account has rated";

                }
            }
            

                if (model == null)
                {
                    return RedirectToAction("Error", "Page");
                }
            
           
            
            return View(model);
        }
        public ActionResult Search(String Keywords="")
        {
            if(Keywords!="")
            {
                var model = db.Products.Where(p => p.Product_Name.Contains(Keywords));
               
                return View(model);
            }
            return View(db.Products);
        }

        [HttpPost]
        public ActionResult Rating([Bind(Include = "RateID,RateStars,Customer_ID,Product_ID")] int rating, Vote vote)
        {
          

                var session = (WebApplication.Common.UserLogin)Session[WebApplication.Common.CommonConstants.USER_SESSION];
                if (session != null)
                {
               
                if (ModelState.IsValid)
                    {
                        vote.RateStars = rating;
                        vote.Customer_ID = session.Customer_ID;
                        vote.Product_ID = (int)TempData["IDDetails"];
                        db.Votes.Add(vote);
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            TempData["CreateProduct"] = "Create Product Successful";

                        }
                        return RedirectToAction("Detail", new { id = (int)TempData["IDDetails"] });
                    }
                    return RedirectToAction("Error", "Page");
               
            }
                return RedirectToAction("Login", "User");
            }
        
        

     


    }
}