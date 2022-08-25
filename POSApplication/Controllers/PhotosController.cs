using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using POSApplication.Models;
using System.IO;

namespace POSApplication.Controllers
{
    public class PhotosController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: Photos
        public ActionResult Index()
        {
            return View(db.Photos.ToList());
        }

        // GET: Photos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // GET: Photos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ImageURL")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Photos.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(photo);
        }

        // GET: Photos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Images = db.Photos.Where(x => x.Id == id).ToList();
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ImageURL")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(photo);
        }

        // GET: Photos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photos.Find(id);
            db.Photos.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult SaveData(Photo photo)
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

                        db.Photos.Add(photo);
                        db.SaveChanges();

                        // get id

                        //var sliderimageid = ;

                        TempData["imageid"] = photo.Id;

                        // save Image With Id


                        var check = ImageSave(photo.Id);

                        transaction.Commit();
                        result = new
                        {
                            flag = true,
                            message = "Success occured. !",
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


                        var path = Path.Combine(Server.MapPath("~/Content/EcommerceContent/photos/"));
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


                            var photosimage = db.Photos.Find(imageid);
                            //sliderimage.Id = UserId;
                            string imagepath = "~/Content/EcommerceContent/photos/" + updatedFileName;
                            photosimage.ImageURL = imagepath;

                            db.Entry(photosimage).State = EntityState.Modified;



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



        public JsonResult UpdateData(Photo photos)
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

                        var photooo = db.Photos.Find(photos.Id);

                        photooo.Name = photos.Name;



                        db.Entry(photooo).State = EntityState.Modified;
                        db.SaveChanges();

                        //TempData["sliderimageid"] = sliderimage.Id;

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = photooo.Id,
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
                    {// delete from server
                        var deleteImage = db.Photos.SingleOrDefault(x => x.Id == Id);
                        string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                        System.IO.File.Delete(path);


                        // delete from database

                        var Item = db.Photos.SingleOrDefault(x => x.Id == Id);
                        db.Photos.Remove(Item);
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
            //}


            return Json(result, JsonRequestBehavior.AllowGet);


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
