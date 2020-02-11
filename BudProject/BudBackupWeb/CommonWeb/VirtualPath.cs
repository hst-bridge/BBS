using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace budbackup.CommonWeb
{
    public class VirtualPath
    {
        private static string virPath = "";
        private VirtualPath() 
        {
            virPath = "../";
        }
        public static string getVirtualPath() 
        {
            return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + "/";
        }
    }
}