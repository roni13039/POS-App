using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using POSApplication.Models;
using POSApplication.Helpers;
using System.Web.Routing;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using POSApplication.ViewModel;

namespace POSApplication.Controllers
{
    [Authorize]
    public class SalesInvoiceController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: SalesInvoice
        public ActionResult Index()
        {
            return View(db.SalesInvoiceMas.OrderByDescending(x=>x.Date).ToList());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesInvoiceMas salesInvoiceMa = db.SalesInvoiceMas.Find(id);
            if (salesInvoiceMa == null)
            {
                return HttpNotFound();
            }
            return View(salesInvoiceMa);
        }

        // GET: SalesInvoice/Create
        public ActionResult Create()
        {
          
            var serialNo = db.SalesInvoiceMas.Where(x => x.Year == DateTime.Now.Year).Count()==0 ? 1: db.SalesInvoiceMas.Where(x => x.Year == DateTime.Now.Year).Count()+1;
            var siNo = serialNo <= 9? ("0" + serialNo) : serialNo.ToString();

            ViewBag.BillNo = "Si-" + siNo + DateTime.Now.Month+DateTime.Now.Year;

            ViewBag.CustomerId = new SelectList(db.Customers.ToList().Distinct(), "Id", "CustomerName");
            SalesInvoiceMas salesInvoiceMa = new SalesInvoiceMas();
            salesInvoiceMa.Date = DateTime.Now;

            return View(salesInvoiceMa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CustomerType,Date,CustomerId,Phone,Address,Description,UserFullName")] SalesInvoiceMas salesInvoiceMa)
        {
            if (ModelState.IsValid)
            {
                db.SalesInvoiceMas.Add(salesInvoiceMa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(salesInvoiceMa);
        }

        //GET: SalesInvoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesInvoiceMas salesInvoiceMa = db.SalesInvoiceMas.Find(id);
            ViewBag.BillNo = salesInvoiceMa.InvoiceNo;

            ViewBag.CustomerId = new SelectList(db.Customers.ToList().Distinct(), "Id", "CustomerName", salesInvoiceMa.CustomerId);

            if (salesInvoiceMa == null)
            {
                return HttpNotFound();
            }
            return View("Create",salesInvoiceMa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerType,Date,CustomerId,Phone,Address,Description,UserFullName")] SalesInvoiceMas salesInvoiceMa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salesInvoiceMa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(salesInvoiceMa);
        }


        [HttpPost]
        public ActionResult Delete(int Id)
        {
            SalesInvoiceMas salesInvoiceMa = db.SalesInvoiceMas.Find(Id);
            db.SalesInvoiceMas.Remove(salesInvoiceMa);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerRelatedData(int? CustomerId)
        {
            if (CustomerId == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = db.Customers.Where(x => x.Id == CustomerId).Select(x => new
                {
                    Phone = x.Phone,
                    Address = x.Address

                }).FirstOrDefault();

                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult GetCustomerInfoByPhone(string phone)
        {
            var data = db.Customers.Where(x=>x.Phone.Trim().Contains(phone)).Distinct().ToList().Select(x => new
            {

                CustomerId = x.Id,
                CustomerName = x.CustomerName,
                Phone = x.Phone,
                Address = x.Address,

            }).OrderBy(x=>x.CustomerName).FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetProductBySerialNo(string SerialNo)
        {
            using (POSBaseController posBase = new POSBaseController())
            {
                var data = posBase.getProductBySerialInfo(SerialNo);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            
        }
        public JsonResult GetCustomerName()
        {
            var data = db.Customers.Distinct().ToList().Select(x => new
            {

                Value = x.Id,
                Text = x.CustomerName

            }).Distinct().OrderBy(x=>x.Text).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProductCatagoryName()
        {
            var data = db.ProductCategories.Select(x => new
            {
                Value = x.Id,
                Text = x.CategoryName
            }).Distinct().OrderBy(x=>x.Text).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductName()
        {
            var data = db.Products.Select(x => new
            {
                Value = x.Id,
                Text = x.ProductName

            }).Distinct().OrderBy(x=>x.Text).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetProductByCatagoryId(int? ProductCategoryId)
        {
            if (ProductCategoryId == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = db.Products.Where(x => x.ProductCategoryId == ProductCategoryId).Select(x => new
                {
                    Value = x.Id,
                    Text = x.ProductName
                }).Distinct().ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPrizeByProductId(int? ProductId)
        {
            if (ProductId == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = db.PurchaseInvoiceDets.Where(x => x.ProductId == ProductId).Select(x => new
                {
                    PurchasePrize = x.PurchasePrize,
                    Id=x.Id
         
                }).OrderByDescending(x=>x.Id).FirstOrDefault();

                if (data==null)
                {

                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult SaveSalesInvoice(SalesInvoiceMas salesInvoiceMa, List<SalesInvoiceDet> SalesInvoiceDetails)
        {
            var result = new
            {
                id = 0,
                flag = false,
                message = "Error occured. !",
            };
            
        //    if (ModelState.IsValid)
          //  {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // salesInvoiceMa.UserFullName = Session["UserFullName"].ToString();
                        if (salesInvoiceMa.Id > 0)
                        {
                            var data = db.SalesInvoiceDets.Where(x => x.SalesInvoiceMasId == salesInvoiceMa.Id).ToList();
                            db.SalesInvoiceDets.RemoveRange(data);
                            db.SaveChanges();

                            salesInvoiceMa.Year = DateTime.Now.Year;
                            db.Entry(salesInvoiceMa).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                       else
	                    {
                            salesInvoiceMa.Year = DateTime.Now.Year;
                            salesInvoiceMa.CreatedBy = Convert.ToInt32(Session["uid"].ToString());
                            salesInvoiceMa.CreatedDate = DateTime.Now;
                            db.SalesInvoiceMas.Add(salesInvoiceMa);
                            db.SaveChanges();

                        }
                        
                        foreach (var item in SalesInvoiceDetails)
                        {
                            SalesInvoiceDet det = new SalesInvoiceDet();

                            det.SalesInvoiceMasId = salesInvoiceMa.Id;
                            det.ProductCategoryId = item.ProductCategoryId;
                            det.ProductId = item.ProductId;
                            det.PurchasePrize = item.PurchasePrize;
                            det.UpdatedPrize = item.UpdatedPrize;
                            det.SalesPrize = item.SalesPrize;
                            det.Quantity = item.Quantity;
                            det.Discount = item.Discount;
                            det.Amount = item.Amount;
                            
                            db.SalesInvoiceDets.Add(det);
                            db.SaveChanges();
                        }

                        transaction.Commit();
                        result = new
                        {   id= salesInvoiceMa.Id,
                            flag = true,
                            message = "Succes occured. !",
                        };

                        //  TempData["Master"] = salesInvoiceMa;
                        //  TempData["Details"] = SalesInvoiceDetails;
                        //-------------Print Memo on Sale-------//
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = new
                        {
                            id = 0,
                            flag = false,
                            message = "Fail occured. !",
                        };

                        var message = ex.Message;
                    }
                }
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintMemo()
        {
           try
          {
                var Masters = (SalesInvoiceMas)TempData["Master"];
                var CustomerData = db.Customers.Where(x => x.Id == Masters.CustomerId).FirstOrDefault();


                //var Master = (from customer in db.Customers join  mas in Masters on customer.Id equals mas.CustomerId
                               
                           
                //              select new
                //             {
                //                 CustomerName = customer.CustomerName,
                //                 Phone = customer.Phone,
                //                 inVoiceNo = mas.InvoiceNo,
                //             }).


          /*  List<SalesInvoiceDet>*/
                                      var Details =  (List<SalesInvoiceDet>)TempData["Details"];

                var data = (from det in Details join
                           products in db.Products on det.ProductId equals products.Id

                            join productCatagory in db.ProductCategories on det.ProductCategoryId equals
                            productCatagory.Id select new
                            {
                                ProductCategoryName=productCatagory.CategoryName,
                                products.ProductName,
                                SalesPrize=det.SalesPrize,

                                Quantity = det.Quantity,
                                Discount =det.Discount,
                                Amount = det.Amount
                            }
                            
                            ).ToList();


            List <VMSalesMemo> salesMemo = new List<VMSalesMemo>();

            foreach (var item in data)
            {
                    salesMemo.Add(new VMSalesMemo
                    {
                        ProductCategoryName = item.ProductCategoryName,
                        ProductName = item.ProductName,
                        SalesPrize = item.SalesPrize,

                        Quantity = (int)(item.Quantity ?? 0),
                        Discount = (int)(item.Discount ?? 0),
                        Amount = (decimal)(item.Amount ?? 0.00m)

                    });

                //    salesMemo.Add(new VMSalesMemo
                //{
                //    ProductCategoryName=item.ProductCategory.CategoryName,
                //    ProductName=item.Product.ProductName,

                  
                //    SalesPrize = item.SalesPrize,
                //    Quantity = (int)(item.Quantity ?? 0),
                //    Discount = (int)(item.Discount ?? 0),
                //    Amount = (decimal)(item.Amount ?? 0.00m)

                //});
            }
            //----- Add Company Information To Report--------//
            var companyInfo = db.CompanyInformations.FirstOrDefault();



            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.PageCountMode = new PageCountMode();
            reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/PrintMemoReport.rdlc");

            List<ReportParameter> paraList = new List<ReportParameter>();
            paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));

            paraList.Add(new ReportParameter("Email", companyInfo.Email));
            paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
            paraList.Add(new ReportParameter("Address", companyInfo.Address));
            paraList.Add(new ReportParameter("Date", Masters.Date.ToString("dd/MM/yyyy")));
            paraList.Add(new ReportParameter("CustomerName", CustomerData.CustomerName));
            paraList.Add(new ReportParameter("PhoneNumber", CustomerData.Phone));
            paraList.Add(new ReportParameter("SalesInvoiceNo", Masters.InvoiceNo));
       


 
              reportViewer.LocalReport.SetParameters(paraList);

             ReportDataSource A = new ReportDataSource("DataSet1", salesMemo);
             reportViewer.LocalReport.DataSources.Add(A);
             reportViewer.ShowRefreshButton = false;


                TempData["PrintMemo"] = reportViewer;

                //ViewBag.ReportViewer = reportViewer;

                return Json(true, JsonRequestBehavior.AllowGet);
            //return View("~/Views/SalesInvoice/PrintMemo.cshtml");
            }
            catch (Exception ex)
            {

                var message = ex.Message;
            }
            return View();
        }

        //--- Get Saved Data For Edit-----//
        public JsonResult GetSavedData(int? Id)
        {
            var data = db.SalesInvoiceDets.Where(x => x.SalesInvoiceMasId == Id).Select(x => new
            {
                ProductCategoryId = x.ProductCategoryId,
                ProductCatagoryName = x.ProductCategory.CategoryName,
                ProductId = x.ProductId,
                ProductName = x.Product.ProductName,
                PurchasePrize = x.PurchasePrize,
                UpdatedPrize = x.UpdatedPrize,
                SalesPrize = x.SalesPrize,
                Qty = x.Quantity,
                Discount = x.Discount,
                Amount = x.Amount,
                SerialNo = ""
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]

        public JsonResult UpdateSalesInvoice(SalesInvoiceMas salesInvoiceMa, List<SalesInvoiceDet> SalesInvoiceDetails)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !",
            };
          
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //salesInvoiceMa.UserFullName = Session["UserFullName"].ToString();

                        if (salesInvoiceMa.Id > 0)
                        {
                            var data = db.SalesInvoiceDets.Where(x => x.SalesInvoiceMasId == salesInvoiceMa.Id).ToList();
                            db.SalesInvoiceDets.RemoveRange(data);
                            db.SaveChanges();
                        }
                        salesInvoiceMa.Year = DateTime.Now.Year;
                        db.Entry(salesInvoiceMa).State = EntityState.Modified;
                        db.SaveChanges();
                        
                        foreach (var item in SalesInvoiceDetails)
                        {
                            SalesInvoiceDet det = new SalesInvoiceDet();

                            det.SalesInvoiceMasId = salesInvoiceMa.Id;
                            det.ProductCategoryId = item.ProductCategoryId;
                            det.ProductId = item.ProductId;
                            det.PurchasePrize = item.PurchasePrize;
                            det.SalesPrize = item.SalesPrize;
                            det.Quantity = item.Quantity;
                            det.Discount = item.Discount;
                            det.Amount = item.Amount;
                            
                            db.SalesInvoiceDets.Add(det);
                            db.SaveChanges();
                        }

                        //var flag = "Edit";
                        //StockHelper.UpdateStockOnSale(flag, SalesInvoiceDetails);

                        transaction.Commit();
                        result = new
                        {
                            flag = true,
                            message = "Succes occured. !",
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = new
                        {
                            flag = false,
                            message = "Fail occured. !",
                        };

                        var message = ex.Message;
                        
                    }
               
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintMemoReport()
        {
            var reportViewer = (ReportViewer) TempData["PrintMemo"];

            ViewBag.ReportViewer = reportViewer;
            return View("~/Views/SalesInvoice/PrintMemo.cshtml");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
