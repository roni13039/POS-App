using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
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

        private string CreateBarCode(string barcode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //The Image is drawn based on length of Barcode text.
                using (Bitmap bitMap = new Bitmap(barcode.Length * 40, 80))
                {
                    //The Graphics library object is generated for the Image.
                    using (Graphics graphics = Graphics.FromImage(bitMap))
                    {
                        //The installed Barcode font.
                        Font oFont = new Font("IDAutomationHC39M Free Version", 16);
                        PointF point = new PointF(2f, 2f);

                        //White Brush is used to fill the Image with white color.
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);

                        //Black Brush is used to draw the Barcode over the Image.
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        graphics.DrawString("*" + barcode + "*", oFont, blackBrush, point);
                    }

                    //The Bitmap is saved to Memory Stream.
                    bitMap.Save(ms, ImageFormat.Png);

                    //The Image is finally converted to Base64 string.
                    var barCodeImage= "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    //  ViewBag.BarcodeImage =barCodeImage;
                    return barCodeImage;
                }

            }
        }
        [HttpPost]
        public ActionResult GenerateBarcode(string barcode)
         {
            var barcodeImage = CreateBarCode(barcode);

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.PageCountMode = new PageCountMode();
            reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/BarCodeReport.rdlc");
            reportViewer.LocalReport.EnableExternalImages = true;

            /* begin added part */     
            string path = new Uri(Server.MapPath("~/Content/images/VillageMarket.png")).AbsoluteUri; // adjust path to Project folder here

            // set above path to report parameter
            var parameter = new ReportParameter[1];
            parameter[0] = new ReportParameter("image", path); // adjust parameter name here
            reportViewer.LocalReport.SetParameters(parameter);

            var list = new[]
                {
                    new { immage=path},    
                }.ToList();

            ReportDataSource A = new ReportDataSource("DataSet1", list);
            reportViewer.LocalReport.DataSources.Add(A);


            reportViewer.LocalReport.Refresh();
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/PurchaseReport/PurchaseReport.cshtml");
          
        }
    }
}