using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Peech.Controllers
{
    public class CommonController : Controller
    {
        public CommonController()
        {
            
        }
        //
        // GET: /Common/

        public ActionResult CommonPaging()
        {
            return PartialView();
        }

        
    }
}
