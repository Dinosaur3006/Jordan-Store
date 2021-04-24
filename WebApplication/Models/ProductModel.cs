using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ProductModel
    {
        [Key]
        //}
        [StringLength(4, MinimumLength = 4, ErrorMessage = "The Product ID has only 4 characters.")]
        [Display(Name = "Product ID")]
        [Required(ErrorMessage = "The Product ID field is required.")]
        public int Product_ID { get; set; }

        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "The Product Name field is required.")]
        public string Product_Name { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price field is required.")]
        public double Product_Price { get; set; }

        //[Display(Name = "Date")]     
        public Nullable<System.DateTime> Product_Date { get; set; }

        //[Display(Name = "Image")]
        //[Required(ErrorMessage = "Image field is required.")]
        public string Product_Image { get; set; }

        //[Display(Name = "Description")]
        public string Product_Description { get; set; }
        public byte[] Product_Comment { get; set; }
        //[DisplayFormat(DataFormatString = "{0:C}")]

        //[Display(Name = "Discount")]
        public double Product_Discount { get; set; }

        //[Display(Name = "Category")]
        //[Required(ErrorMessage = "Category field is required.")]
        public int Category_ID { get; set; }


        //[Display(Name = "Supply")]
        //[Required(ErrorMessage = "Suplly field is required.")]
        public string Supply_ID { get; set; }

        public virtual Category Category { get; set; }
    
        public virtual Supply Supply { get; set; }
    }
}