using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Common;
using Model;
using IBLL;
using budbackup.CommonWeb;
using log4net;
using System.IO;

namespace budbackup.Controllers
{
    public class ObjectSpyController : BaseController
    {
        //
        // GET: /ObjectSpy/
        private readonly IMonitorServerService msSerivice = BLLFactory.ServiceAccess.CreateMonitorServerService();
        private readonly IMonitorServerFolderService msFolderService = BLLFactory.ServiceAccess.CreateMonitorServerFolderService();
        private readonly IMonitorFileListenService msFileListenService = BLLFactory.ServiceAccess.CreateMonitorFileListenService();
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
                ViewData["msDataList"] = msSerivice.GetMonitorServerList();
            }
            catch(Exception ex)
            {
                ViewData["msDataList"] = new List<MonitorServer>();
                logger.Error(ex.Message);
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            MonitorServer initMonitor = new MonitorServer();
            string startPath = "";
            try
            {
                initMonitor = msSerivice.GetMonitorServerById(id);
                startPath = Common.CommonUtil.GetLocalCopyPath();
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
            ViewData["msData"] = initMonitor;
            ViewData["StartPath"] = startPath;
            return View();
        }
        public ActionResult Insert()
        {
            ViewData["StartPath"] = Common.CommonUtil.GetLocalCopyPath();
            return View();
        }
        /// <summary>
        /// データ保存
        /// </summary>
        /// <param name="msModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(MonitorServer msModel)
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
                    //string strpath = msModel.monitorDrive + ":";
                    string strpath = "\\\\" + msModel.monitorServerIP.Trim() + "\\" + msModel.startFile.Trim().Trim('\\').Replace('/', '\\') +"\\";
                    //if (!Directory.Exists(strpath))
                    //{
                    //    result = -20;
                    //}
                    //else
                    //{
                        string dt = Common.CommonUtil.DateTimeNowToString();
                        msModel.monitorDrive = strpath.TrimEnd('\\');
                        msModel.monitorDriveP = msModel.monitorDriveP + ":";
                        msModel.creater = Session["LoginId"].ToString();
                        msModel.createDate = dt;
                        msModel.deleteFlg = 0;
                        msModel.updater = Session["LoginId"].ToString();
                        msModel.updateDate = dt;
                        msModel.synchronismFlg = 0;
                        result = msSerivice.InsertMonitorServer(msModel);
                        
                        //maxID
                        int intMonitorServerID = msSerivice.GetMaxId();
                        //if (Directory.Exists(strpath))
                        //{
                            MonitorServerFolder model = new MonitorServerFolder();
                            model.monitorServerID = intMonitorServerID;
                            model.monitorFileType = "99";
                            model.monitorFilePath = strpath.TrimEnd('\\');
                            model.monitorFileName = "";
                            model.createDate = dt;
                            model.updateDate = dt;
                            model.creater = Session["LoginId"].ToString();
                            model.updater = Session["LoginId"].ToString();
                            model.monitorServerID = intMonitorServerID;
                            model.monitorFileName = "";
                            model.monitorFlg = "0";
                            model.initFlg = "1";
                            model.deleteFlg = 0;
                            model.synchronismFlg = 0;
                            result = msFolderService.InsertMonitorServerFolder(model);
                            result = InsertFolder(strpath, dt, intMonitorServerID);
                        //}
                    //}
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
        public ActionResult Update(MonitorServer msModel)
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
                    msModel.id = Request.Form["msId"];
                    string dt = Common.CommonUtil.DateTimeNowToString();
                    MonitorServer monitorServer = msSerivice.GetMonitorServerById(Convert.ToInt32(msModel.id));
                    msModel.updater = Session["LoginId"].ToString();
                    msModel.updateDate = dt;
                    if (msModel.memo == null)
                    {
                        msModel.memo = string.Empty;
                    }
                    if (msModel.monitorDriveP == "" || msModel.monitorDriveP == null) 
                    {
                        for (char i = 'A'; i <= 'Z'; i++) 
                        {
                            if (!Directory.Exists(i + ":")) 
                            {
                                msModel.monitorDriveP = i.ToString();
                                break;
                            }
                        }
                    }
                    string strpath = "\\\\" + msModel.monitorServerIP.Trim() + "\\" + msModel.startFile.Trim().Trim('\\').Replace('/', '\\') + "\\";
                    msModel.monitorDrive = strpath.TrimEnd('\\');
                    result = msSerivice.UpdateMonitorServer(msModel);
                    if (result > -1)
                    {
                        if (msModel.monitorDriveP.IndexOf(':') <= -1) 
                        {
                            msModel.monitorDriveP = msModel.monitorDriveP + ":";
                        }
                        if (System.IO.Directory.Exists(msModel.monitorDriveP))
                        {
                            int resultForDeleteMonitorServerFolder = msFolderService.DeleteMonitorServerFolderByServerId(Int32.Parse(msModel.id));
                            if (resultForDeleteMonitorServerFolder > -1)
                            {
                                //string strpath = "\\\\" + msModel.monitorServerIP.Trim() + "\\" + msModel.startFile.Trim().Trim('\\').Replace('/', '\\') + "\\";
                                DirectoryInfo di = new DirectoryInfo(msModel.monitorDrive);
                                MonitorServerFolder model = new MonitorServerFolder();
                                model.monitorServerID = Convert.ToInt32(msModel.id);
                                model.monitorFileType = "99";
                                model.monitorFilePath = strpath.TrimEnd('\\');
                                model.monitorFileName = "";
                                model.createDate = dt;
                                model.updateDate = dt;
                                model.creater = Session["LoginId"].ToString();
                                model.updater = Session["LoginId"].ToString();
                                model.monitorFileName = "";
                                model.monitorFlg = "0";
                                model.initFlg = "1";
                                model.deleteFlg = 0;
                                int innerId = msFolderService.InsertMonitorServerFolder(model);
                            }
                        }
                        int monitorFileListenForResult = msFileListenService.UpdateMonitorServer(msModel, monitorServer);
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
                    result = msSerivice.DeleteMonitorServer(id, Session["LoginId"].ToString());
                    if (result > -1)
                    {
                        msFolderService.DeleteMonitorServerFolderByServerId(id);
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
        /// 接続テスト
        /// </summary>
        /// <param name="model"></param>
        /// <returns>result 1:接続成功；-1：接続失敗</returns>
        [HttpPost]
        public ActionResult NetworkTest(MonitorServer model)
        {
            int Result = 0;
            string ipMessage = model.monitorServerIP;
            string stfileMessage = model.startFile;
            string monitorDriveP = model.monitorDriveP;
            string accountMessage = model.account;
            string passMessage = model.password;
            //string serverFolderPath = @"\\" + ipMessage + @"\" + stfileMessage.TrimStart('\\').Replace('/','\\');
            //2014-06-02 wjd modified
            string localPath = model.monitorLocalPath;  //monitorDriveP.IndexOf(':') <= -1 ? monitorDriveP + ":" : monitorDriveP;
            try
            {
                //Use cmd to test the link.
                string ip = @"\\" + ipMessage;
                Common.NetWorkFileShare1 netWorkFileShare = new Common.NetWorkFileShare1();
                if (netWorkFileShare.ConnectState(ip, accountMessage.Trim(), passMessage.Trim()))
                {
                    Result = 1;
                }
                else
                {
                    Result = -1;
                }

                //int status = Common.NetworkConnection.Connect(serverFolderPath, localPath, accountMessage, passMessage);
                //if (status == (int)Common.ERROR_ID.ERROR_SUCCESS)
                //{
                //    Result = 1;
                //}
                //else
                //{
                //    Result = -1;
                //}
                logger.Info("接続状態:" + Result.ToString());
            }
            catch(Exception ex)
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
        /// 転送元名が存在するかをチェックする。
        /// </summary>
        /// <param name="msName"></param>
        /// <returns></returns>
        public ActionResult CheckNameIsExists(string msName)
        {
            int result = 0;
            try
            {
                IList<MonitorServer> msList = msSerivice.GetMonitorServerListByName(msName);
                result = msList.Count;
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
        /// 保存選択の子フォルダ
        /// </summary>
        /// <param name="strFolderPath"></param>
        /// <param name="dt"></param>
        /// <param name="msID"></param>
        /// <returns></returns>
        private int InsertFolder(string strFolderPath, string dt, int msID)
        {
            int result = 0;
            try
            {
                if (Directory.Exists(strFolderPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(strFolderPath);
                    foreach (DirectoryInfo folder in dirInfo.GetDirectories())
                    {
                        MonitorServerFolder model = new MonitorServerFolder();
                        model.createDate = dt;
                        model.updateDate = dt;
                        model.creater = Session["LoginId"].ToString();
                        model.updater = Session["LoginId"].ToString();
                        model.monitorFlg = "0";
                        model.deleteFlg = 0;
                        model.initFlg = "0";
                        model.monitorServerID = msID;
                        model.monitorFileName = folder.Name;
                        model.monitorFilePath = strFolderPath;
                        model.monitorFileType = "99";
                        result = msFolderService.InsertMonitorServerFolder(model);
                        result = InsertFolder(folder.FullName, dt, msID);
                    }
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        MonitorServerFolder model = new MonitorServerFolder();
                        model.createDate = dt;
                        model.updateDate = dt;
                        model.creater = Session["LoginId"].ToString();
                        model.updater = Session["LoginId"].ToString();
                        model.monitorFlg = "1";
                        model.deleteFlg = 0;
                        model.initFlg = "0";
                        model.monitorServerID = msID;
                        model.monitorFileName = file.Name;
                        model.monitorFilePath = strFolderPath;
                        model.monitorFileType = file.Extension.ToString();
                        result = msFolderService.InsertMonitorServerFolder(model);
                    }
                }
            }
            catch (Exception ex)
            {
                result = -99;
                logger.Error(ex.Message);
            }
            return result;
        }

        [HttpPost]
        public ActionResult FolderSelectPartial()
        {
            try
            {
                return PartialView("FolderSelectPartial");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

    }
}
