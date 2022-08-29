
using Microsoft.Reporting.WebForms;
using POSApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace POSApplication.Controllers
{
    public class BarcodeController : Controller
    {
        
        public ActionResult GenerateBarcode()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GenerateBarcode(int  number)
        {

            List<BarcodeNumber> barcodeNumber = new List<BarcodeNumber>();


            for (int i = 0; i < number; i++)
            {

               barcodeNumber.Add(new BarcodeNumber() { number="21209122"+i });
            }

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.PageCountMode = new PageCountMode();
            reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/BarCodeReport.rdlc");

            List<ReportParameter> paraList = new List<ReportParameter>();
           
            reportViewer.LocalReport.SetParameters(paraList);

            ReportDataSource A = new ReportDataSource("DataSet1", barcodeNumber);
            reportViewer.LocalReport.DataSources.Add(A);
            reportViewer.ShowRefreshButton = false;


            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/StockReport/GetStockReport.cshtml");
        }
    }

}