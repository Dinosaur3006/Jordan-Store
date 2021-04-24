using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models.DTO
{
    public class Products
    {
        [Display(Name = "ID")]
        public int Product_ID { get; set; }
        [Display(Name = "Product")]
        public string Product_Name { get; set; }
        [Display(Name = "Image")]
        public double Product_Price { get; set; }
        [Display(Name = "Price")]
        public string Product_Date { get; set; }
        [Display(Name = "Details")]
        public string Product_Image { get; set; }
        [Display(Name = "Category")]
        public string Product_Description { get; set; }
        public string Product_Comment { get; set; }
        public double Product_Discount { get; set; }

        public int Category_ID { get; set; }
        public string Supply_ID { get; set; }
        public int Brand_ID { get; set; }
        public string Image_Detail1 { get; set; }
        public string Image_Detail2 { get; set; }
    }
}