using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using POSApplication.Models;
using POSApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace POSApplication.Controllers
{
    [Authorize]
    public class SalesReportController : Controller
    {
           private static POSDBContext db = new POSDBContext();
       
        // GET: PurchaseReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateSalesReport()
        {
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories.Distinct().OrderBy(x => x.CategoryName).ToList(), "Id", "CategoryName");
            return View();
        }

        public static List<VMSalesInvoice> getSalesInformation(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo, int? ProductId, string phone)
        {

            if (DateFrom == null)
            {
                DateFrom = db.SalesInvoiceDets.Select(x => (DateTime?)x.SalesInvoiceMas.Date).Min();
            }
            if (DateTo == null)
            {
                DateTo = db.SalesInvoiceDets.Select(x => (DateTime?)x.SalesInvoiceMas.Date).Max();
            }
            var phoneList = db.SalesInvoiceMas.Select(x => x.Phone).ToList();


            var data = (from det in db.SalesInvoiceDets
                     join mas in db.SalesInvoiceMas on det.SalesInvoiceMasId equals mas.Id
                     where (mas.Date >= DateFrom && mas.Date <= DateTo)
                     && ((det.ProductCategoryId == ProductCategoryId) || (ProductCategoryId == null))
                     && ((det.ProductId == ProductId) || (ProductId == null))
                     && ((mas.Phone.Contains(phone)) || string.IsNullOrEmpty(phone))
                     select new
                     {
                         ProductCategoryName = det.ProductCategory.CategoryName,
                         ProductName = det.Product.ProductName,
                         PurchasePrice = det.PurchasePrize,
                         SalesPrice = det.SalesPrize,
                         Quantity = det.Quantity ?? 0,
                         Date = mas.Date,
                         CustomerName = mas.CustomerName,
                         //Discount = x.Discount ?? 0,
                         Value = det.Amount ?? 0.00m,
                         Discount = det.Discount,
                         Phone = mas.Phone

                     }).OrderByDescending(x => x.Date).ToList();
                   
            //var data = db.SalesInvoiceDets.Where(x => (x.SalesInvoiceMas.Date >= DateFrom && x.SalesInvoiceMas.Date <= DateTo)
            //                              && ((x.ProductCategoryId == ProductCategoryId) || (ProductCategoryId == null))
            //                              && ((x.ProductId == ProductId) || (ProductId == null))
            //                             // && (( phone==null ||phone=="")||( phoneList.Contains(phone)))
            //                                ).Select(x=>new VMSalesInvoice
            //                                (){

            //                                    ProductCategoryName = x.ProductCategory.CategoryName,
            //                                    ProductName = x.Product.ProductName,
            //                                    PurchasePrice = x.PurchasePrize,
            //                                    SalesPrice = x.SalesPrize,
            //                                    Quantity = x.Quantity ?? 0,
            //                                    Date = x.SalesInvoiceMas.Date,
            //                                    CustomerName = " ",
            //                                    //Discount = x.Discount ?? 0,
            //                                    Value = x.Amount ?? 0.00m,
            //                                    Discount = x.SalesInvoiceMasId,
            //                                    Phone = db.SalesInvoiceMas.Where(y=>y.Id==x.SalesInvoiceMasId).Select(y=>y.Phone).FirstOrDefault()


            //                                }).ToList();

            List<VMSalesInvoice> SalesInvoice = new List<VMSalesInvoice>();

            foreach (var item in data)
            {
                SalesInvoice.Add(new VMSalesInvoice
                {
                    ProductCategoryName = item.ProductCategoryName,
                    ProductName = item.ProductName,
                    PurchasePrice = item.PurchasePrice,
                    SalesPrice = item.SalesPrice,
                    Quantity = item.Quantity,
                    Date = item.Date,
                    CustomerName = item.CustomerName,
                    Discount = item.Discount ?? 0,
                    Value = item.Value,
                    Phone = item.Phone

                });
            }

            return SalesInvoice;
        }


        [HttpPost]
        public ActionResult CreateSalesReport(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo,int? ProductId ,string phone)
        {
            try
            {

                List<VMSalesInvoice> SalesInvoice= getSalesInformation(ProductCategoryId, DateFrom, DateTo, ProductId, phone);

                //if (DateFrom == null)
                //{
                //    DateFrom = db.SalesInvoiceDets.Select(x =>(DateTime?) x.SalesInvoiceMa.Date).Min();
                //}
                //if (DateTo == null)
                //{
                //    DateTo = db.SalesInvoiceDets.Select(x =>(DateTime?)x.SalesInvoiceMa.Date).Max();
                //}

                //var data = db.SalesInvoiceDets  .Where(x => (x.SalesInvoiceMa.Date >= DateFrom && x.SalesInvoiceMa.Date <= DateTo) 
                //                                && ((x.ProductCategoryId == ProductCategoryId) || (ProductCategoryId == null))
                //                                &&( (x.ProductId == ProductId) || (ProductId == null))
                //                                && ((x.SalesInvoiceMa.Phone.Contains(phone)) || (phone == null))
                //                                ).OrderBy(x=>x.SalesInvoiceMa.Date).ToList();

                //List<VMSalesInvoice> SalesInvoice = new List<VMSalesInvoice>();

                //foreach (var item in data)
                //{
                //    SalesInvoice.Add(new VMSalesInvoice
                //    {
                //        ProductCategoryName = item.ProductCategory.CategoryName,
                //        ProductName = item.Product.ProductName,
                //        PurchasePrice = item.PurchasePrize,
                //        SalesPrice=item.SalesPrize,
                //        Quantity = item.Quantity??0,
                //        Date = item.SalesInvoiceMa.Date,
                //        CustomerName =item.SalesInvoiceMa.CustomerId==null?"": item.SalesInvoiceMa.Customer.CustomerName,
                //        Discount=item.Discount??0,
                //        Value=item.Amount??0.00m,
                //        Phone = item.SalesInvoiceMa.Phone

                //    });
                //}

                //----- Add Company Information To Report--------//
                var companyInfo = db.CompanyInformations.FirstOrDefault();

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);
                reportViewer.PageCountMode = new PageCountMode();
                reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/SalesInvoice.rdlc");

                List<ReportParameter> paraList = new List<ReportParameter>();

                paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
                paraList.Add(new ReportParameter("Email", companyInfo.Email));
                paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
                paraList.Add(new ReportParameter("Address", companyInfo.Address));
                paraList.Add(new ReportParameter("DateFrom",DateFrom!=null ? DateFrom.Value.ToShortDateString():null));
                paraList.Add(new ReportParameter("DateTo",DateTo!=null ? DateTo.Value.ToShortDateString():null));

                reportViewer.LocalReport.SetParameters(paraList);

                ReportDataSource A = new ReportDataSource("DataSet1", SalesInvoice);
                reportViewer.LocalReport.DataSources.Add(A);
                reportViewer.ShowRefreshButton = false;


                ViewBag.ReportViewer = reportViewer;


                return View("~/Views/SalesReport/SalesInvoiceReport.cshtml");

            }
            catch (Exception ex)
            {
                var message = ex.Message;

            }

            return View();
        }
        [HttpPost]
        public ActionResult SummarySalessReport(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo,int? ProductId,string phone)
        {
            try
            {
                if (DateFrom == null)
                {
                    DateFrom = db.SalesInvoiceDets.Select(x =>(DateTime?) x.SalesInvoiceMas.Date).Min();
                }
                if (DateTo == null)
                {
                    DateTo = db.SalesInvoiceDets.Select(x =>(DateTime?) x.SalesInvoiceMas.Date).Max();
                }


                var data = db.SalesInvoiceDets.Where(x => (x.SalesInvoiceMas.Date >= DateFrom && x.SalesInvoiceMas.Date <= DateTo)
                                              && ((x.ProductCategoryId == ProductCategoryId) || (ProductCategoryId == null))
                                              && ((x.ProductId == ProductId) || (ProductId == null))
                                              && ((x.SalesInvoiceMas.Phone.Contains(phone)) || (phone == null))
                                                ).OrderBy(x => x.SalesInvoiceMas.Date).ToList();

                List<VMSalesInvoice> SalesInvoice = new List<VMSalesInvoice>();

                foreach (var item in data)
                {
                    SalesInvoice.Add(new VMSalesInvoice
                    {
                        ProductCategoryName = item.ProductCategory.CategoryName,
                        ProductName = item.Product.ProductName,
                        PurchasePrice = item.PurchasePrize,
                        SalesPrice = item.SalesPrize,
                        Quantity = item.Quantity ?? 0,
                        Date = item.SalesInvoiceMas.Date,
                        CustomerName = item.SalesInvoiceMas.CustomerId == null ? "" : item.SalesInvoiceMas.Customer.CustomerName,
                        Discount = item.Discount ?? 0,
                        Value = item.Amount ?? 0.00m,
                        Phone = item.SalesInvoiceMas.Phone

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
                reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/SummarySalesInvoice.rdlc");

                List<ReportParameter> paraList = new List<ReportParameter>();

                paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
                paraList.Add(new ReportParameter("Email", companyInfo.Email));
                paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
                paraList.Add(new ReportParameter("Address", companyInfo.Address));
                paraList.Add(new ReportParameter("DateFrom", DateFrom != null ? DateFrom.Value.ToShortDateString() : null));
                paraList.Add(new ReportParameter("DateTo", DateTo != null ? DateTo.Value.ToShortDateString() : null));


                reportViewer.LocalReport.SetParameters(paraList);

                ReportDataSource A = new ReportDataSource("DataSet1", SalesInvoice);
                reportViewer.LocalReport.DataSources.Add(A);
                reportViewer.ShowRefreshButton = false;


                ViewBag.ReportViewer = reportViewer;


                return View("~/Views/SalesReport/SalesInvoiceReport.cshtml");

            }
            catch (Exception ex)
            {
                var message = ex.Message;

            }

            return View();
        }
        //previos sales invoice report directly pdf 
        //1/7/2022

        public ActionResult ExportToPDF(int? id)
        {
            // data From sales Master and Details

            var data = db.SalesInvoiceDets.Where(x => x.SalesInvoiceMasId == id).Select(x => new VMSalesInvoice()
            {
                Date = x.SalesInvoiceMas.Date,
                ProductName = x.Product.ProductName,
                SalesPrice = x.SalesPrize,
                Quantity = x.Quantity ?? 0,
                Value = x.Amount ?? 0.00m
            }).OrderBy(x => x.ProductName).ToList();

            var salesMasterData = db.SalesInvoiceMas.Where(x => x.Id == id).FirstOrDefault();
            //----- Add Company Information To Report--------//
            var companyInfo = db.CompanyInformations.FirstOrDefault();

            //Report  
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/SalesForCustomerReport.rdlc");


            List<ReportParameter> paraList = new List<ReportParameter>();

            paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
            paraList.Add(new ReportParameter("Email", companyInfo.Email));
            paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
            paraList.Add(new ReportParameter("Address", companyInfo.Address));
            paraList.Add(new ReportParameter("CustomerPhone", salesMasterData.Phone));
            paraList.Add(new ReportParameter("CustomerName", salesMasterData.CustomerName));
            paraList.Add(new ReportParameter("CusAddress", salesMasterData.Address));
            reportViewer.LocalReport.SetParameters(paraList);

            ReportDataSource A = new ReportDataSource("DataSet1", data);
            reportViewer.LocalReport.DataSources.Add(A);



            //Byte  
            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, filenameExtension;

            byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            // Delete All Files From a directory and then save it.

            System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath(@"~\TempFiles"));
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();

            }

            //File  path create
            string FileName = "Test_" + DateTime.Now.Ticks.ToString() + ".pdf";
            string FilePath = HttpContext.Server.MapPath(@"~\TempFiles\") + FileName;

            //then RDlc Report set the file

            //create and set PdfReader  
            PdfReader reader = new PdfReader(bytes);
            FileStream output = new FileStream(FilePath, FileMode.Create);

            string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

            //create and set PdfStamper  
            PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

            if (Agent.Contains("Firefox"))
                pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
            else
                pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

            pdfStamper.FormFlattening = false;
            pdfStamper.Close();
            reader.Close();

            //return file path  
            //   string FilePathReturn = @"TempFiles/" + FileName;

            return File(FilePath, "application/pdf");

            //return Content(FilePathReturn);
        }

        //public ActionResult ExportToPDF(int? id)
        //{
        //    // data From sales Master and Details
        //    var data = db.SalesInvoiceDets.Where(x => x.SalesInvoiceMasId == id).Select(x => new VMSalesInvoice()
        //    {
        //        Date = x.SalesInvoiceMas.Date,
        //        ProductName = x.Product.ProductName,
        //        SalesPrice = x.SalesPrize,
        //        Quantity = x.Quantity ?? 0,
        //        Value = x.Amount ?? 0.00m,
        //        Phone=x.SalesInvoiceMas.Phone
        //    }).OrderBy(x => x.ProductName).ToList();

        //    //----- Add Company Information To Report--------//
        //    var companyInfo = db.CompanyInformations.FirstOrDefault();

        //    ReportViewer reportViewer = new ReportViewer();
        //    reportViewer.ProcessingMode = ProcessingMode.Local;
        //    reportViewer.SizeToReportContent = true;
        //    reportViewer.Width = Unit.Percentage(100);
        //    reportViewer.Height = Unit.Percentage(100);
        //    reportViewer.PageCountMode = new PageCountMode();
        //    reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/SalesForCustomerReport.rdlc");


        //    List<ReportParameter> paraList = new List<ReportParameter>();

        //    paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));
        //    paraList.Add(new ReportParameter("Email", companyInfo.Email));
        //    paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
        //    paraList.Add(new ReportParameter("Address", companyInfo.Address));
        //    paraList.Add(new ReportParameter("CustomerPhone", db.SalesInvoiceMas.Select(x=>x.Phone).FirstOrDefault()));
        //    reportViewer.LocalReport.SetParameters(paraList);

        //    ReportDataSource A = new ReportDataSource("DataSet1", data);
        //    reportViewer.LocalReport.DataSources.Add(A);

        //    reportViewer.LocalReport.DataSources.Add(A);
        //    reportViewer.ShowRefreshButton = false;

        //    ViewBag.ReportViewer = reportViewer;

        //    return View("~/Views/PurchaseReport/PurchaseReport.cshtml");

        //    //return Content(FilePathReturn);
        //}
    }
}
