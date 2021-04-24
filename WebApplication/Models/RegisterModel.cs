using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class RegisterModel
    {
        [Key]
        public int Customer_ID { get; set; }
     //   [Display(Name = "Username")]
        [Required(ErrorMessage = "Username cannot be blank.")]
        [RegularExpression("^[A-Za-z0-9]*$", ErrorMessage = "Invalid Username.")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "Username at least 4 characters.")]
        public string Username { set; get; }

     //   [Display(Name = "Password")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password at least 6 characters.")]
        [Required(ErrorMessage = "Password cannot be blank.")]
        public string Password { set; get; }

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { set; get; }

        [Display(Name = "Full Name")]
        [StringLength(70, MinimumLength = 2, ErrorMessage = "Name too short or too long.")]
        [RegularExpression("^[ A-Za-z]*$" , ErrorMessage = "Invalid Name.")]
        [Required(ErrorMessage = "Name cannot be blank.")]
        public string Customer_Name { set; get; }

        [StringLength(11, MinimumLength = 10, ErrorMessage = "Phone at least 10 -> 11 numbers.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid Phone.")]
        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Phone cannot be blank.")]
        public string Customer_Phone { set; get; }

        
        [Display(Name = "Address")]
        [RegularExpression("^[, A-Za-z0-9]*$", ErrorMessage = "Invalid Address.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Address too short or too long.")]
        [Required(ErrorMessage = "Address cannot be blank.")]
        public string Customer_Address { set; get; }

        [Display(Name = "Email")]
      
        [Required(ErrorMessage = "Email cannot be blank.")]
        public string Email { set; get; }
    }
}