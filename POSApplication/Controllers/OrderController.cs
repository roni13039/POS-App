using POSApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POSApplication.Controllers
{
    public class OrderController : Controller
    {
        private POSDBContext db = new POSDBContext();
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        public ActionResult CreateOrder(List<OrderDet> OrderDetails,OrderMas master)
        {
            try
            {
                master.AddedOn = DateTime.Now;
                db.OrderMas.Add(master);
                db.SaveChanges();

                foreach (var item in OrderDetails)
                {
                    OrderDet det = new OrderDet();
                    det.OrderMasId = master.Id;
                    det.ProductId = item.ProductId;
                    det.Qunatity = item.Qunatity;
                    det.Price = item.Price;

                    db.OrderDets.Add(det);
                    db.SaveChanges();
                }


                return Json(true,JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Order/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
