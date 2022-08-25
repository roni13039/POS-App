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
    [Authorize]
    public class CompanyInformationsController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: CompanyInformations
        public ActionResult Index()
        {
            return View(db.CompanyInformations.ToList());
        }

        // GET: CompanyInformations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            if (companyInformation == null)
            {
                return HttpNotFound();
            }
            return View(companyInformation);
        }

        // GET: CompanyInformations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompanyInformations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyName,Address,Phone,Email,Description")] CompanyInformation companyInformation)
        {
            if (ModelState.IsValid)
            {
                db.CompanyInformations.Add(companyInformation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(companyInformation);
        }

        // GET: CompanyInformations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            if (companyInformation == null)
            {
                return HttpNotFound();
            }
            return View(companyInformation);
        }

        // POST: CompanyInformations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyName,Address,Phone,Email,Description")] CompanyInformation companyInformation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyInformation).State = EntityState.Modified;
                db.SaveChanges();

                //save Image For Comapany
                //saveCompanyImage();
                ViewBag.Message = "Succeess";
                return View();
            }
            return View(companyInformation);
        }

        //public bool saveCompanyImage(int id, HttpPostedFileBase file)
        //{
        //    bool isSavedSuccessfully = false;

        //    string fName = "";
        //    try
        //    {
        //            fName = file.FileName;
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                if (file.FileName.ToLower().EndsWith("jpg") || file.FileName.ToLower().EndsWith("png"))
        //                {

        //                    var path = Path.Combine(Server.MapPath("~/Content/Companyimages/"));
        //                    string pathString = System.IO.Path.Combine(path.ToString());

        //                    var newName = Path.GetFileName(file.FileName);
        //                    bool isExists = System.IO.Directory.Exists(pathString);

        //                    if (!isExists) System.IO.Directory.CreateDirectory(pathString);
        //                    {

        //                        var updatedFileName = id + "!" + newName;
  
        //                        var uploadpath = string.Format("{0}{1}", pathString, updatedFileName);

        //                        file.SaveAs(uploadpath);

        //                        //save in db
        //                        UserImage upload = new UserImage();
        //                        upload.Id = 0;
        //                        string imagepath = "~/Content/images/ProfilePic/" + updatedFileName;
        //                        upload.ImageURL = imagepath;
        //                        upload.ImageDate = DateTime.Now;


        //                        upload.Secu_UserId = UId;

        //                        db.UserImage.Add(upload);
        //                        db.SaveChanges();

        //                        isSavedSuccessfully = true;
        //                    }


        //                }
        //            }
                

        //    }
        //    catch (Exception ex)
        //    {
        //        isSavedSuccessfully = false;
        //    }

        //    return RedirectToAction("Index");



        //}






        // GET: CompanyInformations/Edit/5
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            if (companyInformation == null)
            {
                return HttpNotFound();
            }
            return View(companyInformation);
        }

        // POST: CompanyInformations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail([Bind(Include = "Id,CompanyName,Address,Phone,Email,Description")] CompanyInformation companyInformation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyInformation).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Edit/1");

                ViewBag.Message = "Succeess";
                return View();
            }
            return View(companyInformation);
        }

        // GET: CompanyInformations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            if (companyInformation == null)
            {
                return HttpNotFound();
            }
            return View(companyInformation);
        }

        // POST: CompanyInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            db.CompanyInformations.Remove(companyInformation);
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

                        var Item = db.CompanyInformations.SingleOrDefault(x => x.Id == Id);
                        db.CompanyInformations.Remove(Item);
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
