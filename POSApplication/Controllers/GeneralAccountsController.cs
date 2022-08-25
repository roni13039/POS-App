using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using POSApplication.Models;

namespace POSApplication.Controllers
{
    [Authorize]
    public class GeneralAccountsController : Controller
    {
        private POSDBContext db = new POSDBContext();

        // GET: GeneralAccounts
        public ActionResult Index()
        {
            return View(db.GeneralAccounts.ToList());
        }

        // GET: GeneralAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralAccount generalAccount = db.GeneralAccounts.Find(id);
            if (generalAccount == null)
            {
                return HttpNotFound();
            }
            return View(generalAccount);
        }

        // GET: GeneralAccounts/Create
        public ActionResult Create()
        {
            ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes, "Id", "EType");
            return View();
        }

        // POST: GeneralAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Create(GeneralAccount generalAccount)
        {
           
                db.GeneralAccounts.Add(generalAccount);
                db.SaveChanges();

                
                ViewBag.Message = "Succeess";
                ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes, "Id", "EType");
                return View();

           

            
        }

        // GET: GeneralAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralAccount generalAccount = db.GeneralAccounts.Find(id);
            ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes, "Id", "EType", generalAccount.ExpenseTypeId);

            if (generalAccount == null)
            {
                return HttpNotFound();
            }
            return View(generalAccount);
        }

        // POST: GeneralAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,ExpenseTypeId,PayOver,CashPayment,Description")] GeneralAccount generalAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(generalAccount).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");

                ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes, "Id", "EType", generalAccount.ExpenseTypeId);
                ViewBag.Message = "Succeess";
                return View();
            }
            return View(generalAccount);
        }

        // GET: GeneralAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralAccount generalAccount = db.GeneralAccounts.Find(id);
            if (generalAccount == null)
            {
                return HttpNotFound();
            }
            return View(generalAccount);
        }

        // POST: GeneralAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GeneralAccount generalAccount = db.GeneralAccounts.Find(id);
            db.GeneralAccounts.Remove(generalAccount);
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

                        var Item = db.GeneralAccounts.SingleOrDefault(x => x.Id == Id);
                        db.GeneralAccounts.Remove(Item);
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



        public JsonResult GetPayOver(int expensetypeid)
        {
            var getpayover = (from et in db.ExpenseTypes
                              where (et.Id == expensetypeid)
                              select new
                              {
                                  expensetypeid = et.Id,
                                  payover = et.PayOver

                              }).FirstOrDefault();

            return Json(getpayover, JsonRequestBehavior.AllowGet);
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
