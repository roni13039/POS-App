using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Common;
using System.Linq;
using System.Web;
using POSApplication.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace POSApplication.Models
{
    public class POSDBContext : DbContext
    {
        public POSDBContext() : base("POSDB")
        {

        }

        public POSDBContext (DbConnection con, bool contextOwnsConnection) : base(con, contextOwnsConnection)
        {

        }
        public virtual DbSet<UDC_Barcode> UDC_Barcode { get; set; }
        public virtual DbSet<OrderDet> OrderDets { get; set; }
        public virtual DbSet<OrderMas> OrderMas { get; set; }
        //----------------Parameter DBSet----------------//
    
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }
        public virtual DbSet<CompanyInformation> CompanyInformations { get; set; }
        public virtual DbSet<GeneralAccount> GeneralAccounts { get; set; }
        public virtual DbSet<SliderImage> SliderImages { get; set; }   //Sabid(11.11.2020)
        public virtual DbSet<ClientsFeedback> ClientsFeedbacks { get; set; } //Sabid(12.04.2020)
        public virtual DbSet<Photo> Photos { get; set; } //Sabid(12.04.2020)
        public virtual DbSet<News> News { get; set; } //Sabid(12.08.2020)
        public virtual DbSet<CareerCircular> CareerCirculars { get; set; } //Sabid(12.09.2020)

        //----------------End Parameter DBSet------------//
        //--------- Start Information Db Set --- 30/06/30--Talukder---//

        public virtual DbSet<PurchaseInvoiceDet> PurchaseInvoiceDets { get; set; }
        public virtual DbSet<PurchaseInvoiceMa> PurchaseInvoiceMas { get; set; }
        public virtual DbSet<SalesInvoiceDet> SalesInvoiceDets { get; set; }
        public virtual DbSet<SalesInvoiceMas> SalesInvoiceMas { get; set; }
        //===============----End==================//
        //=====================================================================================
        //                             Security                                           |
        public DbSet<Secu_User> Secu_User { get; set; }
        public DbSet<Secu_Role> Secu_Role { get; set; }
        public DbSet<UserImage> UserImage { get; set; }
        public object Entities { get; internal set; }
        //==============================End security =======================================================
        public virtual ObjectResult<sp_getAllStock_Result> sp_getAllStock(Nullable<System.DateTime> datefrom, Nullable<System.DateTime> dateTo, Nullable<int> productId, Nullable<int> productCategoryId)
        {
            var datefromParameter = datefrom.HasValue ?
                new SqlParameter("Datefrom", datefrom) :
                new SqlParameter("Datefrom", typeof(System.DateTime));

            var dateToParameter = dateTo.HasValue ?
                new SqlParameter("DateTo", dateTo) :
                new SqlParameter("DateTo", typeof(System.DateTime));

            var productIdParameter = productId.HasValue ?
                new SqlParameter("ProductId", productId) :
                new SqlParameter("ProductId", typeof(int));

            var productCategoryIdParameter = productCategoryId.HasValue ?
                new SqlParameter("ProductCategoryId", productCategoryId) :
                new SqlParameter("ProductCategoryId", typeof(int));

           // return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_getAllStock_Result>("sp_getAllStock", datefromParameter, dateToParameter, productIdParameter, productCategoryIdParameter);

         //   return this.Database.SqlQuery<sp_getAllStock_Result>("sp_getAllStock", datefromParameter, dateToParameter, productIdParameter, productCategoryIdParameter);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_getAllStock_Result>("sp_getAllStock", datefromParameter, dateToParameter, productIdParameter, productCategoryIdParameter);
         //   IEnumerable<string> Query = Context.Database.SqlQuery<string>("SELECT * FROM " + Table + " WHERE " + ID + " LIKE '" + SearchTerm + "'");
        }


        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {   

                         
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductCategory>()
                .Property(e => e.CategoryName)
                .IsUnicode(false);


            modelBuilder.Entity<OrderDet>()
                     .Property(e => e.Price)
                     .HasPrecision(12, 2);

            modelBuilder.Entity<OrderMas>()
                .Property(e => e.CustomerName)
                .IsUnicode(false);

            modelBuilder.Entity<OrderMas>()
                .Property(e => e.Town)
                .IsUnicode(false);

            modelBuilder.Entity<OrderMas>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<OrderMas>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<OrderMas>()
                .HasMany(e => e.OrderDets)
                .WithOptional(e => e.OrderMas)
                .HasForeignKey(e => e.OrderMasId);

            modelBuilder.Entity<CompanyInformation>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyInformation>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyInformation>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyInformation>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyInformation>()
                .Property(e => e.Description)
                .IsUnicode(false);



            modelBuilder.Entity<Customer>()
                .Property(e => e.CustomerName)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Hint)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.OpeningBalance)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Customer>()
                .Property(e => e.TotalPurchase)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Customer>()
                .Property(e => e.TotalPaid)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Customer>()
                .Property(e => e.DueBalance)
                .HasPrecision(12, 2);



            modelBuilder.Entity<Product>()
                .Property(e => e.ProductCode)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.ProductName)
                .IsUnicode(false);




            modelBuilder.Entity<ExpenseType>()
                .Property(e => e.EType)
                .IsUnicode(false);

            modelBuilder.Entity<ExpenseType>()
                .Property(e => e.PayOver)
                .IsUnicode(false);



            modelBuilder.Entity<GeneralAccount>()
                .Property(e => e.PayOver)
                .IsUnicode(false);

            modelBuilder.Entity<GeneralAccount>()
                .Property(e => e.CashPayment)
                .HasPrecision(12, 2);

            modelBuilder.Entity<GeneralAccount>()
                .Property(e => e.PayTo)
                .IsUnicode(false);



            modelBuilder.Entity<PurchaseInvoiceDet>()
                .Property(e => e.PurchasePrize)
                .HasPrecision(12, 2);

            modelBuilder.Entity<PurchaseInvoiceMa>()
                .HasMany(e => e.PurchaseInvoiceDets)
                .WithOptional(e => e.PurchaseInvoiceMa)
                .HasForeignKey(e => e.PurchaseInvoiceMasId)
                .WillCascadeOnDelete();




            modelBuilder.Entity<SalesInvoiceMas>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<SalesInvoiceMas>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<SalesInvoiceMas>()
                .Property(e => e.Description)
                .IsUnicode(false);



            modelBuilder.Entity<SalesInvoiceDet>()
                .Property(e => e.PurchasePrize)
                .HasPrecision(12, 2);

            modelBuilder.Entity<SalesInvoiceDet>()
                .Property(e => e.SalesPrize)
                .HasPrecision(12, 2);

            modelBuilder.Entity<SalesInvoiceDet>()
                .Property(e => e.Amount)
                .HasPrecision(12, 2);

            modelBuilder.Entity<SliderImage>()
                .Property(e => e.Price)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ClientsFeedback>()
                .Property(e => e.ClientsName)
                .IsUnicode(false);

            modelBuilder.Entity<ClientsFeedback>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Photo>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<News>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<News>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<CareerCircular>()
                .Property(e => e.JobTitle)
                .IsUnicode(false);

        }

       
    }
}