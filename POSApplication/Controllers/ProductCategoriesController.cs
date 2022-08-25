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
    public class ProductCategoriesController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: ProductCategories
        public ActionResult Index()
        {
            return View(db.ProductCategories.OrderBy(x=>x.CategoryName).ToList());
        }

        // GET: ProductCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // GET: ProductCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryName")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                db.ProductCategories.Add(productCategory);
                db.SaveChanges();

                ViewBag.Message = "Succeess";
                return View();
            }

            return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }

            ViewBag.Images = db.ProductCategories.Where(x => x.Id == id).ToList();
            return View(productCategory);
        }

        // POST: ProductCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryName")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productCategory).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Message = "Succeess";
                return View();
            }
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductCategory productCategory = db.ProductCategories.Find(id);
            db.ProductCategories.Remove(productCategory);
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


                        

                        var Item = db.ProductCategories.SingleOrDefault(x => x.Id == Id);
                        db.ProductCategories.Remove(Item);
                        db.SaveChanges();

                        string path = Path.Combine(Server.MapPath(Item.ImageURL));
                        System.IO.File.Delete(path);

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
        public ActionResult SaveData(ProductCategory master)
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
                        //salesInvoiceMa.UserFullName = Session["UserFullName"].ToString();

                        db.ProductCategories.Add(master);
                        db.SaveChanges();

                        // get id

                        //var sliderimageid = ;

                        //  TempData["sliderimageid"] = master.Id;

                        // save Image With Id


                        // var checck = ImageSave(master.Id);

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
                        

                        var path = Path.Combine(Server.MapPath("~/Content/EcommerceContent/productcategoriesimages/"));
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


                            var image = db.ProductCategories.Find(imageid);
                            //sliderimage.Id = UserId;
                            string imagepath = "~/Content/EcommerceContent/productcategoriesimages/" + updatedFileName;
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




        [HttpPost]
        public ActionResult ImageUpdate(int? UId)
        {
            int sliderimageid = (int)(TempData["sliderimageid"]);
            //var UId = sliderimageid;
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
                        if (file.FileName.ToLower().EndsWith("jpg") || file.FileName.ToLower().EndsWith("png"))
                        {


                            var path = Path.Combine(Server.MapPath("~/Content/EcommerceContent/sliderimages/"));
                            string pathString = System.IO.Path.Combine(path.ToString());
                            var newName = Path.GetFileName(file.FileName);
                            bool isExists = System.IO.Directory.Exists(pathString);
                            if (!isExists) System.IO.Directory.CreateDirectory(pathString);
                            {
                                var updatedFileName = UId + "!" + newName;
                                var uploadpath = string.Format("{0}{1}", pathString, updatedFileName);
                                file.SaveAs(uploadpath);

                                var sliderimage = db.SliderImages.Find(sliderimageid);
                                string imagepath = "~/Content/EcommerceContent/sliderimages/" + updatedFileName;
                                sliderimage.ImageURL = imagepath;

                                db.Entry(sliderimage).State = EntityState.Modified;

                                db.SaveChanges();





                                //SliderImage upload = new SliderImage();
                                //upload.Id = 0;
                                //string imagepath = "~/Content/images/ProfilePic/" + updatedFileName;
                                //upload.ImageURL = imagepath;
                                //upload.ImageDate = DateTime.Now;


                                //upload.Secu_UserId = UId ?? 0;



                                //db.UserImage.Add(upload);
                                //db.SaveChanges();

                                isSavedSuccessfully = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            return RedirectToAction("Index");
        }



        public JsonResult UpdateData(ProductCategory master)
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

                        var mas = db.ProductCategories.Find(master.Id);

                        mas.CategoryName = master.CategoryName ;
                        
                        db.Entry(mas).State = EntityState.Modified;
                        db.SaveChanges();

                        var checck = ImageSave(master.Id);


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
                var deleteImage = db.ProductCategories.SingleOrDefault(x => x.Id == Id);
                string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                System.IO.File.Delete(path);

                //db.ProductCategories.Remove(deleteImage);
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
