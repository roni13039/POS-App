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
    public class ProfitReportController : Controller
    {
        private static POSDBContext db = new POSDBContext();
       
        public ActionResult Index()
        {
            return View();
        }
        // GET: ProfitReport
        public ActionResult CreateProfitReport()
        {
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories.Distinct().OrderBy(x => x.CategoryName).ToList(), "Id", "CategoryName");
            return View();
        }

        public static List<VmPurchaseInvoice> getProfit(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo, int? ProductId)
        {
            
            var data = db.SalesInvoiceDets.Where(x =>((x.SalesInvoiceMas.Date >= DateFrom) || (DateFrom == null))
                            && ((x.SalesInvoiceMas.Date <= DateTo) || (DateTo == null))
                            && ((x.ProductCategoryId == ProductCategoryId) || (ProductCategoryId == null))
                            && ((x.ProductId == ProductId) || (ProductId == null))).OrderByDescending(x=>x.SalesInvoiceMas.Date).ToList();

            List<VmPurchaseInvoice> profit = new List<VmPurchaseInvoice>();
            foreach (var item in data)
            {
                profit.Add(new VmPurchaseInvoice
                {
                    ProductCategory = item.ProductCategory.CategoryName,
                    Product = item.Product.ProductName,
                    PurchasePrize = item.PurchasePrize,
                    SalesPrize = item.SalesPrize,// here amount is sales prize after  calculation with qunatity and discount
                    Quantity = item.Quantity ?? 0,
                    Date = item.SalesInvoiceMas.Date

                });
            }

            return profit;

        }

        [HttpPost]
        public ActionResult CreateProfitReport(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo,int? ProductId)
        {
            try
            {

                List<VmPurchaseInvoice> profit = getProfit(ProductCategoryId, DateFrom, DateTo, ProductId);

                var da=profit.Select(x => (x.SalesPrize * x.Quantity) - (x.PurchasePrize * x.Quantity)).Sum();

                //if (DateFrom == null)
                //{
                //    DateFrom = db.SalesInvoiceDets.Select(x =>(DateTime?) x.SalesInvoiceMa.Date).Min();
                //}
                //if (DateTo == null)
                //{
                //    DateTo = db.SalesInvoiceDets.Select(x => (DateTime?)x.SalesInvoiceMa.Date).Max();
                //}

                //var data =profit.Where(x => ((x.Date >= DateFrom )||(DateFrom==null))
                //&& ((x.Date <= DateTo) || (DateTo == null))
                //&& (x.pr == ProductCategoryId || ProductCategoryId == null
                //&& x.ProductId == ProductId || ProductId == null

                //).ToList();


                //----- Add Company Information To Report--------//
                var companyInfo = db.CompanyInformations.FirstOrDefault();

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);
                reportViewer.PageCountMode = new PageCountMode();
                reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/ProfitReport.rdlc");

                List<ReportParameter> paraList = new List<ReportParameter>();

                paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
                paraList.Add(new ReportParameter("Email", companyInfo.Email));
                paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
                paraList.Add(new ReportParameter("Address", companyInfo.Address));
                paraList.Add(new ReportParameter("DateFrom",DateFrom!=null? DateFrom.Value.ToShortDateString():null));
                paraList.Add(new ReportParameter("DateTo", DateTo!=null ?   DateTo.Value.ToShortDateString():null));

                reportViewer.LocalReport.SetParameters(paraList);

                ReportDataSource A = new ReportDataSource("DataSet1", profit);
                reportViewer.LocalReport.DataSources.Add(A);
                reportViewer.ShowRefreshButton = false;

                ViewBag.ReportViewer = reportViewer;

                return View("~/Views/StockReport/GetStockReport.cshtml");


            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult SummaryProfitReport(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo,int? ProductId)
        {
            try
            {

                if (DateFrom == null)
                {
                    DateFrom = db.SalesInvoiceDets.Select(x => (DateTime?)x.SalesInvoiceMas.Date).Min();
                }
                if (DateTo == null)
                {
                    DateTo = db.SalesInvoiceDets.Select(x => (DateTime?)x.SalesInvoiceMas.Date).Max();
                }


                var data = db.SalesInvoiceDets.Where(x => (x.SalesInvoiceMas.Date >= DateFrom||DateFrom==null )&& (x.SalesInvoiceMas.Date <= DateTo || DateTo==null)
                && (x.ProductCategoryId == ProductCategoryId || ProductCategoryId == null)
                &&( x.ProductId == ProductId || ProductId == null)).ToList();

                List<VmPurchaseInvoice> profit = new List<VmPurchaseInvoice>();

                foreach (var item in data)
                {
                    profit.Add(new VmPurchaseInvoice
                    {
                        ProductCategory = item.ProductCategory.CategoryName,
                        Product = item.Product.ProductName,
                        PurchasePrize = item.PurchasePrize,
                        SalesPrize = item.SalesPrize,// here amount is sales prize after  calculation with qunatity and discount
                        Quantity = item.Quantity ??0,
                        Date = item.SalesInvoiceMas.Date
                    });
                }

                var xprofit = profit.Count()>0 ? profit.Select(x => x.SalesPrize*x.Quantity - x.PurchasePrize*x.Quantity).Sum():0.00m;
                var expense = db.GeneralAccounts.Where(x => (x.Date >= DateFrom || DateFrom == null) && (x.Date <= DateTo || DateTo == null)).Count() > 0 ? db.GeneralAccounts.Where(x => (x.Date >= DateFrom || DateFrom == null) && x.Date <= DateTo || DateTo == null).Select(x => (decimal?)x.CashPayment ?? 0.00m).Sum() : 0;
                 
                List <VMProfitSummaryDS> profitSummary = new List<VMProfitSummaryDS>();
                profitSummary.Add(
                  new VMProfitSummaryDS() { DateFrom = DateFrom != null ? DateFrom.Value.ToShortDateString() : null, DateTo = DateTo != null ? DateTo.Value.ToShortDateString() : null, Profit = xprofit, Expense = expense, TotalProfit = xprofit - expense }
                        );


                //----- Add Company Information To Report--------//
                var companyInfo = db.CompanyInformations.FirstOrDefault();

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);
                reportViewer.PageCountMode = new PageCountMode();
                reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/SummaryProfitReport.rdlc");

                List<ReportParameter> paraList = new List<ReportParameter>();

                paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
                paraList.Add(new ReportParameter("Email", companyInfo.Email));
                paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
                paraList.Add(new ReportParameter("Address", companyInfo.Address));
                paraList.Add(new ReportParameter("DateFrom", DateFrom != null ? DateFrom.Value.ToShortDateString() : null));
                paraList.Add(new ReportParameter("DateTo", DateTo != null ? DateTo.Value.ToShortDateString() : null));

                reportViewer.LocalReport.SetParameters(paraList);

                ReportDataSource A = new ReportDataSource("DataSet1", profit);
                ReportDataSource B = new ReportDataSource("DataSet2", profitSummary);

                reportViewer.LocalReport.DataSources.Add(A);
                reportViewer.LocalReport.DataSources.Add(B);
                reportViewer.ShowRefreshButton = false;
                ViewBag.ReportViewer = reportViewer;

                return View("~/Views/StockReport/ProfitSummaryReport.cshtml");


            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return View();
        }
        
    }
}