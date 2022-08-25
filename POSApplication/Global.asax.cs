using POSApplication.NotificationHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
namespace POSApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //notification
        string con = ConfigurationManager.ConnectionStrings["POSDB"].ConnectionString;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Notificaton Added Changes
            //here in Application Start we will start Sql Dependency  

            // SqlDependency.Start(con);

        }
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    CultureInfo cInfo = new CultureInfo("en-GB");
        //    //cInfo.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
        //    cInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        //    cInfo.DateTimeFormat.DateSeparator = "/";
        //    Thread.CurrentThread.CurrentCulture = cInfo;
        //    Thread.CurrentThread.CurrentUICulture = cInfo;
        //}


        // Notification
        protected void Session_Start(object src, EventArgs e)
        {

            //Notification

          //  NotificationComponent NC = new NotificationComponent();
            var currentTime = DateTime.Now;
            HttpContext.Current.Session["LastUpdated"] = currentTime;
          //  NC.RegisterNotification(currentTime);



            if (Context.Session != null)
            {
                if (Context.Session.IsNewSession)
                {
                    var sessionCookie = Context.Session["uid"];

                    if ((sessionCookie == null))
                    {
                        FormsAuthentication.SignOut();
                        //HttpContext.Current.Response.Redirect("~/Account/Login");
                        //Previous Code 
                        HttpContext.Current.Response.Redirect("~/Authentication/Login");
                        //PreesentCode

                        //HttpContext.Current.Response.Redirect("~/EcommerceHome/Index");
                    }
                    else
                    {

                    }
                }
            }
        }

        //  notification

        protected void Application_End()
        {
            //here we will stop Sql Dependency  
          //  SqlDependency.Stop(con);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
        }

    }
}
