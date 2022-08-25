using POSApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace POSApplication.Helpers
{
    public class ProductHelper
    {
        public static POSDBContext db = new POSDBContext();
        public static bool  UpdateProductPrice(List<PurchaseInvoiceDet> PurchaseInvoicedDetails)
        {
            foreach (var item in PurchaseInvoicedDetails)
            {
                Product products = new Product();

                var data = db.Products.Where(x => x.ProductCategoryId == item.ProductCategoryId && x.Id == item.ProductId).FirstOrDefault();
                //if (item.UpdatedPrice != null)
                //{
                //    //data.UpdatedPrice = item.UpdatedPrice;

                //    db.Entry(data).State = EntityState.Modified;
                //    db.SaveChanges();

                //}
                

            }
            return true;
        }

    }
}