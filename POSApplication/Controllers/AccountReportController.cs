using Microsoft.Reporting.WebForms;
using POSApplication.Models;
using POSApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace POSApplication.Controllers
{
    [Authorize]
    public class AccountReportController : Controller
    {

        private POSDBContext db = new POSDBContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateAccountReport()
        {
            ViewBag.ExpenseType = new SelectList(db.ExpenseTypes.Distinct().OrderBy(x => x.EType).ToList(), "Id", "EType");

            return View();
        }
        [HttpPost]
        public ActionResult CreateAccountReport( DateTime? DateFrom, DateTime? DateTo, int? ProductCategoryId, int? ExpenseType)
        {
         
            if (DateFrom == null)
            {
                //DateFrom = db.GeneralAccounts
                DateFrom =(from acc in db.GeneralAccounts select(DbFunctions.TruncateTime(acc.Date))).Min();

            }
            if (DateTo == null)
            {

                DateTo = (from acc in db.GeneralAccounts select (DbFunctions.TruncateTime(acc.Date))).Max();


            }
            var data = db.GeneralAccounts.Where(x => x.Date >= DateFrom && x.Date <= DateTo &&x.ExpenseTypeId== ExpenseType || ExpenseType==null).ToList();
            List<Accounts> accounts = new List<Accounts>();

            foreach (var item in data)
            {
                accounts.Add(new Accounts
                {
                    Date = item.Date,
                    ExpenseType = item.ExpenseType.EType,
                    PayOver = item.PayOver,
                    CashPayment = item.CashPayment
                });

            }
        
            var companyInfo = db.CompanyInformations.FirstOrDefault();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.PageCountMode = new PageCountMode();
            reportViewer.LocalReport.ReportPath = Request.MapPath("~/Reports/AccountExpense.rdlc");

            List<ReportParameter> paraList = new List<ReportParameter>();
            paraList.Add(new ReportParameter("CompanyName", companyInfo.CompanyName));

            paraList.Add(new ReportParameter("Email", companyInfo.Email));
            paraList.Add(new ReportParameter("Phone", companyInfo.Phone));
            paraList.Add(new ReportParameter("Address", companyInfo.Address));
            paraList.Add(new ReportParameter("DateFrom", DateFrom.Value.ToShortDateString()));
            paraList.Add(new ReportParameter("DateTo",DateTo.Value.ToShortDateString()));
         
            reportViewer.LocalReport.SetParameters(paraList);

            ReportDataSource A = new ReportDataSource("DataSet1", accounts);
            reportViewer.LocalReport.DataSources.Add(A);
            reportViewer.ShowRefreshButton = false;


            ViewBag.ReportViewer = reportViewer;
            //return RedirectToAction("GetStockReport","StockReport");


            return View("~/Views/AccountReport/AccountReport.cshtml");


            //return View();
        }

    }
}