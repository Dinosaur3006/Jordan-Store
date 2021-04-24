using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ShoppingCart
    {
        public static ShoppingCart Cart
        {
            get{
               var cart = HttpContext.Current.Session["Cart"] as ShoppingCart;
            
                if (cart == null)
                {
                    cart = new ShoppingCart();
        HttpContext.Current.Session["Cart"] = cart ;
                }
                return cart;
            }
        }

   
        public List<Product> Items = new List<Product>();

        public void Add(int id)
        {
            try
            {
                var item = Items.Single(i => i.Product_ID == id);
                item.Quantity++;
            }
            catch 
            {
                var db = new JordanEntities();
                var item = db.Products.Find(id);
                item.Quantity = 1;
                Items.Add(item);
            }
        }

        public void Remove(int id)
        {
            var item = Items.Single(i => i.Product_ID == id);
            Items.Remove(item);
        }

        public void Update(int id, int newQuantity)
        {
            var item = Items.Single(i => i.Product_ID == id);
            item.Quantity = newQuantity;
            
        }

        public void Clear()
        {
            Items.Clear();
        }

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public double Total
        {
            get
            {
                return Items.Sum(p => (p.Product_Price - ((p.Product_Price * p.Product_Discount) / 100)) * p.Quantity);
            }
        }
    }

}