using BotDetect.Web.UI.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Common;
using WebApplication.DAO;
using System.Data.Entity;
using WebApplication.Models;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Facebook;
using System.Configuration;
using Vonage.Verify;
using Vonage;
using Vonage.Request;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using RestSharp;

namespace WebApplication.Controllers
{
    public class UserController : Controller
    {
        JordanEntities db = new JordanEntities();
        // GET: User
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDAO();
                if (dao.CheckUserName(model.Username))
                {
                    //ModelState.AddModelError("", "UserName already exists.");
                    ViewBag.Error = "Username already exists.";
                }
                else
                {
                    var user = new Customer();
                    user.Username = model.Username;
                    user.Password = Encryptor.MD5Hash(model.Password);
                    user.Customer_Name = model.Customer_Name;
                    user.Customer_Phone = model.Customer_Phone;
                    user.Customer_Address = model.Customer_Address;
                    user.Email = model.Email;

                    var result = dao.Insert(user);

                    if (result > 0)
                    {
                        ViewBag.Success = "Register Successful.";
                        model = new RegisterModel();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Register Fail");
                    }
                }
            }

            return View(model);
        }
        public ActionResult Logout()
        {
            Session[CommonConstants.USER_SESSION] = null;
            return Redirect("/");

        }
        public ActionResult Login()
        {

            return View();
        }

        public ActionResult History()
        {
            var session = (WebApplication.Common.UserLogin)Session[WebApplication.Common.CommonConstants.USER_SESSION];
            if (session == null)
            {
                return RedirectToAction("Login", "User");
            }
            var od = db.OrderDetails;

            var order = db.OrderDetails.Include(x => x.Product).Include(x => x.Order).Include(x => x.Order.Customer);
            var check = order.Where(x => x.Order.Customer_ID == session.Customer_ID);
            return View(check);

        }

        [HttpPost]
        public ActionResult Login(LoginM model)
        {

            if (ModelState.IsValid)
            {
                var dao = new UserDAO();
                var result = dao.Login(model.Username, Encryptor.MD5Hash(model.Password));
                if (result == 1)
                {
                    var user = dao.GetById(model.Username);
                    var userSession = new UserLogin();
                    userSession.Username = user.Username;
                    userSession.Customer_ID = user.Customer_ID;
                    userSession.Email = user.Email;
                    userSession.Customer_Name = user.Customer_Name;

                    Session.Add(CommonConstants.USER_SESSION, userSession);


                    return Redirect("/");
                }

                else if (result == 0)
                {
                    ViewBag.Error = "Invalid username or password, try again.";
                    //ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -2)
                {
                    ViewBag.Error = "Invalid username or password, try again.";
                    //ModelState.AddModelError("", "Mật khẩu không đúng.");
                }

            }

            return View(model);
        }


        public ActionResult Info(int? id = new int?())
        {
            var session = (WebApplication.Common.UserLogin)Session[WebApplication.Common.CommonConstants.USER_SESSION];
            if (session == null)
            {
                return RedirectToAction("Login", "User");
            }
            if (id == null)
            {

                return RedirectToAction("PageError", "HomeAdmin");
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return RedirectToAction("PageError", "HomeAdmin");
            }

            return View(customer);
        }

        public ActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Forgot(string username)
        {
            if (ModelState.IsValid)
            {
                var model = db.Customers.ToList();
                foreach (var item in model)
                {
                    if (username == item.Username)
                    {
                        TempData["resetusername"] = username;
                        return RedirectToAction("ResetPassword");
                    }

                }
                ViewBag.wrongusername = "No account found with that username.";

            }

            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> ResetPasswordAsync()
        {
            try
            {
                var model = db.Customers.ToList();
                foreach (var item in model)
                {
                    if (item.Username == TempData["resetusername"].ToString())
                    {
                        //Random random = new Random();
                        //int value = random.Next(1000, 9999);
                        //string message1 = value + " is your Activation Code for reset password";

                        //string message2 = HttpUtility.UrlEncode(message1);

                        //const string accountSid = "AC2cad34334f1bbdd735e632b1535d31a9";
                        //const string authToken = "98fcb2edb5bf861eecfd80052baa44e7";
                        //TwilioClient.Init(accountSid, authToken);

                        //// var to = new PhoneNumber(ConfigurationManager.AppSettings["+84934543482"]);
                        ////  var a = item.Customer_Phone.ToString();
                        //// const string to = a;
                        //var from = new PhoneNumber("+19189216242");

                        //var message = MessageResource.Create(
                        //    to: item.Customer_Phone,
                        //    from: from,
                        //    body: message1);
                        //Session["OTPCode"] = value;
                        //TempData["user"] = item.Username;
                        var client = new RestClient(
                            "https://http-api.d7networks.com/send?username=jccf1910&password=mv2YW29k&dlr-method=POST&dlr-url=https://4ba60af1.ngrok.io/receive&dlr=yes&dlr-level=3&from=smsinfo&content=Thisisthesamplecontentsenttotest&to=+84934543482");
client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AlwaysMultipartFormData = true;
                        IRestResponse response = client.Execute(request);
                        return RedirectToAction("VerifyOTP");
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["Wrongpass"] = "Your phone number of account is not supported !";
                return RedirectToAction("Forgot");
            }

            return View();


        }
        [HttpPost]
        public ActionResult VerifyOTP(int otpcode)
        {
            if ((int)Session["OTPCode"] == otpcode)
            {
                var model = db.Customers.FirstOrDefault();
                Session["user"] = TempData["resetusername"];
                return RedirectToAction("EditPassword");
            }
            else
            {
                return View("ResetPassword");
            }
            return View();
        }


        public ActionResult EditProfile(int? id = new int?())
        {
            try
            {
                var session = (WebApplication.Common.UserLogin)Session[WebApplication.Common.CommonConstants.USER_SESSION];
                if (session != null)
                {

                    if (id == null)
                    {

                        return RedirectToAction("Error", "Page");
                    }
                    Customer customer = db.Customers.Find(id);
                    if (customer == null)
                    {
                        return RedirectToAction("Error", "Page");
                    }
                    return View(customer);
                }
                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Page");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "Customer_ID, Username,Password,Customer_Name,Customer_Phone,Customer_Address,Email")] Customer customer)
        {
            try
            {
                db.Entry(customer).State = EntityState.Modified;
                var result = db.SaveChanges();
                TempData["EditCustomer"] = "Edit profile successful.";
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Page");
            }

        }

        public ActionResult ChangePassword(int? id = new int?())
        {
            try
            {
                var session = (WebApplication.Common.UserLogin)Session[WebApplication.Common.CommonConstants.USER_SESSION];

                if (session != null)
                {

                    if (id == null)
                    {
                        return RedirectToAction("Error", "Page");
                    }
                    Customer customer = db.Customers.Find(id);
                    TempData["Oldpassword"] = customer.Password;
                    if (customer == null)
                    {
                        return RedirectToAction("Error", "Page");
                    }
                    return View(customer);
                }
                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Page");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "Customer_ID, Username,Password,Customer_Name,Customer_Phone,Customer_Address, Email")] Customer customer, string newpassword, string confirmpassword, string oldpassword, int? id = new int?())
        {
            try
            {
                oldpassword = Encryptor.MD5Hash(oldpassword);
                if (oldpassword == TempData["Oldpassword"].ToString())
                {
                    if (newpassword == confirmpassword)
                    {
                        customer.Password = Encryptor.MD5Hash(newpassword);
                        db.Entry(customer).State = EntityState.Modified;
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            TempData["ChangePassword"] = "Your Password has changed successful, please login again";
                            Session[CommonConstants.USER_SESSION] = null;
                        }
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["changepass"] = "The password does not match, please try again.";
                        Customer customer1 = db.Customers.Find(id);
                        if (customer1 == null)
                        {
                            return RedirectToAction("PageError", "HomeAdmin");
                        }
                        return View("Info", customer1);
                    }
                }
                else
                {
                    Customer customer1 = db.Customers.Find(id);
                    if (customer1 == null)
                    {
                        return RedirectToAction("PageError", "HomeAdmin");
                    }
                    TempData["oldpass"] = "Old password is wrong, please try again.";
                    return View("Info", customer1);
                }

                return View();

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Page");
            }
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });


            var accessToken = result.access_token;

            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;

                // Get the user's information, like email, first name, middle name etc
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;

                var dao = new UserDAO();
                if (dao.CheckUserName(email))
                {
                    var id = dao.GetById(email);
                    var userSession = new UserLogin();
                    userSession.Username = id.Username;
                    userSession.Customer_ID = id.Customer_ID;
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    return Redirect("/");
                }

                var user = new Customer();
                user.Username = email;
                user.Customer_Name = firstname;
                user.Email = email;
                user.Customer_Phone = " ";
                user.Customer_Address = " ";
                user.Password = " ";

                var resultInsert = new UserDAO().InsertForFacebook(user);
                if (resultInsert > 0)
                {
                    var userSession = new UserLogin();
                    userSession.Username = user.Username;
                    userSession.Customer_ID = user.Customer_ID;

                    Session.Add(CommonConstants.USER_SESSION, userSession);

                    return RedirectToAction("RegisterInfo");
                }
            }
            return Redirect("/");
        }

        public ActionResult RegisterInfo()
        {
            try
            {

                var session = (WebApplication.Common.UserLogin)Session[WebApplication.Common.CommonConstants.USER_SESSION];

                var model = db.Customers;
                foreach (var item in model)
                {
                    if (session.Customer_ID == item.Customer_ID)
                    {
                        Customer cus = db.Customers.Find(item.Customer_ID);
                        return View(cus);
                    }

                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Page");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterInfo([Bind(Include = "Customer_ID, Username,Password, Email,Customer_Name,Customer_Phone,Customer_Address")] Customer customer)
        {


            try
            {
                db.Entry(customer).State = EntityState.Modified;
                var result = db.SaveChanges();


                return Redirect("/");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Page");
            }


        }
    }
}