using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using budbackup.CommonWeb;
using Model;
using IBLL;
using log4net;

namespace budbackup.Controllers
{

    public class AccountController : BaseController
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(AccountController));
        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            Session["LoginId"] = null;
            Session.Clear();
            Session.Abandon();
            return View();
        }

        public ActionResult Index()
        {
            object loginId = Session["LoginId"];
            if (loginId != null)
            {
                return Redirect("/ObjectSpy/index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserInfo model)
        {
            string result = "";
            IUserInfoService userService = BLLFactory.ServiceAccess.CreateUserInfoService();
            try
            {
                if (!"admin".Equals(model.loginID))
                {

                    if ("mac".Equals(model.loginID) && "macmac".Equals(model.password))
                    {
                        //pass the validation, save the user info
                        Session["Usr"] = model;
                        Session["LoginId"] = model.loginID;
                        result = model.loginID;
                        //CommonUtil.LoginId = model.loginID;
                        logger.Info(model.loginID + " 登録成功。");

                    }

                    if ("soumu".Equals(model.loginID) && "kanrika".Equals(model.password))
                    {
                        //pass the validation, save the user info
                        Session["Usr"] = model;
                        Session["LoginId"] = model.loginID;
                        result = model.loginID;
                        //CommonUtil.LoginId = model.loginID;
                        logger.Info(model.loginID + " 登録成功。");

                    }
                }
                else
                {
                    UserInfo user = userService.userExist(model);
                    if (user != null && user.loginID != null)
                    {
                        //pass the validation, save the user info
                        Session["Usr"] = user;
                        Session["LoginId"] = user.loginID;
                        result = user.loginID;
                       // CommonUtil.LoginId = user.loginID;
                        logger.Info(user.loginID + " 登録成功。");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            Response.Write(result);
            Response.End();
            return null;
        }
    }
        
}
