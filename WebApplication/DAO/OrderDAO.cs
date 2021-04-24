using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.DAO
{ 
    
    public class OrderDAO
    {
        JordanEntities db = null;
        public OrderDAO()
        {
            db = new JordanEntities();
        }
        public long Insert(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();
            return order.Order_ID;
        }
    }
}