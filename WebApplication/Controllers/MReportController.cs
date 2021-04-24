using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using System.Data.Entity;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using OfficeOpenXml;

namespace WebApplication.Controllers
{
    [OrAuthorization(Roles = "Admin")]
    
    public class MReportController : Controller
    {
        JordanEntities db = new JordanEntities();
        // GET: MReport
        public ActionResult Index()
        {
            if (Session["Admin"] != null)
            {
                var od = db.OrderDetails;
               
                var order = db.OrderDetails.Include(x => x.Product).Include(x => x.Order).ToList();
                return View(order.Where(x => x.Order.Order_Date.Date == DateTime.Now.Date).ToList());
                }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult FromDate(DateTime? datefrom)
        {
            if (Session["Admin"] != null)
            {
                var od = db.OrderDetails;

                var order = db.OrderDetails.Include(x => x.Product).Include(x => x.Order).ToList();
                Session["FromDate"] = datefrom;
                return View(order.Where(x => x.Order.Order_Date.Date == datefrom).ToList());
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public ActionResult DateRange(DateTime? datefrom, DateTime? dateto)
        {
            if (Session["Admin"] != null)
            {

                var od = db.OrderDetails;
                Session["DateFrom"] = datefrom;
                Session["DateTo"] = dateto;
                var order = db.OrderDetails.Include(x => x.Product).Include(x => x.Order).ToList();
                return View(order.Where(x => x.Order.Order_Date <= dateto  && x.Order.Order_Date >= datefrom).ToList());
               
            }
            return RedirectToAction("Login", "HomeAdmin");
        }
        public void ExportReportByDay()
        {
            var order = db.OrderDetails.Include(x => x.Product).Include(x => x.Order).ToList();
            var check = order.Where(x => x.Order.Order_Date.Date == DateTime.Now.Date);
           var sumquantity = check.Sum(x => x.Quantity);
            var sumtotal = check.Sum(x => x.Quantity * (x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100)));
           List<OrderViewModel> emplist = (order.Where(x => x.Order.Order_Date.Date == DateTime.Now.Date)).ToList().Select(x => new OrderViewModel
            {
               
                ID = x.Order_ID,
                 Product_ID = x.Product.Product_ID,
                Price = x.Product.Product_Price - ((x.Product.Product_Price *x.Product.Product_Discount)/100),
                Quantity = x.Quantity,
             Total = x.Quantity* (x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100)),
            Date = x.Order.Order_Date.ToString(),
         Customer_ID = x.Order.Customer.Customer_ID,
         QuantityTotal = sumquantity,
         SumTotal = sumtotal
           }).ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Report orders by day";
             
            ws.Cells["A2"].Value = "Date export";
            ws.Cells["B2"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            ws.Cells["A4"].Value = "Sum Quantity";
            ws.Cells["A5"].Value = "Sum Money";
            ws.Cells["B4"].Value = sumquantity;
            ws.Cells["B5"].Value = sumtotal;

            ws.Cells["A7"].Value = "ID";
            ws.Cells["B7"].Value = "Product ID";
            ws.Cells["C7"].Value = "Price";
            ws.Cells["D7"].Value = "Quantity";
            ws.Cells["E7"].Value = "Total";
            ws.Cells["F7"].Value = "Date";
            ws.Cells["G7"].Value = "Customer ID";
            
            int rowStart = 8;
            foreach (var item in emplist)
            {
              

                ws.Cells[string.Format("A{0}", rowStart)].Value = item.ID;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Product_ID;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.Price;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.Quantity;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Total;
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.Date;
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.Customer_ID;
                
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", string.Format("attachment; filename=Jordan_Shop_Report_By_Day{0}.xls", DateTime.Now));
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }

        public void ExportReportDate()
        {
            var order = db.OrderDetails.Include(x => x.Product).Include(x => x.Order).ToList();
            var session = Session["FromDate"].ToString();
           
            int sumquantity1 = order.Where(x => x.Order.Order_Date.Date.ToString() == session).Sum(x => x.Quantity);
            int sumtotal1 = order.Where(x => x.Order.Order_Date.Date.ToString() == session).Sum(x => x.Quantity * (x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100)));
            List<OrderViewModel> emplist = (order.Where(x => x.Order.Order_Date.Date.ToString() == session)).ToList().Select(x => new OrderViewModel
            {

                ID = x.Order_ID,
                Product_ID = x.Product.Product_ID,
                Price = x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100),
                Quantity = x.Quantity,
                Total = x.Quantity * (x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100)),
                Date = x.Order.Order_Date.ToString(),
                Customer_ID = x.Order.Customer.Customer_ID,
                QuantityTotal = sumquantity1,
                SumTotal = sumtotal1
            }).ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Report orders by from day";

            ws.Cells["A2"].Value = "Date export";
            ws.Cells["B2"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            ws.Cells["A4"].Value = "Sum Quantity";
            ws.Cells["A5"].Value = "Sum Money";
            ws.Cells["B4"].Value = sumquantity1;
            ws.Cells["B5"].Value = sumtotal1;

            ws.Cells["A7"].Value = "ID";
            ws.Cells["B7"].Value = "Product ID";
            ws.Cells["C7"].Value = "Price";
            ws.Cells["D7"].Value = "Quantity";
            ws.Cells["E7"].Value = "Total";
            ws.Cells["F7"].Value = "Date";
            ws.Cells["G7"].Value = "Customer ID";

            int rowStart = 8;
            foreach (var item in emplist)
            {


                ws.Cells[string.Format("A{0}", rowStart)].Value = item.ID;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Product_ID;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.Price;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.Quantity;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Total;
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.Date;
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.Customer_ID;

                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", string.Format("attachment; filename=Jordan_Shop_Report_Date{0}.xls", DateTime.Now));
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }

        public void ExportReportDateRange()
        {
            var order = db.OrderDetails.Include(x => x.Product).Include(x => x.Order).ToList();
            var session1 = Session["DateFrom"];
            var session2 = Session["DateTo"];

            int sumquantity1 = order.Where(x => x.Order.Order_Date.Date <= (DateTime)session2 && x.Order.Order_Date.Date >= (DateTime)session1).Sum(x => x.Quantity);
            int sumtotal1 = order.Where(x => x.Order.Order_Date.Date <= (DateTime)session2 && x.Order.Order_Date.Date >= (DateTime)session1).Sum(x => x.Quantity * (x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100)));
            List<OrderViewModel> emplist = (order.Where(x => x.Order.Order_Date.Date <= (DateTime)session2 && x.Order.Order_Date.Date >= (DateTime)session1)).ToList().Select(x => new OrderViewModel
            {

                ID = x.Order_ID,
                Product_ID = x.Product.Product_ID,
                Price = x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100),
                Quantity = x.Quantity,
                Total = x.Quantity * (x.Product.Product_Price - ((x.Product.Product_Price * x.Product.Product_Discount) / 100)),
                Date = x.Order.Order_Date.ToString(),
                Customer_ID = x.Order.Customer.Customer_ID,
                QuantityTotal = sumquantity1,
                SumTotal = sumtotal1
            }).ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Report orders by from "+ Session["DateFrom"]+ " to " + Session["DateTo"];

            ws.Cells["A2"].Value = "Date export";
            ws.Cells["B2"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            ws.Cells["A4"].Value = "Sum Quantity";
            ws.Cells["A5"].Value = "Sum Money";
            ws.Cells["B4"].Value = sumquantity1;
            ws.Cells["B5"].Value = sumtotal1;

            ws.Cells["A7"].Value = "ID";
            ws.Cells["B7"].Value = "Product ID";
            ws.Cells["C7"].Value = "Price";
            ws.Cells["D7"].Value = "Quantity";
            ws.Cells["E7"].Value = "Total";
            ws.Cells["F7"].Value = "Date";
            ws.Cells["G7"].Value = "Customer ID";

            int rowStart = 8;
            foreach (var item in emplist)
            {


                ws.Cells[string.Format("A{0}", rowStart)].Value = item.ID;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Product_ID;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.Price;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.Quantity;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Total;
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.Date;
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.Customer_ID;

                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", string.Format("attachment; filename=Jordan_Shop_Report_From&To_Day{0}.xls", DateTime.Now));
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }

    }
}