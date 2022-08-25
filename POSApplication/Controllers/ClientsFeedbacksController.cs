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
    public class ClientsFeedbacksController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: ClientsFeedbacks
        public ActionResult Index()
        {
            return View(db.ClientsFeedbacks.ToList());
        }

        // GET: ClientsFeedbacks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsFeedback clientsFeedback = db.ClientsFeedbacks.Find(id);
            if (clientsFeedback == null)
            {
                return HttpNotFound();
            }
            return View(clientsFeedback);
        }

        // GET: ClientsFeedbacks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientsFeedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClientsName,ImageURL,Description")] ClientsFeedback clientsFeedback)
        {
            if (ModelState.IsValid)
            {
                db.ClientsFeedbacks.Add(clientsFeedback);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientsFeedback);
        }

        // GET: ClientsFeedbacks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsFeedback clientsFeedback = db.ClientsFeedbacks.Find(id);
            if (clientsFeedback == null)
            {
                return HttpNotFound();
            }

            ViewBag.Images = db.ClientsFeedbacks.Where(x => x.Id == id).ToList();
            return View(clientsFeedback);
        }

        // POST: ClientsFeedbacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ClientsName,ImageURL,Description")] ClientsFeedback clientsFeedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientsFeedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientsFeedback);
        }

        // GET: ClientsFeedbacks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsFeedback clientsFeedback = db.ClientsFeedbacks.Find(id);
            if (clientsFeedback == null)
            {
                return HttpNotFound();
            }
            return View(clientsFeedback);
        }

        // POST: ClientsFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientsFeedback clientsFeedback = db.ClientsFeedbacks.Find(id);
            db.ClientsFeedbacks.Remove(clientsFeedback);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult SaveData(ClientsFeedback clients)
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

                        db.ClientsFeedbacks.Add(clients);
                        db.SaveChanges();

                        // get id

                        //var sliderimageid = ;

                        TempData["imageid"] = clients.Id;

                        // save Image With Id


                        var check = ImageSave(clients.Id);

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


                        var path = Path.Combine(Server.MapPath("~/Content/EcommerceContent/clients/"));
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


                            var clientsimage = db.ClientsFeedbacks.Find(imageid);
                            //sliderimage.Id = UserId;
                            string imagepath = "~/Content/EcommerceContent/clients/" + updatedFileName;
                            clientsimage.ImageURL = imagepath;

                            db.Entry(clientsimage).State = EntityState.Modified;



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
                        var deleteImage = db.ClientsFeedbacks.SingleOrDefault(x => x.Id == Id);
                        string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                        System.IO.File.Delete(path);


                        // delete from database

                        var Item = db.ClientsFeedbacks.SingleOrDefault(x => x.Id == Id);
                        db.ClientsFeedbacks.Remove(Item);
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



        [HttpPost]
        public ActionResult ImageUpdate(int? UId)
        {
            int imageid = (int)(TempData["imageid"]);
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


                            var path = Path.Combine(Server.MapPath("~/Content/EcommerceContent/clients/"));
                            string pathString = System.IO.Path.Combine(path.ToString());
                            var newName = Path.GetFileName(file.FileName);
                            bool isExists = System.IO.Directory.Exists(pathString);
                            if (!isExists) System.IO.Directory.CreateDirectory(pathString);
                            {
                                var updatedFileName = UId + "!" + newName;
                                var uploadpath = string.Format("{0}{1}", pathString, updatedFileName);
                                file.SaveAs(uploadpath);

                                var clientsimage = db.SliderImages.Find(imageid);
                                string imagepath = "~/Content/EcommerceContent/clients/" + updatedFileName;
                                clientsimage.ImageURL = imagepath;

                                db.Entry(clientsimage).State = EntityState.Modified;

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

        public JsonResult UpdateData(ClientsFeedback clients)
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

                        var clientsfeedbck = db.ClientsFeedbacks.Find(clients.Id);

                        clientsfeedbck.ClientsName = clients.ClientsName;
                        clientsfeedbck.Description = clients.Description;



                        db.Entry(clientsfeedbck).State = EntityState.Modified;
                        db.SaveChanges();

                        //TempData["sliderimageid"] = sliderimage.Id;

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = clientsfeedbck.Id,
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
