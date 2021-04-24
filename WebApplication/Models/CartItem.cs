using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebApplication.Models
{
 
    public class CartItem
    {
        JordanEntities db = new JordanEntities();
        public Product Product { get; set; } 
     
        public int Quantity { get; set; }
       
        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        
        }
 

    }

}