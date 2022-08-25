using Microsoft.Reporting.WebForms;
using POSApplication.Models;
using POSApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace POSApplication.Controllers
{
    [Authorize]
    public class CustomerReportController : Controller
    {
        private POSDBContext db = new POSDBContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateCustomerReport()
        {
            ViewBag.CustomerId = new SelectList(db.Customers.OrderBy(x => x.CustomerName).ToList(), "Id", "CustomerName");

            return View();
        }
        [HttpPost]
        public ActionResult CreateCustomerReport(int? Id)
        {
            var data = db.Customers.Where(x => x.Id == Id || Id == null).ToList();



            List<VMCustomer> customer = new List<VMCustomer>();

            foreach (var item in data)
            {

                var TotalPurchase = db.SalesInvoiceMas.Where(x => x.CustomerId == item.Id).Select(x => x.TotalBill).Sum();

                var TotalPaid=db.SalesInvoiceMas.Where(x => x.CustomerId == item.Id).Select(x => x.PaidBill).Sum();

                var TotalDue = db.SalesInvoiceMas.Where(x => x.CustomerId == item.Id).Select(x => x.DueBalance).Sum();


                var Total_purchase = item.TotalPurchase??0.00m + TotalPurchase??0.00m;
                var Total_Paid = item.TotalPaid ?? 0.00m + TotalPaid??0.00m;
                var Total_Due = (decimal)(item.DueBalance??0.00m)/*e == null || item.DueBalance==0.00m ? 0:item.DueBalance */+ TotalDue;

                customer.Add(new VMCustomer
                {
                    CustomerName =item.CustomerName,
                    Phone=item.Phone,
                    TotalPurchase= Total_purchase,
                    TotalPaid= Total_Paid,
                    DueBalance=Total_Due??0

                });


            }

            //----- Add Company Information To Report--------//
            var companyInfo = db.CompanyInformations.FirstOrDefault();



            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.PageCountMode = new PageCountMode();
            reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/CustomerReport.rdlc");

            List<ReportParameter> paraList = new List<ReportParameter>();
            paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));

            paraList.Add(new ReportParameter("Email", companyInfo.Email));
            paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
            paraList.Add(new ReportParameter("Address", companyInfo.Address));

            reportViewer.LocalReport.SetParameters(paraList);

            ReportDataSource A = new ReportDataSource("DataSet1", customer);
            reportViewer.LocalReport.DataSources.Add(A);
            reportViewer.ShowRefreshButton = false;


            ViewBag.ReportViewer = reportViewer;


            return View("~/Views/CustomerReport/CustomerReport.cshtml");


          
        }


    }
}