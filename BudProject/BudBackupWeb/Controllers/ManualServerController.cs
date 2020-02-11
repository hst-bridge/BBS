using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using IBLL;
using budbackup.CommonWeb;
using System.IO;
using log4net;

namespace budbackup.Controllers
{
    public class ManualServerController : BaseController
    {
        //
        // GET: /ObjectSpy/
        private readonly IManualBackupServerService msSerivice = BLLFactory.ServiceAccess.CreateManualBackupServerService();
        private readonly ILog logger = LogManager.GetLogger(typeof(ObjectSpyController));
        public ActionResult Index()
        {
            if (Session["LoginId"] == null)
            {
                if (CommonUtil.LoginId != string.Empty)
                {
                    Session["LoginId"] = CommonUtil.LoginId;
                    CommonUtil.LoginId = string.Empty;
                }
                else
                {
                    return RedirectToAction("Account", "Account/LogOn", new { url = Request.Url });
                }
            }
            try
            {
                ViewData["bkList"] = msSerivice.GetManualBackupServerList();
            }
            catch(Exception ex)
            {
                ViewData["bkList"] = new List<ManualBackupServer>();
                logger.Error(ex.Message);
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            try
            {
                ViewData["bkList"] = msSerivice.GetManualBackupServerById(id);
            }
            catch(Exception ex)
            {
                ViewData["bkList"] = new ManualBackupServer();
                logger.Error(ex.Message);
            }
            return View();
        }
        public ActionResult Insert()
        {
            return View();
        }
        /// <summary>
        /// データ保存
        /// </summary>
        /// <param name="msModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(ManualBackupServer msModel)
        {
            int result = -1;
            //session获取值
            if (Session["LoginId"] == null)
            {
                result = -99;
            }
            else
            {
                //保存
                try
                {
                    string dt = Common.CommonUtil.DateTimeNowToString();
                    msModel.DBServerIP = msModel.serverIP;
                    msModel.drive = msModel.drive + ":";
                    msModel.creater = Session["LoginId"].ToString();
                    msModel.createDate = dt;
                    msModel.deleteFlg = 0;
                    msModel.updater = Session["LoginId"].ToString();
                    msModel.updateDate = dt;
                    result = msSerivice.InsertManualBackupServer(msModel);
                }
                catch(Exception ex)
                {
                    result = -10;
                    logger.Error(ex.Message);
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }
        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="msModel"></param>
        /// <returns></returns>
        public ActionResult Update(ManualBackupServer msModel)
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
                    msModel.DBServerIP = msModel.serverIP;
                    msModel.updater = Session["LoginId"].ToString();
                    msModel.updateDate = Common.CommonUtil.DateTimeNowToString();
                    result = msSerivice.UpdateManualBackupServer(msModel);
                }
                catch(Exception ex)
                {
                    result = -10;
                    logger.Error(ex.Message);
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }
        /// <summary>
        /// データ削除
        /// </summary>
        /// <param name="id"></param>
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
                    ManualBackupServer model = msSerivice.GetManualBackupServerById(Convert.ToInt32(id));
                    if (model.id != "")
                    {
                        Common.NetworkConnection.Disconnect(model.drive);
                    }
                    result = msSerivice.DeleteManualBackupServer(id, Session["LoginId"].ToString());
                }
                catch(Exception ex)
                {
                    result = -10;
                    logger.Error(ex.Message);
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }
        /// <summary>
        /// 接続テスト
        /// </summary>
        /// <param name="model"></param>
        /// <returns>result 1:接続成功；-1：接続失敗</returns>
        [HttpPost]
        public ActionResult NetworkTest(ManualBackupServer model)
        {
            int Result = 0;
            string ipMessage = model.serverIP;
            string stfileMessage = model.startFile;
            string monitorDriveP = model.drive;
            string accountMessage = model.account;
            string passMessage = model.password;
            string localPath = monitorDriveP + ":";
            try
            {
                if (System.IO.Directory.Exists(localPath))
                {
                    Result = 1;
                }
                else
                {
                    int status = Common.NetworkConnection.Connect(@"\\" + model.serverIP + @"\" + model.startFile.TrimStart('\\').Replace('/', '\\'), localPath, model.account, model.password);
                    if (status == (int)Common.ERROR_ID.ERROR_SUCCESS)
                    {
                        Result = 1;
                    }
                    else
                    {
                        Result = -1;
                    }
                }
                logger.Info("接続状態:" + Result.ToString());
            }
            catch(Exception ex)
            {
                Result = -10;
                logger.Error(ex.Message);
            }
            Response.Write(Result);
            Response.End();
            return null;
        }
        /// <summary>
        /// 接続テスト
        /// </summary>
        /// <param name="model"></param>
        /// <returns>result 1:接続成功；-1：接続失敗</returns>
        [HttpPost]
        public ActionResult NetworkConnect(string id)
        {
            int Result = 0;
            try
            {
                ManualBackupServer model = msSerivice.GetManualBackupServerById(Convert.ToInt32(id));
                if (model.id != "")
                {
                    string ipMessage = model.serverIP;
                    string stfileMessage = model.startFile;
                    string monitorDriveP = model.drive;
                    string accountMessage = model.account;
                    string passMessage = model.password;
                    string localPath = monitorDriveP;
                    if (System.IO.Directory.Exists(monitorDriveP))
                    {
                        Result = 1;
                    }
                    else
                    {
                        int status = Common.NetworkConnection.Connect(@"\\" + model.serverIP + @"\" + model.startFile.TrimStart('\\').Replace('/', '\\'), localPath, model.account, model.password);
                        if (status == (int)Common.ERROR_ID.ERROR_SUCCESS)
                        {
                            Result = 1;
                        }
                        else
                        {
                            Result = -1;
                        }
                    }
                }
                logger.Info("接続状態:" + Result.ToString());
            }
            catch (Exception ex)
            {
                Result = -10;
                logger.Error(ex.Message);
            }
            Response.Write(Result);
            Response.End();
            return null;
        }
    }
}
