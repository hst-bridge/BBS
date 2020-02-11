using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Common;
using IBLL;
using Model;

namespace budbackup.Controllers
{
    public class TransferController : BaseController
    {
        //
        // GET: /Transfer/
        private readonly IBackupServerService bkService = BLLFactory.ServiceAccess.CreateBackupServer();

        //転送元　サービス
        private readonly IMonitorServerService mService = BLLFactory.ServiceAccess.CreateMonitorServerService();

        //転送先対象グループ　サービス
        private readonly IBackupServerGroupService groupSerivce = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
        //転送先対象グループ明細　サービス
        private readonly IBackupServerGroupDetailService groubDetailService = BLLFactory.ServiceAccess.CreateBackupServerGroupDetailService();

        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(TransferController));
        /// <summary>
        /// 転送先・メインページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //session値チェック
            if (Session["LoginId"] == null)
            {
                return RedirectToAction("Account", "Account/LogOn", new { url = Request.Url });
            }
            try
            {
                ViewData["bkList"] = bkService.GetBackupServerList();
            }
            catch (Exception ex)
            {
                ViewData["bkList"] = new List<BackupServer>();
                logger.Error(ex.Message);
            }
            return View();
        }
        /// <summary>
        /// 転送先・編集ページ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                ViewData["bkserver"] = bkService.GetBackupServerById(id);
                ViewData["msList"] = mService.GetMonitorServerList();
                ViewData["model"] = groupSerivce.GetBackupServerGroupByBackupServerID(id);
            }
            catch (Exception ex)
            {
                ViewData["bkserver"] = new BackupServer();
                ViewData["msList"] = new MonitorServer();
                ViewData["model"] = new BackupServerGroup();
                logger.Error(ex.Message);
            }
            return View();
        }
        /// <summary>
        /// 転送先・新規ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Insert()
        {
            try
            {
                ViewData["msList"] = mService.GetMonitorServerList();
                ViewData["IP_StartFolder"] = CommonUtil.GetTransfer_IP_StartFolder();
            }
            catch (Exception ex)
            {
                ViewData["msList"] = new List<MonitorBackupServer>();
                logger.Error(ex.Message);
            }
            return View();
        }
        /// <summary>
        /// データ保存
        /// </summary>
        /// <param name="bkModel"></param>
        /// <returns></returns>
       [HttpPost]
        public ActionResult Add(BackupServerGroup model, BackupServer bkModel)
        {
            int result = -1;
            //session获取值
           if (Session["LoginId"] == null)
           {
               result = -99;
           }
           else
           {
               try
               {
                   string dt = CommonUtil.DateTimeNowToString();
                   string loginId = Session["LoginId"].ToString();

                   model.backupServerGroupName = bkModel.backupServerName;
                   model.creater = loginId;
                   model.createDate = dt;
                   model.updater = loginId;
                   model.updateDate = dt;
                   model.deleteFlg = 0;
                   result = groupSerivce.InsertBackupServerGroup(model);

                   bkModel.creater = loginId;
                   bkModel.createDate = dt;
                   bkModel.updater = loginId;
                   bkModel.updateDate = dt;
                   bkModel.deleteFlg = 0;
                   result = bkService.InsertBackupServer(bkModel);

                   //add relation detail
                   IList<BackupServerGroup> groupList = groupSerivce.GetBackupServerGroupByName(model.backupServerGroupName);
                   IList<BackupServer> bkList = bkService.GetBackupServerListByName(bkModel.backupServerName);
                   if (groupList.Count > 0 && bkList.Count > 0)
                   {
                       BackupServerGroupDetail modelDetail = new BackupServerGroupDetail();
                       modelDetail.backupServerGroupId = Convert.ToInt32(groupList[0].id);
                       modelDetail.backupServerId = Convert.ToInt32(bkList[0].id);
                       modelDetail.deleteFlg = 0;
                       modelDetail.creater = loginId;
                       modelDetail.createDate = dt;
                       modelDetail.updater = loginId;
                       modelDetail.updateDate = dt;
                       result = groubDetailService.InsertBackupServerGroupDetail(modelDetail);
                   }
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
        /// <param name="bkModel"></param>
        /// <returns></returns>
        public ActionResult Update(BackupServerGroup model, BackupServer bkModel)
        {
            int result = -1;
            //session获取值
            if (Session["LoginId"] == null)
            {
                result = -99;
            }
            else
            {
                try
                {
                    model.id = Request.Form["group_id"];
                    model.updater = bkModel.updater = Session["LoginId"].ToString();
                    model.updateDate = bkModel.updateDate = CommonUtil.DateTimeNowToString();
                    if (model.memo == null)
                    {
                        model.memo = string.Empty;
                    }
                    if (bkModel.memo == null)
                    {
                        bkModel.memo = string.Empty;
                    }
                    if (bkModel.ssbpath == null)
                    {
                        bkModel.ssbpath = string.Empty;
                    }
                    result = groupSerivce.UpdateBackupServerGroup(model);
                    result = bkService.UpdateBackupServer(bkModel);
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
                    string loginId = Session["LoginId"].ToString();

                    result = bkService.DeleteBackupServer(id, loginId);

                    BackupServerGroup backupServerGroup = groupSerivce.GetBackupServerGroupByBackupServerID(id);
                    result = groupSerivce.DeleteBackupServerGroup(Convert.ToInt32(backupServerGroup.id), loginId);
                    //delete relation detail
                    result = groubDetailService.DeleteBackupServerGroupDetail(id, Convert.ToInt32(backupServerGroup.id), loginId);
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
        /// <param name="bkModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NetworkTest(BackupServer model)
        {
            int Result = 0;
            string ipMessage = model.backupServerIP;
            string stfileMessage = model.startFile;
            //string monitorDrive = "AA";
            string accountMessage = model.account;
            string passMessage = model.password;
            //string serverFolderPath = @"\\" + ipMessage + @"\" + stfileMessage.TrimStart('\\').Replace('/', '\\');
            //string localPath = monitorDrive + ":";
            try
            {
                //int status = Common.NetworkConnection.Connect(serverFolderPath, localPath, accountMessage, passMessage);
                //if (status == (int)Common.ERROR_ID.ERROR_SUCCESS)
                //{
                //    Result = 1;
                //}
                //else
                //{
                //    Result = -1;
                //}
                string ip = @"\\" + ipMessage;
                NetWorkFileShare1 netWorkFileShare = new NetWorkFileShare1();
                if (netWorkFileShare.ConnectState(ip, accountMessage.Trim(), passMessage.Trim()))
                {
                    Result = 1;
                }
                else
                {
                    Result = -1;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("1312") > -1)
                {
                    Result = 1;
                }
                else
                {
                    Result = -10; 
                }
                logger.Error(ex.Message);
            }
            Response.Write(Result);
            Response.End();
            return null;
        }
        /// <summary>
        /// 転送先対象名が存在するかをチェックする。
        /// For insert
        /// </summary>
        /// <param name="bkName"></param>
        /// <returns></returns>
        public ActionResult CheckNameIsExists(string bkName)
        {
            int result = 0;
            try
            {
                IList<BackupServer> bkList = bkService.GetBackupServerListByName(bkName);
                result = bkList.Count;
            }
            catch(Exception ex)
            {
                result = -10;
                logger.Error(ex.Message);
            }
            Response.Write(result);
            Response.End();
            return null;
        }

        /// <summary>
        /// 転送先対象名が存在するかをチェックする。
        /// For edit
        /// </summary>
        /// 2014-06-24 wjd add
        /// <param name="id"></param>
        /// <param name="bkName">転送先対象名</param>
        /// <returns></returns>
        public ActionResult CheckNameIsExistsByIdName(int id, string bkName)
        {
            int result = 0;
            try
            {
                IList<BackupServer> bkList = bkService.GetBackupServerListByNameButId(id, bkName);
                result = bkList.Count;
            }
            catch (Exception ex)
            {
                result = -10;
                logger.Error(ex.Message);
            }
            Response.Write(result);
            Response.End();
            return null;
        }

        /// <summary>
        /// IPアドレスと開始フォル唯一。
        /// </summary>
        /// 2014-06-24 wjd add
        /// <param name="id"></param>
        /// <param name="bkIP"></param>
        /// <param name="startFolder"></param>
        /// <returns></returns>
        public ActionResult CheckIPAndStartfolder(int id, string bkIP, string startFolder)
        {
            int result = 0;
            try
            {
                IList<BackupServer> bkList = bkService.GetBackupServerListButId(id, bkIP, startFolder);
                result = bkList.Count;
            }
            catch (Exception ex)
            {
                result = -10;
                logger.Error(ex.Message);
            }
            Response.Write(result);
            Response.End();
            return null;
        }

        /// <summary>
        /// Check Deleted Backup Server
        /// </summary>
        /// 2014-06-30 wjd add
        /// <param name="bkIP"></param>
        /// <param name="startFolder"></param>
        /// <returns></returns>
        public ActionResult CheckDeletedBackupServer(string bkIP, string startFolder)
        {
            int result = 0;
            try
            {
                IList<BackupServer> bkList = bkService.GetDeletedBackupServerList(bkIP, startFolder);
                result = bkList.Count;
            }
            catch (Exception ex)
            {
                result = -10;
                logger.Error(ex.Message);
            }
            Response.Write(result);
            Response.End();
            return null;
        }
    }
}
