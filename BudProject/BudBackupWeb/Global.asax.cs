using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using budbackup.CommonWeb.Util;
using System.Reflection;

namespace budbackup
{
    // メモ: IIS6 または IIS7 のクラシック モードの詳細については、
    // http://go.microsoft.com/?LinkId=9394801 を参照してください

    public class MvcApplication : System.Web.HttpApplication
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Default", // ルート名
                "{controller}/{action}/{id}", // パラメーター付きの URL
                new { controller = "Account", action = "Index", id = UrlParameter.Optional } // パラメーターの既定値
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            logger.Error(MessageUtil.GetExceptionMsg(exception, ""));
        }
    }
}