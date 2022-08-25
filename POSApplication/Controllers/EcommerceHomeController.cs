using POSApplication.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POSApplication.Controllers
{
    public class EcommerceHomeController : Controller
    {
        private POSDBContext db = new POSDBContext();
        public ActionResult Index()
        {
            var companydata = db.CompanyInformations.FirstOrDefault();

            Session["CompanyName"] = companydata.CompanyName;
            Session["CompanyAddress"] = companydata.Address;
            Session["CompanyEmail"] = companydata.Email;
            Session["CompanyMobileNo"] = companydata.Phone;

            // Pass Slider Image to InDex View

            var SliderData = db.SliderImages.ToList();
            
            Session["ProductCategory"]= db.ProductCategories.ToList();






            // get all product categoryId
            var data = db.ProductCategories.ToList();

            dynamic mymodel = new ExpandoObject();
            // pass multiple list to a single view

            mymodel.SliderData = SliderData;

            mymodel.categoryData = data;
            
            return View(mymodel);
        }

        // GET: EcommerceHome/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EcommerceHome/Create
        public ActionResult Create()
        {
    


            return View();
        }

        // POST: EcommerceHome/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EcommerceHome/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EcommerceHome/Edit/5
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

        // GET: EcommerceHome/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EcommerceHome/Delete/5
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
        
        public ActionResult About()
        {
            return View();
        }

        public ActionResult News()
        {
            var data = db.News.ToList();
            return View(data);
        }

        public ActionResult Contact()
        {

            return View();

        }

        public ActionResult Photos()
        {

            dynamic mymodel = new ExpandoObject();
            // pass multiple list to a single view
            var PhotosData = db.Photos.ToList();
            mymodel.PhotosData = PhotosData;


            return View(mymodel);
        }

        public ActionResult Clients()
        {

            var data = db.ClientsFeedbacks.ToList();
            //ViewBag.CategoryName = db.ProductCategories.Where(x => x.Id == Id).Select(x => x.CategoryName).FirstOrDefault();
            // pass product category Name 


            return View(data);
            //return View();
        }

        public ActionResult Career()
        {

            var data = db.CareerCirculars.ToList();
            //ViewBag.CategoryName = db.ProductCategories.Where(x => x.Id == Id).Select(x => x.CategoryName).FirstOrDefault();
            // pass product category Name 


            return View(data);
            //return View();
        }

        public ActionResult CareerApply()
        {

            var data = db.CareerCirculars.ToList();
            //ViewBag.CategoryName = db.ProductCategories.Where(x => x.Id == Id).Select(x => x.CategoryName).FirstOrDefault();
            // pass product category Name 


            return View(data);
            //return View();
        }
    }
}
