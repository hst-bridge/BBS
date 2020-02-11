using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Model;
using budbackup.CommonWeb;
using IBLL;
using DevExpress.Web.Mvc;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web;
using log4net;

namespace budbackup.Controllers
{
    public class FileSpyController : BaseController
    {
        //
        // GET: /FileSpy/
        private readonly IMonitorServerService msService = BLLFactory.ServiceAccess.CreateMonitorServerService();
        private readonly IMonitorServerFolderService msFolderService = BLLFactory.ServiceAccess.CreateMonitorServerFolderService();
        private readonly ILog logger = LogManager.GetLogger(typeof(FileSpyController));
        private static IList<FileTypeSet> setFileList;
        private static IList<MonitorServerFolder> globalfolderList;
        private static List<MonitorServerFolder> tempRemoveLists = new List<MonitorServerFolder>();
        public ActionResult Index(string monistorId)
        {
            //session値チェック
            if (Session["LoginId"] == null)
            {
                return RedirectToAction("Account", "Account/LogOn", new { url = Request.Url });
            }
            string strMonitorId = string.Empty;
            try
            {
                IList<MonitorServer> monitorList = msService.GetMonitorServerList();
                ViewData["monitorList"] = monitorList;
                ViewData["expandedPath"] = Request.QueryString["expandedPath"];
                ViewData["StartPath"] = "";
                ViewData["initMonistorId"] = "";
                TempData["msID"] = 0;

                MonitorServer msModel = null;
                if (!String.IsNullOrWhiteSpace(monistorId))
                {
                    msModel = msService.GetMonitorServerById(int.Parse(monistorId));
                }
                else if (monitorList.Count > 0)
                {
                    msModel = monitorList[0];
                }

                if (msModel != null)
                {
                    ViewData["StartPath"] = msModel.monitorDrive.TrimEnd('\\');
                    ViewData["initMonistorId"] = msModel.id;
                    TempData["msID"] = msModel.id;
                    if (!System.IO.Directory.Exists(@"\\" + msModel.monitorServerIP + @"\" + msModel.startFile.TrimStart('\\').Replace('/', '\\')))
                    {
                        int status = Common.NetworkConnection.Connect(@"\\" + msModel.monitorServerIP + @"\" + msModel.startFile.TrimStart('\\').Replace('/', '\\'), msModel.monitorDriveP, msModel.account, msModel.password);
                    }
                    //connState = networkConntion(monitorList[0].monitorServerIP, monitorList[0].startFile, monitorList[0].monitorDrive, monitorList[0].account, monitorList[0].password);
                }

                List<Model.MonitorServerFolder> folderList = new List<MonitorServerFolder>();
                ViewData["ClientSideEvents"] = new string[]{
                "Init",
                "ExpandedChanging",
                "ExpandedChanged",
                "CheckedChanged",
                "NodeClick"
                };
                ViewData["initFolderDetail"] = GetinitFolderList(strMonitorId);
            }
            catch (Exception ex)
            {
                ViewData["monitorList"] = new List<MonitorServer>();
                TempData["msID"] = 0;
                ViewData["StartPath"] = "";
                ViewData["initMonistorId"] = "";
                ViewData["initFolderDetail"] = new List<Models.MonitFolder>();
                logger.Error(ex.Message);
            }
            return View();
        }
        public ActionResult Edit(string msID, string msrFolderName)
        {
            ViewData["msID"] = msID;
            ViewData["msrFolderName"] = msrFolderName;
            IFileTypeSetService setService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
            //ViewData["setFile"] = setService.GetFileTypeSetByMonitorServerID(msID) ;
            try
            {
                ViewData["setFile"] = setService.GetFileTypeSetByMonitorServerIdAndFolderName(msID, msrFolderName);
            }
            catch (Exception ex)
            {
                ViewData["setFile"] = new List<FileTypeSet>();
                logger.Error(ex.Message);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Add(string dataJson, int  intMonitorServerID)
        {
            int result = 0;

            if (Session["LoginId"] == null)
            {
                result = -99;
            }
            else
            {
                try {
                    List<MonitorServerFolder> folderList = new List<MonitorServerFolder>();
                    folderList = JsonHelper.ParseFormJson<List<MonitorServerFolder>>(dataJson);
                    IFileTypeSetService setService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
                    setFileList = setService.GetFileTypeSetByMonitorServerID(intMonitorServerID.ToString());
                    string dt = Common.CommonUtil.DateTimeNowToString();
                    msFolderService.DeleteMonitorServerFolderByServerId(intMonitorServerID);

                    //format folder list
                    globalfolderList = folderList;

                    tempRemoveLists.Clear();
                    //Not filter——2014-06-11 wjd commented
                    //formatListsByFileTypeSet(intMonitorServerID.ToString());

                    foreach (MonitorServerFolder model in globalfolderList)
                    {
                        if (model != null)
                        {
                            model.id = null;
                            model.monitorFilePath = model.monitorFilePath.TrimEnd('\\');
                            if (model.monitorFileType == "99")
                            { //選択のはフォルダの場合
                                string strpath = string.Empty;
                                if (model.monitorFileName == "")
                                {
                                    strpath = model.monitorFilePath;
                                }
                                else
                                {
                                    strpath = model.monitorFilePath + "\\" + model.monitorFileName;
                                }
                                if (Directory.Exists(strpath))
                                {
                                    FileTypeSet fset = setService.GetFileTypeSetByMonitorServerIdAndFolderName(intMonitorServerID.ToString(), model.monitorFilePath);
                                    DirectoryInfo dirInfo = new DirectoryInfo(strpath);
                                    if (fset != null)
                                    {
                                        if (fset.hiddenFileFlg == "1")
                                        {//隠しファイル
                                            if ((dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                            {
                                                continue;
                                            }
                                        }
                                        if (fset.systemFileFlg == "1")
                                        {//システムファイル
                                            if ((dirInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    model.createDate = dt;
                                    model.updateDate = dt;
                                    model.creater = Session["LoginId"].ToString();
                                    model.updater = Session["LoginId"].ToString();
                                    model.monitorServerID = intMonitorServerID;
                                    model.monitorFlg = "0";
                                    model.initFlg = "1";
                                    model.deleteFlg = 0;
                                    result = msFolderService.InsertMonitorServerFolder(model);
                                    //Not save subfolder——2014-06-11 wjd commented
                                    //result = InsertFolder(strpath, dt.ToString(), intMonitorServerID);
                                }
                            }
                            else
                            {//選択のはファイルの場合
                                string pathName = model.monitorFilePath + "\\" + model.monitorFileName;
                                FileTypeSet fset = setService.GetFileTypeSetByMonitorServerIdAndFolderName(intMonitorServerID.ToString(), model.monitorFilePath);
                                FileInfo fileInfo = new FileInfo(pathName);
                                if (fset != null)
                                {
                                    if (fset.hiddenFileFlg == "1")
                                    {//隠しファイル
                                        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                        {
                                            continue;
                                        }
                                    }
                                    if (fset.systemFileFlg == "1")
                                    {//システムファイル
                                        if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                model.createDate = dt;
                                model.updateDate = dt;
                                model.creater = Session["LoginId"].ToString();
                                model.updater = Session["LoginId"].ToString();
                                model.monitorServerID = intMonitorServerID;
                                model.monitorFlg = "1";
                                model.deleteFlg = 0;
                                model.initFlg = "1";
                                result = msFolderService.InsertMonitorServerFolder(model);
                            }
                           
                        }
                    }
                    /* --選択条件　不要
                    foreach (FileTypeSet fts in setFileList)
                    {
                        if (Directory.Exists(fts.monitorServerFolderName))
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(fts.monitorServerFolderName);
                            foreach (FileInfo file in dirInfo.GetFiles())
                            {
                               
                                if (CommonUtil.IsExceptFile(file.Name, fts.includeAttribute1, fts.includeAttribute2, fts.includeAttribute3))
                                {
                                    bool bSaved = false;
                                    foreach (MonitorServerFolder modelFolder in folderList)
                                    {

                                        if (fts.monitorServerFolderName == modelFolder.monitorFilePath && file.Name == modelFolder.monitorFileName)
                                        {
                                            bSaved = true;
                                            break;
                                        }
                                    }
                                    if (bSaved)
                                    {
                                        continue;
                                    }
                                    MonitorServerFolder model = new MonitorServerFolder();
                                    //FileInfo file = new FileInfo(strFolderPath);
                                    model.createDate = dt.ToString();
                                    model.updateDate = dt.ToString();
                                    model.creater = Session["LoginId"].ToString();
                                    model.updater = Session["LoginId"].ToString();
                                    model.monitorFlg = "1";
                                    model.deleteFlg = 0;
                                    model.initFlg = "1";
                                    model.monitorServerID = intMonitorServerID;
                                    model.monitorFileName = file.Name;
                                    model.monitorFilePath = file.DirectoryName;
                                    model.monitorFileType = file.Extension.ToString();
                                    result = msFolderService.InsertMonitorServerFolder(model);
                                }
                            }
                          
                        }
                    }
                    */
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
        [HttpPost]
        public ActionResult VirtualModePartial(string monistorId)
        {
            //ViewData["options2"] = options;
            MonitorServer initMonitor = new MonitorServer();
            try
            {
                if (monistorId != null)
                {
                    initMonitor = msService.GetMonitorServerById(Convert.ToInt32(monistorId));
                    ViewData["initFolderDetail"] = GetinitFolderList(monistorId);
                }
                ViewData["StartPath"] = initMonitor.monitorDrive;
                ViewData["msID"] = TempData["msID"];
               
            }
            catch(Exception ex)
            {
                ViewData["StartPath"] = "";
                ViewData["msID"] = 0;
                logger.Error(ex.Message);
            }
            return PartialView("VirtualModePartial");
        }
        public ActionResult PathSelectModePartial(string monistorId)
        {
            MonitorServer initMonitor = new MonitorServer();
            try
            {
                if (monistorId != null)
                {
                    initMonitor = msService.GetMonitorServerById(Convert.ToInt32(monistorId));
                    ViewData["initFolderDetail"] = GetinitFolderList(monistorId);
                }
                ViewData["StartPath"] = initMonitor.monitorDrive;
                ViewData["msID"] = TempData["msID"];

            }
            catch (Exception ex)
            {
                ViewData["StartPath"] = "";
                ViewData["msID"] = 0;
                logger.Error(ex.Message);
            }
            return PartialView("PathSelectModePartial");
        }
        /// <summary>
        /// 右側フォルダ明細を取得する。
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="checkstate"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFolderDetail(string folderPath,bool checkstate,string msID)
        {
            string strResult = string.Empty;
            List<budbackup.Models.FolderDetail> detailList = new List<Models.FolderDetail>();
            try
            {
                if (Directory.Exists(folderPath))
                {
                    IFileTypeSetService setService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
                    FileTypeSet fset = setService.GetFileTypeSetByMonitorServerIdAndFolderName(msID, folderPath);
                    DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                    foreach (DirectoryInfo folder in dirInfo.GetDirectories())
                    {
                        if (fset.id != null)
                        {
                            if (fset.hiddenFileFlg == "1")
                            {//隠しファイル
                                if ((folder.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                {
                                    continue;
                                }
                            }
                            if (fset.systemFileFlg == "1")
                            {//システムファイル
                                if ((folder.Attributes & FileAttributes.System) == FileAttributes.System)
                                {
                                    continue;
                                }
                            }
                        }
                        budbackup.Models.FolderDetail detail = new Models.FolderDetail();
                        detail.checkState = checkstate;
                        detail.fileExtensionType = "99";//99はフォルダーのフラグ
                        detail.fileLastUpdateTime = folder.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");
                        detail.fileName = folder.Name;
                        detail.fileSize = "&lt;DIR&gt;";
                        detail.filePath = folderPath;
                        detailList.Add(detail);

                    }
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {

                        if (fset.id != null)
                        {
                            //string strExceptAttribute1 = string.Empty;
                            //string strExceptAttribute2 = string.Empty;
                            //string strExceptAttribute3 = string.Empty;
                            //if (fset.exceptAttributeFlg1 == "1")
                            //{
                            //    strExceptAttribute1 = fset.exceptAttribute1;
                            //}
                            //if (fset.exceptAttributeFlg2 == "1")
                            //{
                            //    strExceptAttribute2 = fset.exceptAttribute2;
                            //}
                            //if (fset.exceptAttributeFlg3 == "1")
                            //{
                            //    strExceptAttribute3 = fset.exceptAttribute3;
                            //}
                            //if (CommonWeb.CommonUtil.IsExceptFile(file.Name, strExceptAttribute1, strExceptAttribute2, strExceptAttribute3))
                            //{
                            //    continue;
                            //}
                            if (fset.hiddenFileFlg == "1")
                            {//隠しファイル
                                if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                {
                                    continue;
                                }
                            }
                            if (fset.systemFileFlg == "1")
                            {//システムファイル
                                if ((file.Attributes & FileAttributes.System) == FileAttributes.System)
                                {
                                    continue;
                                }
                            }
                        }
                        budbackup.Models.FolderDetail detail = new Models.FolderDetail();
                        detail.checkState = checkstate;
                        detail.fileExtensionType = file.Extension.ToString();
                        detail.fileLastUpdateTime = file.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");
                        detail.fileName = file.Name;
                        detail.fileSize = file.Length.ToString("###,###");
                        detail.filePath = folderPath;
                        detailList.Add(detail);

                    }
                }
                else if (System.IO.File.Exists(folderPath))
                {
                    FileInfo file = new FileInfo(folderPath);
                    budbackup.Models.FolderDetail detail = new Models.FolderDetail();
                    detail.checkState = checkstate;
                    detail.fileExtensionType = file.Extension.ToString();
                    detail.fileLastUpdateTime = file.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");
                    detail.fileName = file.Name;
                    detail.fileSize = file.Length.ToString("###,###");
                    detail.filePath = file.DirectoryName;
                    detailList.Add(detail);

                }
                strResult = JsonHelper.GetJson<List<budbackup.Models.FolderDetail>>(detailList);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            Response.Write(strResult);
            Response.End();
            return null;
        }
        /// <summary>
        /// 初期化の時　転送のファイルを取得する
        /// </summary>
        /// <param name="strMonitorServerID">転送元ID</param>
        /// <returns></returns>
        public ActionResult GetInitMonitorFolderDatail(string monistorId)
        {
            string strResult = string.Empty;
            try
            {
                //IList<MonitorServerFolder> folderList = msFolderService.GetMonitorServerFolderByMonitorServerID(monistorId);

                //IList<MonitorServerFolder> folderList = msFolderService.GetMonitorFolderByServerIDAndInitFlg(monistorId);
                //List<budbackup.Models.MonitFolder> initFolderList = new List<Models.MonitFolder>();
                //if (folderList.Count > 0)
                //{
                //    foreach (MonitorServerFolder folderModel in folderList)
                //    {
                //        budbackup.Models.MonitFolder model = new Models.MonitFolder();
                //        model.monitorFileName = folderModel.monitorFileName;
                //        model.monitorFilePath = folderModel.monitorFilePath;
                //        model.monitorFileType = folderModel.monitorFileType;
                //        initFolderList.Add(model);
                //    }
                //    strResult = JsonHelper.GetJson<List<budbackup.Models.MonitFolder>>(initFolderList);
                //}
                MonitorServer monitorServer = msService.GetMonitorServerById(int.Parse(monistorId));
                if (!System.IO.Directory.Exists(@"\\" +monitorServer.monitorServerIP + @"\" + monitorServer.startFile.TrimStart('\\').Replace('/','\\')))
                {
                    int status = Common.NetworkConnection.Connect(@"\\" + monitorServer.monitorServerIP + @"\" + monitorServer.startFile.TrimStart('\\').Replace('/', '\\'), monitorServer.monitorDriveP, monitorServer.account, monitorServer.password);
                }

                List<budbackup.Models.MonitFolder> initFolderList = new List<Models.MonitFolder>();
                globalfolderList = msFolderService.GetMonitorFolderByServerIDAndInitFlg(monistorId);

                //format folder list——2014-06-11 wjd commented——Not filter
                //formatListsByFileTypeSet(monistorId);

                if (globalfolderList.Count > 0)
                {
                    foreach (MonitorServerFolder folderModel in globalfolderList)
                    {
                        budbackup.Models.MonitFolder model = new Models.MonitFolder();
                        model.monitorFileName = folderModel.monitorFileName;
                        model.monitorFilePath = folderModel.monitorFilePath;
                        model.monitorFileType = folderModel.monitorFileType;
                        initFolderList.Add(model);
                    }
                    strResult = JsonHelper.GetJson<List<budbackup.Models.MonitFolder>>(initFolderList);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
            Response.Write(strResult);
            Response.End();
            return null;
        }
        /// <summary>
        /// get except condition
        /// </summary>
        private void formatListsByFileTypeSet(string monistorId)
        {
            IFileTypeSetService iftss = BLLFactory.ServiceAccess.CreateFileTypeSetService();
            string monitorServerId = monistorId;
            IList<FileTypeSet> fileTypeSetList = iftss.GetFileTypeSetByMonitorServerID(monitorServerId);
            foreach (FileTypeSet fileTypeSet in fileTypeSetList)
            {
                if (!Directory.Exists(fileTypeSet.monitorServerFolderName))
                {
                    continue;
                }
                List<string> exList = new List<string>();
                if (fileTypeSet.exceptAttributeFlg1 == "1" && !string.IsNullOrEmpty(fileTypeSet.exceptAttribute1))
                {
                    exList.Add(fileTypeSet.exceptAttribute1);
                }
                if (fileTypeSet.exceptAttributeFlg2 == "1" && !string.IsNullOrEmpty(fileTypeSet.exceptAttribute2))
                {
                    exList.Add(fileTypeSet.exceptAttribute2);
                }
                if (fileTypeSet.exceptAttributeFlg3 == "1" && !string.IsNullOrEmpty(fileTypeSet.exceptAttribute3))
                {
                    exList.Add(fileTypeSet.exceptAttribute3);
                }
                DirectoryInfo dir = new DirectoryInfo(fileTypeSet.monitorServerFolderName);
                FileInfo[] files = dir.GetFiles();
                if (files.Count() > 0)
                {
                    foreach (FileInfo fileInfo in files)
                    {
                        string extension = fileInfo.Extension;
                        if (!exList.Contains(extension))
                        {
                            continue;
                        }
                        string fullPath = fileInfo.FullName.IndexOf('\\') > -1 ? fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf('\\')) : fileInfo.FullName;
                        MonitorServerFolder msf = new MonitorServerFolder();
                        msf.monitorServerID = Convert.ToInt32(monistorId);
                        msf.monitorFilePath = fullPath;
                        msf.monitorFileName = fileInfo.Name;
                        msf.monitorFileType = fileInfo.Extension;
                        formatListsData(msf, monistorId);
                    }
                }
            }
        }
        /// <summary>
        /// format except list and remove
        /// </summary>
        /// <param name="msf"></param>
        private void formatListsData(MonitorServerFolder msf, string monistorId)
        {
            string msfFullPath = msf.monitorFilePath + "\\" + msf.monitorFileName;
            List<MonitorServerFolder> tempLists = new List<MonitorServerFolder>();
            //List<MonitorServerFolder> tempRemoveLists = new List<MonitorServerFolder>();
            if (globalfolderList.Count > 0)
            {
                foreach (MonitorServerFolder innerMsf in globalfolderList)
                {
                    string listFullPath = innerMsf.monitorFilePath + "\\" + innerMsf.monitorFileName;
                    if (msfFullPath.IndexOf(listFullPath) > -1)
                    {
                        if (lists_exist(tempRemoveLists, innerMsf) <= -1)
                        {
                            tempRemoveLists.Add(innerMsf);
                        }
                        if (lists_exist(tempRemoveLists, msf) <= -1)
                        {
                            tempRemoveLists.Add(msf);
                        }
                        if (Directory.Exists(listFullPath))
                        {
                            DirectoryInfo dir = new DirectoryInfo(listFullPath);
                            FileSystemInfo[] fileSystemInfos = dir.GetFileSystemInfos();
                            foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
                            {
                                string fullPath = fileSystemInfo.FullName.IndexOf('\\') > -1 ? fileSystemInfo.FullName.Substring(0, fileSystemInfo.FullName.LastIndexOf('\\')) : fileSystemInfo.FullName;
                                string extension = fileSystemInfo.Extension;
                                if (Directory.Exists(fileSystemInfo.FullName))
                                {
                                    extension = "99";
                                }
                                MonitorServerFolder msfl = new MonitorServerFolder();
                                msfl.monitorServerID = Convert.ToInt32(monistorId);
                                msfl.monitorFilePath = fullPath;
                                msfl.monitorFileName = fileSystemInfo.Name;
                                msfl.monitorFileType = extension;
                                if (msfFullPath.IndexOf(fileSystemInfo.FullName) > -1 && !Directory.Exists(fileSystemInfo.FullName))
                                {
                                    continue;
                                }
                                else
                                {
                                    tempLists.Add(msfl);
                                }
                            }
                        }
                    }
                }
                if (tempLists.Count > 0)
                {
                    foreach (MonitorServerFolder innerMsf in tempLists)
                    {
                        lists_contain(innerMsf);
                        globalfolderList.Add(innerMsf);
                    }
                }
                if (tempRemoveLists.Count > 0)
                {
                    foreach (MonitorServerFolder innerMsf in tempRemoveLists)
                    {
                        lists_contain(innerMsf);
                    }
                }
            }
        }
        private int lists_contain(MonitorServerFolder list)
        {
            int index = -1;
            for (int i = 0; i < globalfolderList.Count; i++)
            {
                if (globalfolderList[i].monitorServerID == list.monitorServerID
                    && globalfolderList[i].monitorFileName == list.monitorFileName
                    && globalfolderList[i].monitorFilePath == list.monitorFilePath)
                {
                    index = i;
                    globalfolderList.RemoveAt(i);
                    break;
                }
            }
            return index;
        }
        private int lists_exist(List<MonitorServerFolder> tempRemoveLists, MonitorServerFolder list)
        {
            int index = -1;
            for (int i = 0; i < tempRemoveLists.Count; i++)
            {
                if (tempRemoveLists[i].monitorServerID == list.monitorServerID
                    && tempRemoveLists[i].monitorFileName == list.monitorFileName
                    && tempRemoveLists[i].monitorFilePath == list.monitorFilePath)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        /// <summary>
        /// フォルダ設定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddFileTypeSet(FileTypeSet model)
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
                    //FileTypeSet model = JsonHelper.ParseFormJson<FileTypeSet>(jsonModel);
                    string dt = Common.CommonUtil.DateTimeNowToString();
                    IFileTypeSetService setService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
                    if (model.monitorServerFolderName == null)
                    {
                        model.monitorServerFolderName = "";
                    }
                    if (model.systemFileFlg == null)
                    {
                        model.systemFileFlg = "0";
                    }
                    if (model.hiddenFileFlg == null)
                    {
                        model.hiddenFileFlg = "0";
                    }
                    if (model.exceptAttributeFlg1 == null)
                    {
                        model.exceptAttributeFlg1 = "0";
                    }
                    if (model.exceptAttributeFlg2 == null)
                    {
                        model.exceptAttributeFlg2 = "0";
                    }
                    if (model.exceptAttributeFlg3 == null)
                    {
                        model.exceptAttributeFlg3 = "0";
                    }
                    if (model.exceptAttribute1 == null)
                    {
                        model.exceptAttribute1 = "";
                    }
                    if (model.exceptAttribute2 == null)
                    {
                        model.exceptAttribute2 = "";
                    }
                    if (model.exceptAttribute3 == null)
                    {
                        model.exceptAttribute3 = "";
                    }
                    if (model.id == null || model.id == "")
                    {//インサート
                        model.createDate = dt;
                        model.updateDate = dt;
                        model.creater = Session["LoginId"].ToString();
                        model.updater = Session["LoginId"].ToString();
                        result = setService.InsertFileTypeSet(model);
                    }
                    else
                    { //更新
                        model.updateDate = dt;
                        model.updater = Session["LoginId"].ToString();
                        result = setService.UpdateFileTypeSet(model);
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
        public ActionResult GetFileTypeSet(string msID,string folderName)
        {
            string strResult = string.Empty;
            IFileTypeSetService setService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
            try
            {
                FileTypeSet fSet = setService.GetFileTypeSetByMonitorServerIdAndFolderName(msID, folderName);
                strResult = JsonHelper.GetJson<FileTypeSet>(fSet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            Response.Write(strResult);
            Response.End();
            return null;
        }

        /// <summary>
        /// 获取所有父节点的除外条件
        /// </summary>
        /// 2014-8-30 wjd add
        /// <param name="msID"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public ActionResult GetBranchFileTypeSet(string msID, string folderName)
        {
            string strResult = string.Empty;
            IFileTypeSetService setService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
            try
            {
                string p1 = folderName.Substring(folderName.IndexOf("\\\\") + 2);
                string[] ps = p1.Split(new char[] { '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
                List<string> pss = new List<string>();
                int j = 0;
                for (int i = 0; i < ps.Length; i++)
                {
                    j = i;
                    if (ps[i] == "")
                    {
                        j -= 1;
                        continue;
                    }
                    if (i == 0)
                    {
                        pss.Add("\\\\" + ps[i]);
                    }
                    else
                    {
                        pss.Add(pss[j - 1] + "\\" + ps[i]);
                    }
                }
                List<string> exceptedExt = new List<string>();
                foreach (string path in pss)
                {
                    FileTypeSet fts = setService.GetFileTypeSetByMonitorServerIdAndFolderName(msID, path);
                    string ea1 = fts.exceptAttribute1;
                    bool flag1 = fts.exceptAttributeFlg1 == "1";
                    string ea2 = fts.exceptAttribute2;
                    bool flag2 = fts.exceptAttributeFlg2 == "1";
                    string ea3 = fts.exceptAttribute3;
                    bool flag3 = fts.exceptAttributeFlg3 == "1";

                    if (flag1 && ea1 != "")
                    {
                        exceptedExt.Add(ea1);
                    }
                    if (flag2 && ea2 != "")
                    {
                        exceptedExt.Add(ea2);
                    }
                    if (flag3 && ea3 != "")
                    {
                        exceptedExt.Add(ea3);
                    }
                }

                strResult = JsonHelper.GetJson<List<string>>(exceptedExt);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            Response.Write(strResult);
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
        private int InsertFolder(string strFolderPath,string dt,int msID)
        {
            int result = 0;
            try
            {
                if (Directory.Exists(strFolderPath))
                {
                    FileTypeSet fset = searchFileTypeSet(msID.ToString(), strFolderPath);
                    DirectoryInfo dirInfo = new DirectoryInfo(strFolderPath);
                    foreach (DirectoryInfo folder in dirInfo.GetDirectories())
                    {
                        if (fset != null)
                        {
                            if (fset.hiddenFileFlg == "1")
                            {//隠しファイル
                                if ((folder.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                {
                                    continue;
                                }
                            }
                            if (fset.systemFileFlg == "1")
                            {//システムファイル
                                if ((folder.Attributes & FileAttributes.System) == FileAttributes.System)
                                {
                                    continue;
                                }
                            }
                        }
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
                        //model.monitorFilePath = folder.Parent.FullName.ToString();
                        model.monitorFilePath = strFolderPath;
                        model.monitorFileType = "99";
                        result = msFolderService.InsertMonitorServerFolder(model);
                        result = InsertFolder(folder.FullName,dt,msID);
                    }
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        if (fset != null)
                        {
                            string strExceptAttribute1 = string.Empty;
                            string strExceptAttribute2 = string.Empty;
                            string strExceptAttribute3 = string.Empty;
                            if (fset.exceptAttributeFlg1 == "1")
                            {
                                strExceptAttribute1 = fset.exceptAttribute1;
                            }
                            if (fset.exceptAttributeFlg2 == "1")
                            {
                                strExceptAttribute2 = fset.exceptAttribute2;
                            }
                            if (fset.exceptAttributeFlg3 == "1")
                            {
                                strExceptAttribute3 = fset.exceptAttribute3;
                            }
                            if (CommonUtil.IsExceptFile(file.Name, strExceptAttribute1, strExceptAttribute2, strExceptAttribute3))
                            {
                                continue;
                            }
                            if (fset.hiddenFileFlg == "1")
                            {//隠しファイル
                                if ((file.Attributes & FileAttributes.Hidden)  == FileAttributes.Hidden)
                                {
                                    continue;
                                }
                            }
                            if (fset.systemFileFlg == "1")
                            {//システムファイル
                                if ((file.Attributes & FileAttributes.System) == FileAttributes.System)
                                {
                                    continue;
                                }
                            }
                        }
                        MonitorServerFolder model = new MonitorServerFolder();
                        //FileInfo file = new FileInfo(strFolderPath);
                        model.createDate = dt;
                        model.updateDate = dt;
                        model.creater = Session["LoginId"].ToString();
                        model.updater = Session["LoginId"].ToString();
                        model.monitorFlg = "1";
                        model.deleteFlg = 0;
                        model.initFlg = "0";
                        model.monitorServerID = msID;
                        model.monitorFileName = file.Name;
                        //model.monitorFilePath = file.DirectoryName;
                        model.monitorFilePath = strFolderPath;
                        model.monitorFileType = file.Extension.ToString();
                        result = msFolderService.InsertMonitorServerFolder(model);
                    }
                }
            }
            catch(Exception ex)
            {
                result = -99;
                logger.Error(ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 初期化チェック済のフォルダ・ファイル
        /// </summary>
        /// <param name="monistorId"></param>
        /// <returns></returns>
        private List<budbackup.Models.MonitFolder> GetinitFolderList(string monistorId)
        {
            List<budbackup.Models.MonitFolder> initFolderList = new List<Models.MonitFolder>();
            try
            {
                IList<MonitorServerFolder> folderList = msFolderService.GetMonitorServerFolderByMonitorServerID(monistorId);

                if (folderList.Count > 0)
                {
                    foreach (MonitorServerFolder folderModel in folderList)
                    {
                        budbackup.Models.MonitFolder model = new Models.MonitFolder();
                        model.monitorFileName = folderModel.monitorFileName;
                        model.monitorFilePath = folderModel.monitorFilePath;
                        model.monitorFileType = folderModel.monitorFileType;
                        initFolderList.Add(model);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
            return initFolderList;
        }
        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="strIp"></param>
        /// <param name="strStartFile"></param>
        /// <param name="strDrive"></param>
        /// <param name="strAccount"></param>
        /// <param name="strPW"></param>
        private int networkConntion(string strIp,string strStartFile,string strDrive,string strAccount,string strPW)
        {
            int Result = 0;
            //string ipMessage = strIp;
            //string stfileMessage = strStartFile;
            //string monitorDrive = strDrive;
            //string accountMessage = strAccount;
            //string passMessage = strPW;
            string serverFolderPath = @"\\" + strIp + @"\" + strStartFile.TrimStart('\\');
            string localPath = strDrive + ":";
            int status = Common.NetworkConnection.Connect(serverFolderPath, localPath, strAccount, strPW);
            if (status == (int)Common.ERROR_ID.ERROR_SUCCESS)
            {
                Result = 1;
            }
            else
            {
                Result = -1;
            }
            return Result;
        }

        private FileTypeSet searchFileTypeSet(string msID,string folderName)
        { 
            FileTypeSet fSet = null;
            foreach (FileTypeSet fts in setFileList)
            {
                if (fts.monitorServerID.ToString() == msID && fts.monitorServerFolderName == folderName)
                {
                    fSet = fts;
                    setFileList.Remove(fts);
                    break;
                }
            }
            return fSet;
        }
    }
}
