using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms;
using POSApplication.Helpers;
using POSApplication.Models;
using ReportViewerForMvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace POSApplication.Controllers
{
    [Authorize]
    public class StockReportController : Controller
    {

        private POSDBContext db = new POSDBContext();
        // GET: StockrReport
        public ActionResult GetStock()
        {
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories.Distinct().OrderBy(x => x.CategoryName).ToList(), "Id", "CategoryName");
            ViewBag.SupplierId = new SelectList(db.Suppliers.ToList().Distinct().OrderBy(x => x.SupplierName), "Id", "SupplierName");
            return View();
        }
        [HttpPost]
        public ActionResult GetStock(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo,int? ProductId, int? SupplierId)
        {
            var Stockdata= POSBaseController.GetStockByProduct(ProductCategoryId, DateFrom, DateTo, ProductId,SupplierId);

            List<sp_getAllStock_Result> stock = new List<sp_getAllStock_Result>();

            foreach (var item in Stockdata)
            {
                stock.Add(new sp_getAllStock_Result
                {
                    ProductName = item.ProductName,
                    CategoryName = item.CategoryName,
                    OpeningStock = item.OpeningStock??0,
                    closingStock = item.closingStock??0,
                    CurrentStock = item.CurrentStock,
                    StockAmount=item.StockAmount
                });
            }

            if (DateFrom == null)
            {
                DateFrom = db.PurchaseInvoiceDets.Select(x => x.PurchaseInvoiceMa.Date).Min();
            }
            if (DateTo == null)
            {
                DateTo = db.PurchaseInvoiceDets.Select(x => x.PurchaseInvoiceMa.Date).Max();
            }
            //-----Add Company Information To Report--------//

            var companyInfo = db.CompanyInformations.FirstOrDefault();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.PageCountMode = new PageCountMode();
            reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/StockReport.rdlc");

            List<ReportParameter> paraList = new List<ReportParameter>();
            var supplierName= db.Suppliers.Where(x => x.Id == SupplierId).Select(x=>x.SupplierName).FirstOrDefault()??"ALL";

            paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
            paraList.Add(new ReportParameter("Email", companyInfo.Email));
            paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
            paraList.Add(new ReportParameter("Address", companyInfo.Address));
            paraList.Add(new ReportParameter("DateFrom", DateFrom.Value.ToShortDateString()));
            paraList.Add(new ReportParameter("DateTo", DateTo.Value.ToShortDateString()));
            paraList.Add(new ReportParameter("SupplierName", supplierName));

            reportViewer.LocalReport.SetParameters(paraList);

            ReportDataSource A = new ReportDataSource("DataSet1", stock);
            reportViewer.LocalReport.DataSources.Add(A);
            reportViewer.ShowRefreshButton = false;
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/StockReport/GetStockReport.cshtml");
        }


       // [HttpPost]
        //public ActionResult GetStock(int? ProductCategoryId,DateTime? DateFrom, DateTime? DateTo)
        //{
        //    try {
        //        DateTime? SalesDatefrom=null;
        //        DateTime ?SalesDateTo=null;
        //        DateTime ?PurchaseDatefrom =null;
        //        DateTime ?PurchaseDateTo =null;


        //        if (DateFrom == null)
        //        {
        //            PurchaseDatefrom = db.PurchaseInvoiceMas.Select(x => x.Date).Min();

        //            SalesDatefrom= db.SalesInvoiceMas.Select(x => x.Date).Min();
        //        }
        //        if (DateTo == null)
        //        {

        //            PurchaseDateTo = db.PurchaseInvoiceMas.Select(x => x.Date).Max();

        //            SalesDateTo = db.SalesInvoiceMas.Select(x => x.Date).Max();
        //        }


        //        var data = db.Products.Where(x => x.ProductCategoryId == ProductCategoryId || ProductCategoryId == null).ToList();

        //        List<Stock> stock = new List<Stock>();

        //        foreach (var item in data)
        //        {
        //            var quantity = db.PurchaseInvoiceDets.Where(x => x.ProductCategoryId == item.ProductCategoryId &&
        //            x.ProductId == item.Id&&
        //            x.PurchaseInvoiceMa.Date>= PurchaseDatefrom || x.PurchaseInvoiceMa.Date >= DateFrom &&
        //            x.PurchaseInvoiceMa.Date<= PurchaseDateTo|| x.PurchaseInvoiceMa.Date<= DateTo

        //            ).Select(x => x.Quantity).Sum();
              
        //            var totalQuantity = item.PresentQuantity + Convert.ToInt32(quantity??0);


        //            var salesQuantity = db.SalesInvoiceDets.Where(x => x.ProductCategoryId == item.ProductCategoryId && x.ProductId == item.Id
        //                    && x.SalesInvoiceMa.Date >= SalesDatefrom || x.SalesInvoiceMa.Date >= DateFrom &&
        //                    x.SalesInvoiceMa.Date <= SalesDateTo || x.SalesInvoiceMa.Date <= DateTo)

        //                    .Select(x => x.Quantity).Sum();

        //            var PresentQuantity = totalQuantity -Convert.ToInt32( salesQuantity??0);

        //            stock.Add(new Stock { ProductName = item.ProductName,
        //                  ProductCategoryName = item.ProductCategory.CategoryName,
        //                  PurchasePrice = item.UpdatedPrice == null ? item.PurchasePrice :item.UpdatedPrice??0.00m,
        //                  TotalQuantity= (int)PresentQuantity,
        //                   //Value= Convert.ToInt32(PresentQuantity) * Convert.ToDecimal(item.UpdatedPrice)
        //            });

        //        }
        //        //----- Add Company Information To Report--------//
        //        var companyInfo = db.CompanyInformations.FirstOrDefault();



        //        ReportViewer reportViewer = new ReportViewer();
        //        reportViewer.ProcessingMode = ProcessingMode.Local;
        //        reportViewer.SizeToReportContent = true;
        //        reportViewer.Width = Unit.Percentage(100);
        //        reportViewer.Height = Unit.Percentage(100);
        //        reportViewer.PageCountMode = new PageCountMode();
        //        reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/StockReport.rdlc");

        //        List<ReportParameter> paraList = new List<ReportParameter>();
        //        paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));

        //        paraList.Add(new ReportParameter("Email",companyInfo.Email));
        //        paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
        //        paraList.Add(new ReportParameter("Address",companyInfo.Address));
        //        paraList.Add(new ReportParameter("DateFrom", DateFrom.ToString()));
        //        paraList.Add(new ReportParameter("DateTo", DateTo.ToString()));

        //        reportViewer.LocalReport.SetParameters(paraList);

        //        ReportDataSource A = new ReportDataSource("DataSet1", stock);
        //        reportViewer.LocalReport.DataSources.Add(A);
        //        reportViewer.ShowRefreshButton = false;


        //        ViewBag.ReportViewer = reportViewer;
        //        //return RedirectToAction("GetStockReport","StockReport");


        //        return View("~/Views/StockReport/GetStockReport.cshtml");



        //    }



        //    catch (Exception e)
        //    {
        //        var message = e.Message;
        //    }

        //    ViewBag.ProductCategoryId = new SelectList(db.ProductCategories.Distinct().OrderBy(x => x.CategoryName).ToList(), "Id", "CategoryName");
        //    return View();


        //}

    }
}
