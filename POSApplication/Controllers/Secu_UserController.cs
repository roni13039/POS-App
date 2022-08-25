using POSApplication.Helpers;
using POSApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace POSApplication.Controllers
{
    [Authorize]
    public class Secu_UserController : Controller
    {
        private POSDBContext db = new POSDBContext();
        private EncryptionDecryptionUtil encryptionDecryptionUtil = new EncryptionDecryptionUtil();
        private int saltLength = 5;

        // GET: Secu_User
        public ActionResult Index()
        {
            var IsAdmin = Convert.ToBoolean(Session["IsAdmin"].ToString());

            if (IsAdmin)
            {
                return View(db.Secu_User.ToList());
            }
            else
            {
                return View(db.Secu_User.Where(x => x.IsAdmin == null || x.IsAdmin == false).ToList());
            }

        }

        // GET: Secu_User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Secu_User secu_User = db.Secu_User.Find(id);
            if (secu_User == null)
            {
                return HttpNotFound();
            }
            return View(secu_User);
        }



        // GET: Secu_User/Create
        public ActionResult Create()
        {
            ViewBag.UserStatus = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Inactive", Value = "0" }, new SelectListItem { Text = "Active", Value = "1" } }, "Value", "Text");
            ViewBag.RoleId = new SelectList(db.Secu_Role, "Id", "Name");
          

            return View();
        }

        


        public JsonResult SaveUser(Secu_User User)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !",
                Id = 0
            };

            try
            {
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        string salt = encryptionDecryptionUtil.GenerateSalt(saltLength);

                        //
                        var user = new Secu_User()
                        {
                            Id = User.Id,
                            UserName = User.UserName,
                            UserStatus = User.UserStatus,
                            Phone = User.Phone,
                            Email = User.Email,
                            //Password = User.Password,
                            Salt = salt,
                            Password = encryptionDecryptionUtil.CreatePasswordHash(User.Password, salt),
                          
                            UserFullName = User.UserFullName,
                            RoleId = User.RoleId,
                            IsAdmin = User.IsAdmin
                        };


                        //db.Entry(user).State = EntityState.Added;
                        db.Secu_User.Add(user);
                        db.SaveChanges();

                        TempData["myData"] = user.Id;
                        //-------------------
                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = user.Id
                        };

                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        result = new
                        {
                            flag = false,
                            message = ex.Message,
                            Id = 0
                        };
                    }
                }

            }
            catch (Exception ex)
            {

                result = new
                {
                    flag = false,
                    message = ex.Message,
                    Id = 0
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ChangePassword(int userid, string pass)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !",
                Id = 0
            };

            try
            {
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Secu_User user = db.Secu_User.Find(userid);

                        user.Salt = encryptionDecryptionUtil.GenerateSalt(saltLength);
                        user.Password = encryptionDecryptionUtil.CreatePasswordHash(pass, user.Salt);


                        //user.Password = pass;
                        db.Entry(user).State = EntityState.Modified;
                        //db.Secu_User.Add(user);
                        db.SaveChanges();

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = user.Id
                        };
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        result = new
                        {
                            flag = false,
                            message = ex.Message,
                            Id = 0
                        };
                    }
                }
            }
            catch (Exception ex)
            {

                result = new
                {
                    flag = false,
                    message = ex.Message,
                    Id = 0
                };
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }



        public JsonResult UpdateUser(Secu_User User)
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
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        //var user = new Secu_User()
                        //{
                        //    Id = User.Id,
                        //    UserName = User.UserName,
                        //    UserStatus = User.UserStatus,
                        //    UserFullName = User.UserFullName,
                        //    TeamId = User.TeamId,
                        //    DesignationId = User.DesignationId,
                        //    IsAdmin = User.IsAdmin
                        //};
                        
                        var userId = Convert.ToInt32(Session["uid"]);

                        var user = db.Secu_User.Find(User.Id);


                        user.UserName = User.UserName;
                        user.UserFullName = User.UserFullName;
                        user.Phone = User.Phone;
                        user.Email = User.Email;
                        user.UserStatus = User.UserStatus;
                        user.RoleId = User.RoleId;
                        user.IsAdmin = User.IsAdmin;
              

                        db.Entry(user).State = EntityState.Modified;

                        //db.Secu_User.Add(user);
                        db.SaveChanges();

                        dbContextTransaction.Commit();

                   

                        if (user.Id == userId)
                        {
                            result = new
                            {
                                flag = true,
                                message = "Saving successful !!",
                                Id = user.Id,
                                isRedirect = true
                            };

                        }
                        else
                        {
                            result = new
                            {
                                flag = true,
                                message = "Saving successful !!",
                                Id = user.Id,
                                isRedirect = false
                            };
                        }


                        //result = new
                        //{
                        //    flag = true,
                        //    message = "Saving successful !!",
                        //    Id = user.Id
                        //};

                        //Success("Record saved successfully.", true);
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

        public ActionResult Edit(int? id)
        {
            //var IsSuperAdmin = Convert.ToBoolean(Session["IsSuperAdmin"].ToString());
            var IsAdmin = Convert.ToBoolean(Session["IsAdmin"].ToString());
            var userId = Convert.ToInt32(Session["uid"].ToString());

            if(IsAdmin || userId== id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Secu_User secu_User = db.Secu_User.Find(id);
                if (secu_User == null)
                {
                    return HttpNotFound();
                }

                ViewBag.UserStatus = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Inactive", Value = "0" }, new SelectListItem { Text = "Active", Value = "1" } }, "Value", "Text", secu_User.UserStatus);
                ViewBag.RoleId = new SelectList(db.Secu_Role, "Id", "Name", secu_User.RoleId);
              

                ViewBag.Images = db.UserImage.Where(x => x.Secu_UserId == secu_User.Id).ToList();
                ViewBag.ShowUploadBtn = db.UserImage.Where(x => x.Secu_UserId == secu_User.Id).Count() > 0 ? false : true;


                return View(secu_User);
            }
            else
            {
                return RedirectToAction("AccessDenied", "ErrorHandler");
            }


        }

        // POST: Secu_User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Password,Salt,UserFullName,IsAdmin,UserStatus,LastLoginDate,InvalidAttempt,TeamId,DesignationId,RoleId")] Secu_User secu_User)
        {
            if (ModelState.IsValid)
            {
                db.Entry(secu_User).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(secu_User);
        }

        // GET: Secu_User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Secu_User secu_User = db.Secu_User.Find(id);
            if (secu_User == null)
            {
                return HttpNotFound();
            }
            return View(secu_User);
        }

        // POST: Secu_User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Secu_User secu_User = db.Secu_User.Find(id);
            db.Secu_User.Remove(secu_User);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public JsonResult DeleteUser(int id)
        {
            bool flag = false;
            try
            {
                var itemToDeleteMas = db.Secu_User.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nUser Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //var checkUser = db.InvoiceCommDet.Where(x => x.InvoiceCommFactMasId == id).ToList();

                //if (checkUser.Count > 0)
                //{
                //    var result = new
                //    {
                //        flag = false,
                //        message = "Deletion failed!\nInvoice exists. Delete invoice data first."
                //    };
                //    return Json(result, JsonRequestBehavior.AllowGet);

                //}


                db.Secu_User.Remove(itemToDeleteMas);

                flag = db.SaveChanges() > 0;

            }
            catch (Exception)
            {

            }

            if (flag)
            {
                var result = new
                {
                    flag = true,
                    message = "Deletion successful."
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Deletion failed!\nError Occured."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult GetNames()
        {
            var data = db.Secu_User.Select(y => new { Name = y.UserName, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult GetNamesByCompany(int CompanyUnitId)
        //{
        //    var data = db.Secu_User.Where(x => x.CompanyUnitId == CompanyUnitId && x.IsSuperAdmin != true).Select(y => new { Name = y.UserName, Id = y.Id }).ToList();

        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}

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

                        var deleteImage = db.UserImage.Where(x => x.Secu_UserId == Id).FirstOrDefault();
                        //var deleteImage = db.UserImage.FirstOrDefault(x => x.Secu_UserId == Id);
                        //var deletedItem = db.ProjectSiteStatus.Where(x => x.ProjectSiteId == SiteId && x.ProjectId == ProjectId && x.PlanCode == PlanCode && x.SiteStatusDate == SiteStatusDate).ToList();

                        if (deleteImage != null)
                        {

                            string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                            System.IO.File.Delete(path);



                            var deletedItem = db.UserImage.Where(x => x.Secu_UserId == Id).ToList();
                            db.UserImage.RemoveRange(deletedItem);
                            db.SaveChanges();

                        }

                        var Item = db.Secu_User.SingleOrDefault(x => x.Id == Id);
                        db.Secu_User.Remove(Item);
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


        [HttpPost]
        public ActionResult ImageUploadForSaveAndUpdate(int? id)
        {
            //update
            if (id!=null)
            {
                var data = db.UserImage.Find(id);
                string fullPath = data.ImageURL;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                db.UserImage.Remove(data);
                db.SaveChanges();
            }
           
            //update

          var UserId = (int)(TempData["myData"]);

            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    if (file != null && file.ContentLength > 0)
                    {
                        if (file.FileName.ToLower().EndsWith("jpg") || file.FileName.ToLower().EndsWith("png") || file.FileName.ToLower().EndsWith("jpeg") || file.FileName.ToLower().EndsWith("bmp") || file.FileName.ToLower().EndsWith("gif") )
                        {

                            var path = Path.Combine(Server.MapPath("~/Content/images/ProfilePic/").ToString());

                            var updatedFileName = "Pp" + UserId + "!" + file.FileName;
                            var uploadpath = string.Format("{0}{1}", path, updatedFileName);                           
                            file.SaveAs(uploadpath);
                            
                            //save in db
                            UserImage upload = new UserImage();

                            upload.Id = 0;
                            upload.ImageURL = "~/Content/images/ProfilePic/" + updatedFileName;
                            upload.ImageDate = DateTime.Now;
                            upload.Secu_UserId = UserId;

                            db.UserImage.Add(upload);
                            db.SaveChanges();

                            return RedirectToAction("Index");

                        }
                    }
                }
                  
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return RedirectToAction("NotFound", "ErrorHandler");
            }
            
           return RedirectToAction("Index");
            


        }
        [HttpPost]
        public ActionResult ImageUploadForUpdate(int? UId)
        {
            
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
                         

                                var path = Path.Combine(Server.MapPath("~/Content/images/ProfilePic/"));
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
                                    UserImage upload = new UserImage();
                                    upload.Id = 0;
                                    string imagepath = "~/Content/images/ProfilePic/" + updatedFileName;
                                    upload.ImageURL = imagepath;
                                    upload.ImageDate = DateTime.Now;


                                    upload.Secu_UserId = UId??0;
                                    
                               

                                    db.UserImage.Add(upload);
                                    db.SaveChanges();

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

        public JsonResult DeleteImage(int Id)
        {

            bool flag = false;
            try
            {
                var deleteImage = db.UserImage.SingleOrDefault(x => x.Id == Id);
                //var deletedItem = db.ProjectSiteStatus.Where(x => x.ProjectSiteId == SiteId && x.ProjectId == ProjectId && x.PlanCode == PlanCode && x.SiteStatusDate == SiteStatusDate).ToList();
                string path = Path.Combine(Server.MapPath(deleteImage.ImageURL));

                System.IO.File.Delete(path);

                db.UserImage.Remove(deleteImage);
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


    }
}