using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Common;
using WebApplication.DAO;
using WebApplication.Models;
using System.Data.Entity;
using System.Net;
using PayPal.Api;
using System.Configuration;
using Common;
namespace WebApplication.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        JordanEntities db = new JordanEntities();
        public string strCart = "Cart";
        public ActionResult Index()
        {
         

            return View();

        }
        public ActionResult OrderProduct(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(Session[strCart] == null)
            {
                List<CartItem> ListCart = new List<CartItem>
                {
                    new CartItem(db.Products.Find(id),1)
                };
                Session[strCart] = ListCart;
            }
            else
            {
                List<CartItem> ListCart = (List<CartItem>)Session[strCart];
                int check = isExistProduct(id);
                if (check == -1)
                {
                    ListCart.Add(new CartItem(db.Products.Find(id), 1));
                }
                else
                {
                    ListCart[check].Quantity++;
                }
               
                               
                Session[strCart] = ListCart;
            }
            return View("Index");
        }
        private int isExistProduct(int? id)
        {
            List<CartItem> ListCart = (List<CartItem>)Session[strCart];
            for(int i=0; i< ListCart.Count; i++)
            {
                if (ListCart[i].Product.Product_ID == id) return i;
            }
            return -1;
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int check = isExistProduct(id);
            List<CartItem> ListCart = (List<CartItem>)Session[strCart];
            ListCart.RemoveAt(check);
            return View("Index");
        }
        public ActionResult UpdateCart(FormCollection frc)
        {
            string [] quantities = frc.GetValues("quantity");
            List<CartItem> ListCart = (List<CartItem>)Session[strCart];
            for (int i = 0; i < ListCart.Count; i++)
            {
                ListCart[i].Quantity = Convert.ToInt32(quantities[i]);
            }
            Session[strCart] = ListCart;
            return View("Index");
        }
        public ActionResult CheckOut()
        {
          
            return View("CheckOut");
        }

        public ActionResult ProcessOrder()
        {
              
            List<CartItem> ListCart = (List<CartItem>)Session[strCart];
         
        // save order
       var order = new WebApplication.Models.Order();
            UserLogin cus = (UserLogin)Session[CommonConstants.USER_SESSION];
            order.Order_Date = DateTime.Now;
            
                order.Customer_ID = cus.Customer_ID;
                order.Status = "Pending";
          

                // save od
                foreach (CartItem cart in ListCart)
            {
                OrderDetail orderdetail = new OrderDetail()
                {
                    Quantity = cart.Quantity,
                    Product_Price = cart.Product.Product_Price,
                    Product_Discount = cart.Product.Product_Discount,
                    Product_ID = cart.Product.Product_ID,
                    Order_ID = order.Order_ID
                };
                var model = db.Products.Single(x => x.Product_ID == cart.Product.Product_ID);

                if (cart.Quantity > model.Quantity)
                {

                return RedirectToAction("Fail", "Cart");
                }
                else
                {
                    db.OrderDetails.Add(orderdetail);
                    db.Orders.Add(order);
                                     db.SaveChanges();
                    
                    model.Quantity = model.Quantity - orderdetail.Quantity;
                    db.SaveChanges();
                }
               
            }
          
          
            Session.Remove(strCart);

            return View("Success");
        }
       

       public ActionResult Clear()
        {
            Session.Remove(strCart);
            return View("Index");
        }

    
        public ActionResult Success()
        {

            return View();
        }

        public ActionResult Fail()
        {

            return View();
        }

        private Payment payment;

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var listItems = new ItemList() { items = new List<Item>() };

            List<CartItem> listCarts = (List<CartItem>)Session[strCart];
            foreach(var cart in listCarts)
            {
                var total = cart.Product.Product_Price - ((cart.Product.Product_Price * cart.Product.Product_Discount) / 100);
                listItems.items.Add(new Item()
                {
                    name = cart.Product.Product_Name, 
                    currency = "USD",
                    price = total.ToString(), 
                    quantity = cart.Quantity.ToString(), 
                    sku = "sku"
                });
            }

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "1",
                shipping = "2",
               subtotal = listCarts.Sum(x => x.Quantity * (x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100))).ToString()
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal)).ToString(),
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = " Watches",
                invoice_number = Convert.ToString((new Random()).Next(100000)), 
                amount = amount,
                item_list = listItems

            });

            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }
     
    private Payment ExcutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }

       public ActionResult PaymentWithPaypal()
        {
            var session = Session[CommonConstants.USER_SESSION];
            if (session == null || session == " ")
            {
                return RedirectToAction("Login", "User");
            }
        
            List<CartItem> ListCart = (List<CartItem>)Session[strCart];

            // save order
            var order = new WebApplication.Models.Order();
            UserLogin cus = (UserLogin)Session[CommonConstants.USER_SESSION];
            order.Order_Date = DateTime.Now;

            order.Customer_ID = cus.Customer_ID;
            order.Status = "Pending";
            


            // save od
            foreach (CartItem cart in ListCart)
            {
                OrderDetail orderdetail = new OrderDetail()
                {
                    Quantity = cart.Quantity,
                    Product_Price = cart.Product.Product_Price,
                    Product_Discount = cart.Product.Product_Discount,
                    Product_ID = cart.Product.Product_ID,
                    Order_ID = order.Order_ID
                };
                TempData["Quan"] = cart.Quantity;
                TempData["PN"]= cart.Product.Product_Name;
                TempData["pri"] = cart.Product.Product_Price - ((cart.Product.Product_Price * cart.Product.Product_Discount) / 100);
                TempData["Tot"]= (cart.Product.Product_Price - ((cart.Product.Product_Price * cart.Product.Product_Discount) / 100))*cart.Quantity;
               
                var model = db.Products.Single(x => x.Product_ID == cart.Product.Product_ID);

                if (cart.Quantity > model.Quantity)
                {

                    return RedirectToAction("Fail", "Cart");
                }

                db.OrderDetails.Add(orderdetail);
                model.Quantity = model.Quantity - orderdetail.Quantity;
            }
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Cart/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = string.Empty;

                    while (links.MoveNext())
                    {
                        Links link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executePayment = ExcutePayment(apiContext, payerId, Session[guid] as string);
                    if(executePayment.state.ToLower() != "approved")
                    {
                        return View("Fail");
                    }
                }
            }
            catch (Exception ex)
            {
                PaypalLogger.Log("Error: " + ex.Message);
                          return View("Fail");
            }
          Session.Remove(strCart);
            // Send mail
            db.Orders.Add(order);
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Theme/client/template/order.html"));

            content = content.Replace("{{ProductName}}", TempData["PN"].ToString());
            content = content.Replace("{{Quantity}}", TempData["Quan"].ToString());
            content = content.Replace("{{Price}}", TempData["pri"].ToString());
            content = content.Replace("{{Total}}", TempData["Tot"].ToString());
            content = content.Replace("{{Date}}", DateTime.Now.ToString());
            content = content.Replace("{{CustomerName}}", cus.Username);
            content = content.Replace("{{FullName}}", cus.Customer_Name);
            var toEmail = cus.Email;


            new MailHelper().SendMail(toEmail, "Jordan Shop Receipt", content);
            db.SaveChanges();

        

            return View("Success");
        }
       
       
    }
}