using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using POSApplication.Models;
using System.Web.Security;
using System.Data.SqlClient;
using System.Transactions;
using POSApplication.Helpers;
using POSApplication.ViewModel;

namespace POSApplication.Controllers
{
    [Authorize]
    public class PurchaseInvoiceController : Controller
    {
        private POSDBContext db = new POSDBContext();

        
        public ActionResult ExpiraProductList()
        {
 

            return View();
        }
        public ActionResult ShowExpiraProductList(string serialNumber)
        {

            var data = db.PurchaseInvoiceDets.Where(x =>x.SerialNo.Contains(serialNumber)).ToList();

            return View(data);
        }
        
        [HttpPost]
        public JsonResult UpdateProductQtyForExpire(List<VMExpireProducts>DetailsValue)
        {
            var result = new
            {
                flag = false,
                message = "Error occured.!",
            };
            try
            {
                if (DetailsValue.Count > 0)
                {
                    foreach (var item in DetailsValue)
                    {
                        var data = db.PurchaseInvoiceDets.Where(x => x.Id == item.Id).FirstOrDefault();
                        data.Quantity = data.Quantity - item.quantity;
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    result = new
                    {
                        flag = true,
                        message = "Succes occured. !",
                    };
                }
            }
            catch (Exception ex)
            {
                result = new
                {
                    flag = false,
                    message = "Fail occured. !",
                };
                var message = ex.Message;
            }
                
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View(db.PurchaseInvoiceMas.OrderBy(x => x.Date).ToList());

        }
        public ActionResult Create()
        {
            ViewBag.SupplierId = new SelectList(db.Suppliers.ToList().Distinct().OrderBy(x=>x.SupplierName), "Id", "SupplierName");
            return View();
        }





        [HttpPost]    
        public JsonResult SavePurchaseInvoice(PurchaseInvoiceMa purchaseInvoiceMa, List<PurchaseInvoiceDet> PurchaseInvoicedDetails)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !",

            };


            if (ModelState.IsValid)
            {               
                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        purchaseInvoiceMa.UserId =(int) Session["uid"];
                        
                        db.PurchaseInvoiceMas.Add(purchaseInvoiceMa);
                        db.SaveChanges();

                        foreach (var item in PurchaseInvoicedDetails)
                        {
                            PurchaseInvoiceDet det = new PurchaseInvoiceDet();
                            det.PurchaseInvoiceMasId = purchaseInvoiceMa.Id;
                            det.ProductCategoryId = item.ProductCategoryId;
                            det.Quantity = item.Quantity;
                            det.SerialNo = item.SerialNo;
                            det.PurchasePrize = item.PurchasePrize;
                            det.Amount = item.Amount;
                            det.ProductId = item.ProductId;
                            det.ExpireDate = item.ExpireDate;
                            det.SalesPrice = item.SalesPrice;
                            
                            db.PurchaseInvoiceDets.Add(det);
                            db.SaveChanges();
                        }
                        transaction.Complete();
                        result = new
                        {
                            flag = true,
                            message = "Succes occured. !",

                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Dispose();
                        result = new
                        {
                            flag = false,
                            message = "Fail occured. !",

                        };

                        var message = ex.Message;

                    }
                }


            }

