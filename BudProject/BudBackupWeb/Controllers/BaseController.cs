using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace budbackup.Controllers
{
    /// <summary>
    /// this controller is used to filter the action request 
    /// </summary>
    public class BaseController:Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (!Request.RawUrl.Contains("FileDownload/"))
            {
                Session["DownloadSearchResult"] = null;
                Session["zipPathHt"] = null;

            }

            //authority :the user mac can only can only download
            object loginId = Session["LoginId"];
            if (loginId != null)
            {
                string username = loginId as string;
                if (!"admin".Equals(username))
                {
                    string url = Request.RawUrl.ToLower();
                    if (!url.Contains("filedownload/") && !url.Contains("account/logon"))
                    {

                        filterContext.Result = Redirect("/FileDownload/index");
                    }
                }
                

            }
            else
            {
                string url = Request.RawUrl.ToLower();
                if (!url.Equals("/") && !url.Contains("home/") && !url.Contains("account/"))
                {

                    filterContext.Result = Redirect("/Account/index");
                }
            }
            
        }

      
    }
}