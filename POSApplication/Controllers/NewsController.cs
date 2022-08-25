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
    public class NewsController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: News
        public ActionResult Index()
        {
            return View(db.News.ToList());
        }

        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ImageURL")] News news)
        {
            if (ModelState.IsValid)
            {
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }

            ViewBag.Images = db.News.Where(x => x.Id == id).ToList();
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ImageURL")] News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult SaveData(News news)
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

                        db.News.Add(news);
                        db.SaveChanges();

                        // get id

                        //var sliderimageid = ;

                        TempData["imageid"] = news.Id;

                        // save Image With Id


                        var check = ImageSave(news.Id);

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
                        var path = Path.Combine(Server.MapPath("~/Content/EcommerceContent/news/"));
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


                            var newsimage = db.News.Find(imageid);
                            //sliderimage.Id = UserId;
                            string imagepath = "/Content/EcommerceContent/news/" + updatedFileName;
                            //string imagepath = "~/Content/EcommerceContent/news/" + updatedFileName;
                            newsimage.ImageURL = imagepath;

                            db.Entry(newsimage).State = EntityState.Modified;
                            
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
                        var deleteImage = db.News.SingleOrDefault(x => x.Id == Id);
                        string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                        System.IO.File.Delete(path);
                        
                        // delete from database

                        var Item = db.News.SingleOrDefault(x => x.Id == Id);
                        db.News.Remove(Item);
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

        public JsonResult UpdateData(News master)
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

                        var mas = db.News.Find(master.Id);

                        mas.Name = master.Name;
                        mas.Description = master.Description;
                        

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
                var deleteImage = db.News.SingleOrDefault(x => x.Id == Id);
                string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                System.IO.File.Delete(path);

                db.News.Remove(deleteImage);
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
