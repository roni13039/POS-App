using POSApplication.Models;
using POSApplication.NotificationHelper;
using POSApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace POSApplication.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            //Dash Board value

            ViewBag.Stock= POSBaseController.GetStockByProduct(null, null, null, null,null).Select(x=>x.TotalValue).Sum().ToString("0.00");
            ViewBag.Profit = ProfitReportController.getProfit(null, DateTime.Now.AddDays(-31).Date, DateTime.Now.Date, null).Select(x => (x.SalesPrize * x.Quantity) - (x.PurchasePrize * x.Quantity)).Sum();
            ViewBag.ExpireProducts = PurchaseReportController.getExpireProduct(null, null, 30).Select(x => x.ProductName).Count();
            ViewBag.SalesAmount = SalesReportController.getSalesInformation(null, DateTime.Now.AddDays(-30), DateTime.Now, null, null).Select(x => x.Value).Sum();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult EcommerceHome()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public ActionResult SignOut()
        {
            var uid = 0;
            if (Session["uid"] != null)
            {
                uid = Convert.ToInt32(Session["uid"].ToString());
            }
         
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Authentication");
        }


        public JsonResult GetNotifications()
        {
            var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
            NotificationComponent NC = new NotificationComponent();
            var list = NC.GetData(notificationRegisterTime);

            //update session here for get only new added contacts (notification)  
            Session["LastUpdate"] = DateTime.Now;
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Demo()
        {

            try
            {


                var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
                NotificationComponent NC = new NotificationComponent();
                dynamic list = NC.GetData(notificationRegisterTime);

                //update session here for get only new added contacts (notification)  
                Session["LastUpdate"] = DateTime.Now;
                //return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var messa = ex.Message;
                return Json(false, JsonRequestBehavior.AllowGet);

            }
        }
    }
}