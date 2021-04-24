using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Common
{
    public class UserLogin
    {
        public int Customer_ID{ set; get; }
        public string Username { set; get; }
        public string Email { get; set; }
        public string Customer_Name { get; set; }
    }
}