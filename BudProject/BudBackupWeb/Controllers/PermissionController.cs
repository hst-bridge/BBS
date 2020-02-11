using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Common;
using IBLL;


namespace budbackup.Controllers
{
    public class PermissionController : BaseController
    {
        //
        // GET: /Permission/
        private readonly IUserInfoService UserInfoService = BLLFactory.ServiceAccess.CreateUserInfoService();
        /// <summary>
        /// Get all account information
        /// </summary>
        /// <param name="name">ユーザー名前</param>
        /// <param name="LoginID">ログインID</param>
        /// <param name="password">パスワード</param>
        /// <param name="mail">ユーザーメール</param>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewData["userList"] = UserInfoService.GetUserInfoList();
            return View();
        }
        /// <summary>
        /// Get a account information
        /// </summary>
        /// <param name="name">ユーザー名前</param>
        /// <param name="LoginID">ログインID</param>
        /// <param name="password">パスワード</param>
        /// <param name="mail">ユーザーメール</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ViewData["userList"] = UserInfoService.GetUserInfoById(id);
            return View();
        }
        /// <summary>
        /// Update a account information
        /// </summary>
        /// <param name="updater">更新者</param>
        /// <param name="updateDate">更新日時</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(UserInfo uinfo)
        {
            int result = -1;
            if (Session["LoginId"] == null)
            {
                result = -99;
            }
            else
            {
                try
                {
                    uinfo.updater = Session["LoginId"].ToString();
                    uinfo.updateDate = CommonUtil.DateTimeNowToString();
                    result = UserInfoService.UpdateUserInfo(uinfo);
                }
                catch
                {
                    result = -10;
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }

        /// <summary>
        /// Delete a account information
        /// </summary>
        /// <param name="deleteDate">削除者</param>
        /// <param name="deleter">削除日時</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            int result = -1;
            if (Session["LoginId"] == null)
            {
                result = -99;
            }
            else
            {
                try
                {
                    result = UserInfoService.DeleteUserInfo(id, Session["LoginId"].ToString());
                }
                catch
                {
                    result = -10;
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }

        public ActionResult Insert()
        {
            return View();
        }

        /// <summary>
        /// Add  a account information
        /// </summary>
        /// <param name="creater">登録者</param>
        /// <param name="createDate">登録日時</param>
        /// <param name="updater">更新者</param>
        /// <param name="updateDate">更新日時</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(UserInfo uinfo)
        {
            int result = -1;
            //Get session value
            if (Session["LoginId"] == null)
            {
                result = -99;
            }
            else
            {
                //Save
                try
                {
                    uinfo.creater = Session["LoginId"].ToString();
                    uinfo.createDate = CommonUtil.DateTimeNowToString();
                    uinfo.deleteFlg = 0;
                    uinfo.updater = Session["LoginId"].ToString();
                    uinfo.updateDate = CommonUtil.DateTimeNowToString();
                    result = UserInfoService.InsertUserInfo(uinfo);
                }
                catch
                {
                    result = -10;
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }

    }
}
