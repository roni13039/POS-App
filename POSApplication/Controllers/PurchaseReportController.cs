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
    public class PurchaseReportController : Controller
    {
        private static POSDBContext db = new POSDBContext();
        // GET: PurchaseReport
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult CreatePurchaseReport()
        {
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories.Distinct().OrderBy(x => x.CategoryName).ToList(), "Id", "CategoryName");
            ViewBag.SupplierId = new SelectList(db.Suppliers.Distinct().OrderBy(x => x.SupplierName).ToList(), "Id", "SupplierName");

            return View();
        }
        [HttpPost]
        public ActionResult CreatePurchaseReport(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo, int? supplierId, int? productId,string serialNumber )
        {
            try
            {


                if (DateFrom == null)
                {
                    DateFrom = db.PurchaseInvoiceDets.Select(x => x.PurchaseInvoiceMa.Date).Min();
                }
                if (DateTo == null)
                {
                    DateTo = db.PurchaseInvoiceDets.Select(x => x.PurchaseInvoiceMa.Date).Max();
                }
               
                var data = db.PurchaseInvoiceDets.Where(x => (x.PurchaseInvoiceMa.Date >= DateFrom && x.PurchaseInvoiceMa.Date <= DateTo)
                
                            && ((x.ProductCategoryId == ProductCategoryId)|| (ProductCategoryId == null))
                            && ((x.ProductId==productId) ||(productId==null))
                            && ((x.PurchaseInvoiceMa.SupplierId==supplierId) || (supplierId==null))
                            && ((x.SerialNo.Contains(serialNumber)) || (serialNumber == null))
                            ).OrderBy(x=>x.PurchaseInvoiceMa.Date).ToList();

                List<VMPurchaseInvoice> purchaseInvoice = new List<VMPurchaseInvoice>();
                foreach (var item in data)
                {
                    purchaseInvoice.Add(new VMPurchaseInvoice
                    {
                        ProductCategoryName = item.ProductCategory.CategoryName,
                        ProductName = item.Product.ProductName,
                        Quantity = item.Quantity ?? 0,
                        Date = item.PurchaseInvoiceMa.Date,
                        PurchasePrize=item.PurchasePrize,
                        SupplierName=item.PurchaseInvoiceMa.Supplier.SupplierName,
                        Value=item.Amount

                    });
                }

                //----- Add Company Information To Report--------//
      

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);
                reportViewer.PageCountMode = new PageCountMode();
                reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/PurchaseInvoice.rdlc");
                List<ReportParameter> paraList = new List<ReportParameter>();

                var companyInfo = db.CompanyInformations.FirstOrDefault();

                paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
                paraList.Add(new ReportParameter("Email", companyInfo.Email));
                paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
                paraList.Add(new ReportParameter("Address", companyInfo.Address));
                paraList.Add(new ReportParameter("DateFrom", DateFrom.Value.ToShortDateString()));
                paraList.Add(new ReportParameter("DateTo", DateTo.Value.ToShortDateString()));

                reportViewer.LocalReport.SetParameters(paraList);

                ReportDataSource A = new ReportDataSource("DataSet1", purchaseInvoice);
                reportViewer.LocalReport.DataSources.Add(A);
                reportViewer.ShowRefreshButton = false;

                ViewBag.ReportViewer = reportViewer;

                return View("~/Views/PurchaseReport/PurchaseReport.cshtml");

            }
            catch (Exception ex)
            {


            }

                return View();
        }
        [HttpPost]
        public ActionResult SummaryPurchaseReport(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo, int? supplierId, int? productId)
        {
            try
            {
                if (DateFrom == null)
                {
                    DateFrom = db.PurchaseInvoiceDets.Select(x => x.PurchaseInvoiceMa.Date).Min();
                }
                if (DateTo == null)
                {
                    DateTo = db.PurchaseInvoiceDets.Select(x => x.PurchaseInvoiceMa.Date).Max();
                }

                var data = db.PurchaseInvoiceDets
                            .Where(x => (x.PurchaseInvoiceMa.Date >= DateFrom && x.PurchaseInvoiceMa.Date <= DateTo)
                            && ((x.ProductCategoryId == ProductCategoryId) || (ProductCategoryId == null))
                            && ((x.ProductId == productId )|| (productId == null))
                            && ((x.PurchaseInvoiceMa.SupplierId == supplierId) || (supplierId == null))

                            ).OrderBy(x => x.PurchaseInvoiceMa.Date).ToList();

                List<VMPurchaseInvoice> purchaseInvoice = new List<VMPurchaseInvoice>();
                foreach (var item in data)
                {

                    purchaseInvoice.Add(new VMPurchaseInvoice
                    {
                        ProductCategoryName = item.ProductCategory.CategoryName,
                        ProductName = item.Product.ProductName,
                        Quantity = item.Quantity ?? 0,
                        Date = item.PurchaseInvoiceMa.Date,
                        PurchasePrize = item.PurchasePrize,
                        SupplierName = item.PurchaseInvoiceMa.Supplier.SupplierName,
                        Value=item.Amount

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
                reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/PurchaseSummary.rdlc");
                List<ReportParameter> paraList = new List<ReportParameter>();

                paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
                paraList.Add(new ReportParameter("Email", companyInfo.Email));
                paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
                paraList.Add(new ReportParameter("Address", companyInfo.Address));
                paraList.Add(new ReportParameter("DateFrom", DateFrom.Value.ToShortDateString()));
                paraList.Add(new ReportParameter("DateTo", DateTo.Value.ToShortDateString()));

                reportViewer.LocalReport.SetParameters(paraList);

                ReportDataSource A = new ReportDataSource("DataSet1", purchaseInvoice);
                reportViewer.LocalReport.DataSources.Add(A);

                reportViewer.ShowRefreshButton = false;

                ViewBag.ReportViewer = reportViewer;

                return View("~/Views/PurchaseReport/SummaryPurchaseReport.cshtml");

            }
            catch (Exception ex)
            {


            }

            return View();
        }

        public ActionResult CreateExpireProductReport()
        {
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories.Distinct().OrderBy(x => x.CategoryName).ToList(), "Id", "CategoryName");
            ViewBag.SupplierId = new SelectList(db.Suppliers.Distinct().OrderBy(x => x.SupplierName).ToList(), "Id", "SupplierName");

            return View();
        }

        public static List<VMPurchaseInvoice> getExpireProduct(int? ProductCategoryId, int? productId, int? Days)
        {

            var expireDays = DateTime.Now.AddDays(Days ?? 0);
            var data = db.PurchaseInvoiceDets.Where(x => (x.ExpireDate >=DateTime.Now && x.ExpireDate<=expireDays)
                        && ((x.ProductCategoryId == ProductCategoryId) || (ProductCategoryId == null))
                        && ((x.ProductId == productId) || (productId == null))
                        ).OrderBy(x => x.ExpireDate).ToList();

            List<VMPurchaseInvoice> purchaseInvoice = new List<VMPurchaseInvoice>();
            foreach (var item in data)
            {
                purchaseInvoice.Add(new VMPurchaseInvoice
                {
                    ProductCategoryName = item.ProductCategory.CategoryName,
                    ProductName = item.Product.ProductName,
                    Quantity = item.Quantity ?? 0,
                    ExpireDate = item.ExpireDate,
                    PurchasePrize = item.PurchasePrize,
                    SupplierName = item.PurchaseInvoiceMa.Supplier.SupplierName,
                    Value = item.Amount

                });
            }

            return purchaseInvoice;
        }

        [HttpPost]
        public ActionResult CreateExpireProductReport(int? ProductCategoryId, int? productId, int? Days)
        {
            try
            {
                List<VMPurchaseInvoice> purchaseInvoice = getExpireProduct(ProductCategoryId, productId,Days);

                //var expireDays = DateTime.Now.AddDays(Days??0);
                //var data = db.PurchaseInvoiceDets.Where(x =>(x.ExpireDate<= expireDays)
                //            && ((x.ProductCategoryId == ProductCategoryId) || (ProductCategoryId == null))
                //            && ((x.ProductId == productId) || (productId == null))
                //            ).OrderBy(x => x.ExpireDate).ToList();

                //List<VMPurchaseInvoice> purchaseInvoice = new List<VMPurchaseInvoice>();
                //foreach (var item in data)
                //{
                //    purchaseInvoice.Add(new VMPurchaseInvoice
                //    {
                //        ProductCategoryName = item.ProductCategory.CategoryName,
                //        ProductName = item.Product.ProductName,
                //        Quantity = item.Quantity ?? 0,
                //        ExpireDate = item.ExpireDate,
                //        PurchasePrize = item.PurchasePrize,
                //        SupplierName = item.PurchaseInvoiceMa.Supplier.SupplierName,
                //        Value = item.Amount

                //    });
                //}

                //----- Add Company Information To Report--------//


                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);
                reportViewer.PageCountMode = new PageCountMode();
                reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/ExpireProduct.rdlc");
                List<ReportParameter> paraList = new List<ReportParameter>();

                var companyInfo = db.CompanyInformations.FirstOrDefault();

                paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
                paraList.Add(new ReportParameter("Email", companyInfo.Email));
                paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
                paraList.Add(new ReportParameter("Address", companyInfo.Address));
                //paraList.Add(new ReportParameter("DateFrom", DateFrom.Value.ToShortDateString()));
                //paraList.Add(new ReportParameter("DateTo", DateTo.Value.ToShortDateString()));

                reportViewer.LocalReport.SetParameters(paraList);

                ReportDataSource A = new ReportDataSource("DataSet1", purchaseInvoice);
                reportViewer.LocalReport.DataSources.Add(A);
                reportViewer.ShowRefreshButton = false;

                ViewBag.ReportViewer = reportViewer;

                return View("~/Views/PurchaseReport/PurchaseReport.cshtml");

            }
            catch (Exception ex)
            {


            }

            return View();
        }

    }


}
    
