using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;

namespace WebApplication.Models
{
    public interface IAuthorizationFilter
    {
        void OnAuthorization(AuthorizationContext filterContext);
    }
}