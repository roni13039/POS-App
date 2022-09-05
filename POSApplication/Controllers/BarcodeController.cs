
using BarcodeGenerator;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using POSApplication.Models;
using POSApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        private POSDBContext db = new POSDBContext();
        public ActionResult GenerateBarcode()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GenerateBarcode(int  number)
        {

            //generate barcode

            //
            var lastNumber = _GetBarcodeLastNumber(number);
            List<BarcodeNumber> barcodeNumber = new List<BarcodeNumber>();

            for (int i = 0; i < number; i++)
            {
                var barcodePath = GenerateBarcodeImage((lastNumber + i).ToString());
                barcodeNumber.Add(new BarcodeNumber() { number = barcodePath });
            }


            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.PageCountMode = new PageCountMode();
            reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/BarCodeReport.rdlc");

            //List<ReportParameter> paraList = new List<ReportParameter>();     
            //reportViewer.LocalReport.SetParameters(paraList);

            reportViewer.LocalReport.EnableExternalImages = true;
            ReportDataSource A = new ReportDataSource("DataSet1", barcodeNumber);
            reportViewer.LocalReport.DataSources.Add(A);
         
            reportViewer.ShowRefreshButton = false;
            ViewBag.ReportViewer = reportViewer;


         


            return View("~/Views/StockReport/GetStockReport.cshtml");
        }

        private dynamic GenerateBarcodeImage(string barcodeValue)
        {


            var barcodeList = new List<string>();
            var barcodeUrl = "";
            string path = HttpContext.Server.MapPath(@"~/Content/BarcodeImages/");
            barcodeList.Add(barcodeValue);

            int width = Convert.ToInt32(120);
            int length = Convert.ToInt32(50);

            TYPE type = (TYPE)Enum.Parse(typeof(TYPE), "EAN13"); //need to remove upper case

            GenerateBarcode _barcodegen = new GenerateBarcode();

            _barcodegen.CreateImage(path, barcodeList, type, width, length, AlignmentPositions.CENTER, true);

             barcodeUrl = new Uri(Server.MapPath("~/Content/BarcodeImages/" + barcodeValue + ".jpg")).AbsoluteUri;
            return barcodeUrl ;
        }

        private long _GetBarcodeLastNumber(long number)
        {
            var lastNumber = db.UDC_Barcode.Select(x => x.BarcodeNumber).FirstOrDefault();
            var barcode= db.UDC_Barcode.FirstOrDefault();
            barcode.BarcodeNumber = barcode.BarcodeNumber + number;

            db.Entry(barcode).State = EntityState.Modified;
            db.SaveChanges();

            return lastNumber;

        }

        private string GenerateRandomUniqueNumber(int length)
        {
            string numbers = "0";
            string characters = numbers;
           
            string id = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (id.IndexOf(character) != -1);
                id += character;
            }
            return id;
  
        }
    }

}