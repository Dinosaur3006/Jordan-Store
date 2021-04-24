using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class LoginM
    {
        [Key]
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username cannot be blank.")]
        public string Username { set; get; }

        [Required(ErrorMessage = "Password cannot be blank.")]
        [Display(Name = "Password")]
        public string Password { set; get; }
    }
}