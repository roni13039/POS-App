using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POSApplication.Models;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace POSApplication.Controllers
{
    public class POSBaseController : Controller
    {
        private static POSDBContext db = new POSDBContext();

        public dynamic getProductBySerialInfo(string SerialNo)
        {
            var data = db.PurchaseInvoiceDets.Where(x => x.SerialNo.Trim().Equals(SerialNo.Trim()))
            .Select(x => new 
            {

                ProductCategoryId = x.ProductCategoryId,
                ProductCatagoryName = x.ProductCategory.CategoryName,
                SerialNo = x.SerialNo,
                PurchasePrize = x.PurchasePrize,
                SalesPrice = x.SalesPrice??0.00m,
                ProductId = x.ProductId,
                ProductName = x.Product.ProductName,
                Id=x.Id
                
            }).OrderByDescending(x=>x.Id).Take(1).ToList();

            return data;
        }
        [HttpPost]
        public JsonResult GetProductByCatagoryId(int? ProductCategoryId)
        {
            if (ProductCategoryId == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = db.Products.Where(x => x.ProductCategoryId == ProductCategoryId).Select(x => new
                {

                    Value = x.Id,
                    Text = x.ProductName

                }).OrderBy(x=>x.Text).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        public static List<sp_getAllStock_Result> GetStockByProduct(int? ProductCategoryId, DateTime? DateFrom, DateTime? DateTo, int? ProductId, int? SupplierId)
        {

            var ProductCategoryParaId = new SqlParameter("@ProductCategoryId", ProductCategoryId ?? SqlInt32.Null);
            var ProductParaId = new SqlParameter("@ProductId", ProductId ?? SqlInt32.Null);
            var DateFromPara = new SqlParameter("@DateFrom", DateFrom ?? SqlDateTime.Null);
            var DateTopara = new SqlParameter("@DateTo", DateTo ?? SqlDateTime.Null);
            var SupplierIdpara = new SqlParameter("@SupplierId", SupplierId ?? SqlInt32.Null);

            var result = db.Database.SqlQuery<sp_getAllStock_Result>("sp_getAllStock @Datefrom ,@DateTo, @ProductId, @ProductCategoryId ,@SupplierId",
                                                           DateFromPara, DateTopara, ProductParaId, ProductCategoryParaId, SupplierIdpara).ToList();

            return result;
        }

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