                return Json(result,JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult UpdatePurchaseInvoice(PurchaseInvoiceMa purchaseInvoiceMa, List<PurchaseInvoiceDet> PurchaseInvoicedDetails)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !",

            };
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {

                    try
                    {
                        if (purchaseInvoiceMa.Id > 0)
                        {
                            var data = db.PurchaseInvoiceDets.Where(x => x.PurchaseInvoiceMasId == purchaseInvoiceMa.Id).ToList();
                            db.PurchaseInvoiceDets.RemoveRange(data);
                            db.SaveChanges();
                        }
                        purchaseInvoiceMa.UserId = (int)Session["uid"];

                        db.Entry(purchaseInvoiceMa).State = EntityState.Modified;
                        db.SaveChanges();

                        foreach (var item in PurchaseInvoicedDetails)
                        {
                            PurchaseInvoiceDet det = new PurchaseInvoiceDet();
                  
                            det.PurchaseInvoiceMasId = purchaseInvoiceMa.Id;
                            det.ProductCategoryId = item.ProductCategoryId;
                            det.Quantity = item.Quantity;
                            det.PurchasePrize = item.PurchasePrize;
                            det.Amount = item.Amount;
                            det.ProductId = item.ProductId;
                            det.SerialNo = item.SerialNo;
                            det.ExpireDate = item.ExpireDate;

                            db.PurchaseInvoiceDets.Add(det);
                            db.SaveChanges();
                        }
                   
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
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: PurchaseInvoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseInvoiceMa purchaseInvoiceMa = db.PurchaseInvoiceMas.Find(id);
            if (purchaseInvoiceMa == null)
            {
                return HttpNotFound();
            }
            ViewBag.SupplierId = new SelectList(db.Suppliers.Distinct().OrderBy(x=>x.SupplierName).ToList(), "Id", "SupplierName", purchaseInvoiceMa.SupplierId);


            ViewBag.CompanyName = purchaseInvoiceMa.Supplier.CompanyName;
            ViewBag.Email = purchaseInvoiceMa.Supplier.Email;
            ViewBag.Phone = purchaseInvoiceMa.Supplier.Phone;

            return View(purchaseInvoiceMa);
        }

        // POST: PurchaseInvoice/Edit/5


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyName,Phone,Email,Date,Secu_UserId")] PurchaseInvoiceMa purchaseInvoiceMa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseInvoiceMa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(purchaseInvoiceMa);
        }
        //--- Get Saved Data For Edit-----//
        public JsonResult GetSavedData(int? Id)
        {
          var data = db.PurchaseInvoiceDets.Where(x => x.PurchaseInvoiceMasId == Id).Select(x => new
                {
                    ProductCategoryId = x.ProductCategoryId,
                    ProductCatagoryName=x.ProductCategory.CategoryName,
                    ProductId = x.ProductId,
                    ProductName=x.Product.ProductName,
                    Qty=x.Quantity,
                    Amount= x.Amount,
                    PurchasePrize=x.PurchasePrize,
                    SerialNo = x.SerialNo??"",
                    ExpireDate = x.ExpireDate==null? "" : x.ExpireDate.ToString()
          }).ToList();

           return Json(data, JsonRequestBehavior.AllowGet);
            }
        public JsonResult GetSupplierRelatedData(int? SupplierId)
        {
            if (SupplierId == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = db.Suppliers.Where(x => x.Id == SupplierId).Select(x => new
                {
                    CompanyName = x.CompanyName,
                    Phone = x.Phone,
                    Email = x.Email

                }).FirstOrDefault();

                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetProductCatagoryName()
        {

            var data = db.ProductCategories.Distinct().ToList().Select(x => new
            {
                Value = x.Id,
                Text = x.CategoryName

            }).Distinct().OrderBy(x=>x.Text).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetProductName()
        {

            var data = db.Products.Distinct().ToList().Select(x => new
            {

                Value = x.Id,
                Text = x.ProductName

            }).Distinct().OrderBy(x=>x.Text).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);


        }
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


        //public JsonResult GetPrizeByProductId(int? ProductId)
        //{
        //    if (ProductId == null)
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var data = db.Products.Where(x => x.Id == ProductId).Select(x => new
        //        {


        //            PurchasePrize = x.PurchasePrice

        //        }).FirstOrDefault();

        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }

        //}

        [HttpPost]
       
        public ActionResult Delete(int Id)
        {
            PurchaseInvoiceMa purchaseInvoiceMa = db.PurchaseInvoiceMas.Find(Id);
            db.PurchaseInvoiceMas.Remove(purchaseInvoiceMa);
            db.SaveChanges();
            return Json(true,JsonRequestBehavior.AllowGet);
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
