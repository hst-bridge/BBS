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
    public class GroupTransferController : BaseController
    {
        //
        // GET: /GroupTransfer/
        //転送先対象グループ　サービス
        private readonly IBackupServerGroupService groupSerivce = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
       //転送先対象グループ明細　サービス
        private readonly IBackupServerGroupDetailService groubDetailService = BLLFactory.ServiceAccess.CreateBackupServerGroupDetailService();
       //転送先対象　サービス
        private readonly IBackupServerService sService = BLLFactory.ServiceAccess.CreateBackupServer();
       //転送元　サービス
        private readonly IMonitorServerService mService = BLLFactory.ServiceAccess.CreateMonitorServerService();
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(GroupTransferController));
        /// <summary>
        /// //メインページ
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
                ViewData["modelList"] = groupSerivce.GetBackupServerGroupList();
            }
            catch (Exception ex)
            {
                ViewData["modelList"] = new List<BackupServerGroup>();
                logger.Error(ex.Message);
            }
            return View();
        }
        /// <summary>
        /// 新規ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Insert()
        {
            try
            {
                ViewData["msList"] = mService.GetMonitorServerList();
            }
            catch (Exception ex)
            {
                ViewData["msList"] = new List<MonitorBackupServer>();
                logger.Error(ex.Message);
            }
            return View();
        }
        /// <summary>
        /// 編集ページ
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                ViewData["model"] = groupSerivce.GetBackupServerGroupById(id);
                ViewData["msList"] = mService.GetMonitorServerList();
            }
            catch (Exception ex)
            {
                ViewData["model"] = new BackupServerGroup();
                ViewData["msList"] = new List<MonitorBackupServer>();
                logger.Error(ex.Message);
            }
            return View();
        }
        /// <summary>
        /// 明細設定ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult DetailEdit(int id)
        {
            try
            {
                ViewData["serverList"] = sService.GetBackupServerList();
                ViewData["groupList"] = groupSerivce.GetBackupServerGroupList();
                ViewData["selectList"] = sService.GetGroupBackupServerList(id.ToString());
                ViewData["unSelectList"] = sService.GetPartBackupServerList(id.ToString());
                ViewData["groupId"] = id.ToString();
            }
            catch (Exception ex)
            {
                ViewData["serverList"] = new List<BackupServer>();
                ViewData["groupList"] = new List<BackupServerGroup>();
                ViewData["selectList"] = new List<BackupServer>();
                ViewData["unSelectList"] = new List<BackupServer>();
                ViewData["groupId"] = "";
                logger.Error(ex.Message);
            }
            return View();
        }
        /// <summary>
        /// 追加レコード メソッド
        /// </summary>
        /// <param name="model">エンティティ</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(BackupServerGroup model)
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
                    string dt = CommonUtil.DateTimeNowToString();
                    model.creater = Session["LoginId"].ToString();
                    model.createDate = dt;
                    model.updater = Session["LoginId"].ToString();
                    model.updateDate = dt;
                    model.deleteFlg = 0;
                    result = groupSerivce.InsertBackupServerGroup(model);
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
        /// 更新レコード メソッド
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Update(BackupServerGroup model)
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
                    model.updateDate = CommonUtil.DateTimeNowToString();
                    model.updater = Session["LoginId"].ToString();
                    if (model.memo == null)
                    {
                        model.memo = string.Empty;
                    }
                    result = groupSerivce.UpdateBackupServerGroup(model);
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
        /// 削除レコード メソッド
        /// </summary>
        /// <param name="id">id</param>
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
                    result = groupSerivce.DeleteBackupServerGroup(id, Session["LoginId"].ToString());
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
        public ActionResult AddDetail(int groupId)
        {
            int result = -1;
            if (Session["LoginId"] == null)
            {
                result = -99;
            }
            else
            {
                if (Request.Form != null)
                {
                    string dt = CommonUtil.DateTimeNowToString();
                    try
                    {
                        for (int i = 0; i < Request.Form.Count; i++)
                        {
                            BackupServerGroupDetail model = new BackupServerGroupDetail();
                            model.backupServerGroupId = groupId;
                            model.backupServerId = Convert.ToInt16(Request.Form[Request.Form.Keys[i]].ToString());
                            model.deleteFlg = 0;
                            model.creater = Session["LoginId"].ToString();
                            model.createDate = dt;
                            model.updater = Session["LoginId"].ToString();
                            model.updateDate = dt;
                            result = groubDetailService.InsertBackupServerGroupDetail(model);
                        }
                    }
                    catch(Exception ex)
                    {
                        result = -10;
                        logger.Error(ex.Message);
                    }
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }
        public ActionResult DeleteDetail(int groupId)
        {
            int result = -1;
            if (Session["LoginId"] == null)
            {
                result = 99;
            }
            else
            {
                try
                {
                    if (Request.Form != null)
                    {
                        string dt = CommonUtil.DateTimeNowToString();
                        for (int i = 0; i < Request.Form.Count; i++)
                        {
                            //BackupServerGroupDetail model = new BackupServerGroupDetail();
                            //model.backupServerGroupId = groupId;
                            //model.backupServerId = Convert.ToInt16(Request.Form[Request.Form.Keys[i]].ToString());
                            //model.deleteFlg = 1;
                            //model.deleter = Session["LoginId"].ToString();
                            //model.deleteDate = dt;
                            int backupServerId = Convert.ToInt16(Request.Form[Request.Form.Keys[i]].ToString());
                            result = groubDetailService.DeleteBackupServerGroupDetail(backupServerId, groupId, Session["LoginId"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = -10;
                    logger.Error(ex.Message);
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }
        public ActionResult choseGroup(int id)
        {
            return null;
        }
        /// <summary>
        /// 転送先グループ対象名が存在するかをチェックする。
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public ActionResult CheckNameIsExists(string groupName)
        {
            int result = 0;
            try
            {
                IList<BackupServerGroup> groupList = groupSerivce.GetBackupServerGroupByName(groupName);
                result = groupList.Count;
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
    }
}
