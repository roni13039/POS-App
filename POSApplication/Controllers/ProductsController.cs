using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using POSApplication.Models;

namespace POSApplication.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.OrderBy(x=>x.ProductName).ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.SupplierId = new SelectList(db.Suppliers.OrderBy(x=>x.SupplierName), "Id", "SupplierName");
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories.OrderBy(x=>x.CategoryName), "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                ViewBag.Message = "Succeess";
              
            }
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "SupplierName");
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories, "Id", "CategoryName");
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories, "Id", "CategoryName", product.ProductCategoryId);

            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Images = db.Products.Where(x => x.Id == id).ToList();
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Message = "Succeess";
           
            }
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "SupplierName");
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories, "Id", "CategoryName");

            return View(product);
        }


        // GET: Products/Edit/5
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories, "Id", "CategoryName", product.ProductCategoryId);

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }




        public JsonResult DeleteItem(int Id)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !"
            };

            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //var checkData = db.Suppliers.Where(x => x.Id == Id).ToList();
                        //db.Suppliers.RemoveRange(checkData);
                        //db.SaveChanges();


                        var DeleteFromPurchase = db.PurchaseInvoiceDets.Where(x => x.ProductId == Id).ToList(); 
                        db.PurchaseInvoiceDets.RemoveRange(DeleteFromPurchase);
                        db.SaveChanges();

                        var DeleteFromSales = db.SalesInvoiceDets.Where(x => x.ProductId == Id).ToList();
                        db.SalesInvoiceDets.RemoveRange(DeleteFromSales);
                        db.SaveChanges();

                        //var deleteImage = db.Products.SingleOrDefault(x => x.Id == Id);
                        //string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));
                        //System.IO.File.Delete(path);

                        var Item = db.Products.SingleOrDefault(x => x.Id == Id);
                        db.Products.Remove(Item);
                        db.SaveChanges();

                        dbContextTransaction.Commit();
                        result = new
                        {
                            flag = true,
                            message = "Update successful !!"
                        };
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        result = new
                        {
                            flag = false,
                            message = ex.Message
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                result = new
                {
                    flag = false,
                    message = ex.Message
                };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveData(Product master)
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
                        db.Products.Add(master);
                        db.SaveChanges();
                        transaction.Commit();
                        // TempData["sliderimageid"] = master.Id;
                        // save Image With Id
                        // var checck = ImageSave(master.Id);
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
        public bool ImageSave(int Id)
        {

            int imageid = Id;
            var UId = imageid;



            bool isSavedSuccessfully = false;

            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {


                        var path = Path.Combine(Server.MapPath("~/Content/EcommerceContent/productimages/"));
                        string pathString = System.IO.Path.Combine(path.ToString());
                        var newName = Path.GetFileName(file.FileName);
                        bool isExists = System.IO.Directory.Exists(pathString);
                        if (!isExists) System.IO.Directory.CreateDirectory(pathString);
                        {
                            var updatedFileName = UId + "!" + newName;
                            //var uploadpath = string.Format("{0}\\{1}", pathString, file.FileName);
                            var uploadpath = string.Format("{0}{1}", pathString, updatedFileName);
                            file.SaveAs(uploadpath);
                            //save in db
                            //SliderImage sliderimage = new SliderImage();


                            var image = db.Products.Find(imageid);
                            //sliderimage.Id = UserId;
                            string imagepath = "~/Content/EcommerceContent/productimages/" + updatedFileName;
                            image.ImageURL = imagepath;

                            db.Entry(image).State = EntityState.Modified;



                            //db.SliderImages.Add(sliderimage);
                            db.SaveChanges();

                            isSavedSuccessfully = true;
                        }



                    }
                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            return true;
        }
        public JsonResult UpdateData(Product master)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !",
                Id = 0,
                isRedirect = false
            };

            try
            {
                //var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //var userId = Convert.ToInt32(Session["uid"]);

                        var mas = db.Products.Find(master.Id);
                        mas.ProductCode = master.ProductCode;
                        mas.ProductName = master.ProductName;
                    
                        mas.ProductCategoryId = master.ProductCategoryId;
                    
                        //mas.SalesPrice = master.SalesPrice;
                       // mas.Discount = master.Discount;

                        db.Entry(mas).State = EntityState.Modified;
                        db.SaveChanges();

                       // var checck = ImageSave(master.Id);
                        //TempData["sliderimageid"] = sliderimage.Id;

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = master.Id,
                            isRedirect = true
                        };



                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        result = new
                        {
                            flag = false,
                            message = ex.Message,
                            Id = 0,
                            isRedirect = false
                        };

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new
                {
                    flag = false,
                    message = ex.Message,
                    Id = 0,
                    isRedirect = false
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }




        public JsonResult DeleteImage(int Id)
        {

            bool flag = false;
            try
            {
                var deleteImage = db.Products.SingleOrDefault(x => x.Id == Id);
                string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                System.IO.File.Delete(path);

                db.Products.Remove(deleteImage);
                flag = db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {

            }

            if (flag)
            {
                var result = new
                {
                    flag = true,
                    message = "Image deletion successful."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Image deletion failed!\nError Occured."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
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
