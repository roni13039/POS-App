using POSApplication.Models;
using POSApplication.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POSApplication.Controllers
{

    public class EccomerceProductController : Controller
    {
        private POSDBContext db = new POSDBContext();
        public ActionResult Index()
        {
            return View();
        }

        

        public PartialViewResult getProductByCategoryId(int Id)
        {
            var data = db.Products.Where(x => x.ProductCategoryId == Id).ToList();

            // pass product category Name 
            ViewBag.CategoryName = db.ProductCategories.Where(x => x.Id == Id).Select(x => x.CategoryName).FirstOrDefault();
                
            return PartialView("ProductListPartialView", data);
        }


        public ActionResult GetProductsFromDropdown(int Id)
        {
            var data = db.Products.Where(x => x.ProductCategoryId == Id).ToList();
            ViewBag.CategoryName = db.ProductCategories.Where(x => x.Id == Id).Select(x => x.CategoryName).FirstOrDefault();
            // pass product category Name 


            return View( data);
        }

        public ActionResult CartProduct(List<int> cartData)
        {
            List<VMCartProduct> productlist = new List<VMCartProduct>();
            Hashtable ht = new Hashtable();

            List<VMCartData> cart = new List<VMCartData>();
            var numberGroups = cartData.GroupBy(i => i);
            foreach (var grp in numberGroups)
            {
                var number = grp.Key;
                var total = grp.Count();
                cart.Add(new VMCartData { ID = number, TotalQuantity = total });
            }


            foreach (var item in cart)
            {

                var data = db.Products.Where(x => x.Id == item.ID).Select(x => new
                {
                    id = x.Id,
                    productName = x.ProductName,
                    imageurl = x.ImageURL,
                    //price = x.SalesPrice,
                    quantity = item.TotalQuantity


                }).FirstOrDefault();

                productlist.Add(new VMCartProduct { ProductId = data.id, ProductName = data.productName, ImageUrl = data.imageurl, Quantity = data.quantity, });


            }
            //var data = db.Products.Where(x => cartData.Contains(x.Id)).ToList();
            return PartialView("CartProduct", productlist);
            //return Json(data, JsonRequestBehavior.AllowGet);
        }

       
        
        // GET: EccomerceProduct/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EccomerceProduct/Create
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


    }
}
