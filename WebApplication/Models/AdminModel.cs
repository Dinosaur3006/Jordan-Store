using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AdminModel
    {
        public int Account_ID { get; set; }
        public string Username { set; get; }
        public string Password { set; get; }
        public string FullName { set; get; }
        public string Role { get; set; }
        public string Email { set; get; }
        public string Phone { get; set; }
    }
}