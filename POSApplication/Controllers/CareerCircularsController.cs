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
    public class CareerCircularsController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: CareerCirculars
        public ActionResult Index()
        {
            return View(db.CareerCirculars.ToList());
        }

        // GET: CareerCirculars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareerCircular careerCircular = db.CareerCirculars.Find(id);
            if (careerCircular == null)
            {
                return HttpNotFound();
            }
            return View(careerCircular);
        }

        // GET: CareerCirculars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CareerCirculars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,JobTitle,ImageURL,PublishedDate,ExpiredDate")] CareerCircular careerCircular)
        {
            if (ModelState.IsValid)
            {
                db.CareerCirculars.Add(careerCircular);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(careerCircular);
        }

        // GET: CareerCirculars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareerCircular careerCircular = db.CareerCirculars.Find(id);
            if (careerCircular == null)
            {
                return HttpNotFound();
            }
            ViewBag.Images = db.CareerCirculars.Where(x => x.Id == id).ToList();
            return View(careerCircular);
        }

        // POST: CareerCirculars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,JobTitle,ImageURL,PublishedDate,ExpiredDate")] CareerCircular careerCircular)
        {
            if (ModelState.IsValid)
            {
                db.Entry(careerCircular).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(careerCircular);
        }

        // GET: CareerCirculars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareerCircular careerCircular = db.CareerCirculars.Find(id);
            if (careerCircular == null)
            {
                return HttpNotFound();
            }
            return View(careerCircular);
        }

        // POST: CareerCirculars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CareerCircular careerCircular = db.CareerCirculars.Find(id);
            db.CareerCirculars.Remove(careerCircular);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveData(CareerCircular career)
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

                        db.CareerCirculars.Add(career);
                        db.SaveChanges();

                        // get id

                        //var sliderimageid = ;

                        TempData["imageid"] = career.Id;

                        // save Image With Id


                        var check = ImageSave(career.Id);

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
                        var path = Path.Combine(Server.MapPath("~/Content/EcommerceContent/career/"));
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
                            
                            var careerimage = db.CareerCirculars.Find(imageid);
                            //sliderimage.Id = UserId;
                            string imagepath = "~/Content/EcommerceContent/career/" + updatedFileName;
                            careerimage.ImageURL = imagepath;

                            db.Entry(careerimage).State = EntityState.Modified;
                            
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
                        var deleteImage = db.CareerCirculars.SingleOrDefault(x => x.Id == Id);
                        string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                        System.IO.File.Delete(path);
                        
                        var Item = db.CareerCirculars.SingleOrDefault(x => x.Id == Id);
                        db.CareerCirculars.Remove(Item);
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


        public JsonResult UpdateData(CareerCircular career)
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

                        var carreer = db.CareerCirculars.Find(career.Id);

                        carreer.JobTitle = career.JobTitle;
                        carreer.PublishedDate = career.PublishedDate;
                        carreer.ExpiredDate = career.ExpiredDate;



                        db.Entry(carreer).State = EntityState.Modified;
                        db.SaveChanges();

                        //TempData["sliderimageid"] = sliderimage.Id;

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = carreer.Id,
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
