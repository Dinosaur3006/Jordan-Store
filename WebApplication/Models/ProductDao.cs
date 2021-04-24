using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Models
{

    public class ProductDao
    {
        JordanEntities db = null;
       
        public ProductDao()
        {
            db = new JordanEntities();
        }
        public Product ViewDetail(long id)
        {
            return db.Products.Find(id);
        }
    }
}