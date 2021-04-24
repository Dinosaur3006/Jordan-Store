using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class OrderViewModel
    {
        public int ID { get; set; }
        public int Product_ID { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
        public string Date { get; set; }
        public int Customer_ID { get; set; }
        public int SumTotal { get; set; }
        public int QuantityTotal { get; set; }

    }
}