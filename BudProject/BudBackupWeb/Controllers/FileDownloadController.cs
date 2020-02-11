using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using IBLL;
using budbackup.CommonWeb;
using log4net;
using System.IO;
//using ICSharpCode.SharpZipLib.Zip;
//using ICSharpCode.SharpZipLib.Checksums;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using Ionic.Zip;
using System.Net;
using Microsoft.VisualBasic;
using budbackup.BLL;
using System.Text;
using budbackup.CommonWeb.Helper;
using System.Collections;


namespace budbackup.Controllers
{
    public class FileDownloadController : BaseController
    {
        // GET: /FileDownload/
        private readonly IMonitorFileListenService msSerivice = BLLFactory.ServiceAccess.CreateMonitorFileListenService();
        private readonly IMonitorServerService mService = BLLFactory.ServiceAccess.CreateMonitorServerService();
        private readonly IManualBackupServerService mManualService = BLLFactory.ServiceAccess.CreateManualBackupServerService();
        private readonly ILog logger = LogManager.GetLogger(typeof(FileDownloadController));
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

            object loginId = Session["LoginId"];
            string username = loginId as string;
            try
            {
                FileDownloadService fds = new FileDownloadService();
                string dbServerip = ConfigUtil.AppSetting("remoteIP");
               
                List<Models.MonitorServer> mslist = fds.GetRemoteMSList(dbServerip) ?? new List<Models.MonitorServer>();

                if ("soumu".Equals(username))
                {
                    mslist = mslist.FindAll(x => x.MonitorServerName.Equals("soumu"));
                }
                else if ("mac".Equals(username)) { 
                    mslist = mslist.FindAll(x => !x.MonitorServerName.Equals("soumu"));
                }

                ViewData["msList"] = mslist;
            }
            catch (Exception ex) 
            {
                ViewData["msList"] = new List<Models.MonitorServer>();
                logger.Error(ex.Message);
            }
            
             if (loginId != null)
             {
                
                 if ("admin".Equals(username))
                 {
                     return View();
                 }
             }

             return View("NmIndex");
           
        }
        public ActionResult Search_old(MonitorFileListen MonitorFileListen)
        {
            string result = string.Empty;
            //session
            if (Session["LoginId"] == null)
            {
                result = "-99";
            }
            else
            {
                //save
                try
                {
                    IList<MonitorFileListen> MonitorFileListens = msSerivice.GetMonitorFileListenList(MonitorFileListen.monitorFileName, Convert.ToInt32(MonitorFileListen.monitorServerID));
                    result = JsonHelper.GetJson(MonitorFileListens);
                }
                catch (Exception ex)
                {
                    result = "-10";
                    logger.Error(ex.Message);
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }
        public ActionResult Search(string monitorFileName, string monitorServerID,string dirname, int pindex, int pagesize) 
        {
            string result = string.Empty;
            //session
            if (Session["LoginId"] == null)
            {
                result = "-99";
            }
            else
            {
                //save
                try
                {
                    //特殊字符转为全角再检索——2014-8-29 wjd add
                    monitorFileName = this.ConvertToDoubleByteChar(monitorFileName);

                    //whether download file from local machine.
                    string localIP = Common.CommonUtil.GetLocalIP();

                    CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
                    //IList<MonitorFileListen> MonitorFileListens = msSerivice.GetMonitorFileListenList(monitorFileListen.monitorFileName, Convert.ToInt32(monitorFileListen.monitorServerID));
                    //folder
                    IList<MonitorFileListen> MonitorFileFolderListens = msSerivice.GetMonitorFileListenListByPage(monitorFileName, monitorServerID, "1", pindex, pagesize);
                    int foldertotalCount = msSerivice.GetMonitorFileListenListCount(monitorFileName, monitorServerID, "1");
                    int folderpageCount = (int)Math.Ceiling((double)foldertotalCount / pagesize) == 0 ? 1 : (int)Math.Ceiling((double)foldertotalCount / pagesize);
                    int filetotalCount = msSerivice.GetMonitorFileListenListCount(monitorFileName, monitorServerID, "0");
                    int totalCount = foldertotalCount + filetotalCount;
                    int pageCount = (int)Math.Ceiling((double)totalCount / pagesize);
                    string tableTR = "";
                    if ((foldertotalCount > 0 && foldertotalCount < pindex * pagesize)||(foldertotalCount >= pindex * pagesize))
                    {
                        if (MonitorFileFolderListens.Count > 0)
                        {
                            string fileTR = "";
                            fileTR = "<tr>" +
                                     "           <td class=\"cel1\" style=\"text-align:center;line-height:28px;\"><input type=\"checkbox\" value='@monitorFileLocalPath' class=\"filePath\" name=\"monitorFileLocalPath[]\" title=\"@id\"/></td>" +
                                     "           <td class=\"cel2\">フォルダー</td>" +
                                     "           <td class=\"cel2\">@monitorFileName</td>" +
                                     "           <td class=\"cel3\">@monitorFileFullPath</td>" +
                                     "           <td class=\"cel4\">@updateDate</td>" +
                                     "       </tr>";
                            string tr;
                            //用于处理重复消息 （当更新文件内容时，会产生多个消息）
                            var knownKeys = new HashSet<string>();
                            for (int i = 0; i < MonitorFileFolderListens.Count; i++)
                            {
                                string monitorFileDirectory = "\\\\" + MonitorFileFolderListens[i].monitorServerIP + "\\" + MonitorFileFolderListens[i].sharePoint + MonitorFileFolderListens[i].monitorFileRelativeDirectory;
                                string monitorFileFullPath = "\\\\" + MonitorFileFolderListens[i].monitorServerIP + "\\" + MonitorFileFolderListens[i].sharePoint + MonitorFileFolderListens[i].monitorFileRelativeFullPath;
                                //2014-06-04 wjd modified
                                string monitorLocalPath = MonitorFileFolderListens[i].monitorLocalPath + MonitorFileFolderListens[i].monitorFileRelativeFullPath;

                                //folder name
                                int startIndex = Compare.LastIndexOf(monitorFileDirectory, "\\" + monitorFileName, CompareOptions.IgnoreCase);
                                //int startIndex = MonitorFileFolderListens[i].monitorFileDirectory.IndexOf("\\"+monitorFileName);
                                string name = monitorFileDirectory.Substring(startIndex).Trim('\\');
                                int endIndex = name.IndexOf("\\") == -1 ? name.Length : name.IndexOf("\\");
                                MonitorFileFolderListens[i].monitorFileName = name.Substring(0,endIndex);
                                //folder local path
                                monitorLocalPath = monitorLocalPath.Replace("\\", "\\\\");
                                startIndex = Compare.LastIndexOf(monitorLocalPath, "\\" + monitorFileName, CompareOptions.IgnoreCase);
                                //startIndex = MonitorFileFolderListens[i].monitorFileLocalPath.IndexOf("\\" + monitorFileName);
                                monitorLocalPath = monitorLocalPath.Substring(0, startIndex);
                                monitorLocalPath = monitorLocalPath + "\\" + MonitorFileFolderListens[i].monitorFileName;

                                if (!knownKeys.Add(monitorLocalPath)) continue;

                                //folder full path
                                monitorFileFullPath = monitorFileFullPath.Replace("\\", "\\\\");
                                startIndex = Compare.LastIndexOf(monitorFileFullPath, "\\" + monitorFileName, CompareOptions.IgnoreCase);
                                //startIndex = MonitorFileFolderListens[i].monitorFileFullPath.IndexOf("\\" + monitorFileName);
                                monitorFileFullPath = monitorFileFullPath.Substring(0, startIndex);
                                monitorFileFullPath = monitorFileFullPath + "\\" + MonitorFileFolderListens[i].monitorFileName;
                                tr = fileTR;
                                tr = tr.Replace("@updateDate", MonitorFileFolderListens[i].updateDate);
                                tr = tr.Replace("@monitorFileFullPath", monitorFileFullPath);
                                tr = tr.Replace("@monitorFileName", MonitorFileFolderListens[i].monitorFileName);

                                //非本地时，路径为：DBServerIP + monitorLocalPath中的最后目录（已共享的目录）+ monitorFileRelativeFullPath
                                tr = tr.Replace("@monitorFileLocalPath", localIP.Equals(MonitorFileFolderListens[i].DBServerIP) ? monitorLocalPath : MonitorFileFolderListens[i].DBServerIP
                                    + (MonitorFileFolderListens[i].monitorLocalPath.Substring(MonitorFileFolderListens[i].monitorLocalPath.LastIndexOf("\\")) + MonitorFileFolderListens[i].monitorFileRelativeDirectory).Replace("\\", "\\\\"));

                                tr = tr.Replace("@id", MonitorFileFolderListens[i].id);
                                tr = tr.Replace("@monitorServerID", MonitorFileFolderListens[i].monitorServerID);
                                tableTR = tableTR + tr;
                            }
                        }
                    }
                    if (filetotalCount > 0 && foldertotalCount < pindex * pagesize)
                    {
                        int pindex2 = pindex - folderpageCount + 1;
                        pagesize = pagesize - MonitorFileFolderListens.Count;
                        //file
                        IList<MonitorFileListen> MonitorFileListens = msSerivice.GetMonitorFileListenListByPage(monitorFileName, monitorServerID, "0", pindex2, pagesize);
                        if (MonitorFileListens.Count > 0)
                        {
                            string fileTR = "";
                            fileTR = "<tr>" +
                                     "           <td class=\"cel1\" style=\"text-align:center;line-height:28px;\"><input type=\"checkbox\" value='@monitorFileLocalPath' class=\"filePath\" name=\"monitorFileLocalPath[]\" title=\"@id\"/></td>" +
                                     "           <td class=\"cel2\">ファイル</td>" +
                                     "           <td class=\"cel2\">@monitorFileName</td>" +
                                     "           <td class=\"cel3\">@monitorFileFullPath</td>" +
                                     "           <td class=\"cel4\">@updateDate</td>" +
                                     "       </tr>";
                            string tr;
                            for (int i = 0; i < MonitorFileListens.Count; i++)
                            {
                                string monitorFileDirectory = "\\\\" + MonitorFileListens[i].monitorServerIP + "\\" + MonitorFileListens[i].sharePoint + MonitorFileListens[i].monitorFileRelativeDirectory;
                                //2014-06-04 wjd modified
                                string monitorFileFullPath = "\\\\" + MonitorFileListens[i].monitorServerIP + "\\" + MonitorFileListens[i].sharePoint + MonitorFileListens[i].monitorFileRelativeFullPath;
                                string monitorLocalPath = MonitorFileListens[i].monitorLocalPath + MonitorFileListens[i].monitorFileRelativeFullPath;

                                monitorLocalPath = monitorLocalPath.Replace("\\", "\\\\");
                                monitorFileFullPath = monitorFileFullPath.Replace("\\", "\\\\");
                                tr = fileTR;
                                tr = tr.Replace("@updateDate", MonitorFileListens[i].updateDate);
                                tr = tr.Replace("@monitorFileFullPath", monitorFileFullPath);
                                tr = tr.Replace("@monitorFileName", MonitorFileListens[i].monitorFileName);

                                //非本地时，路径为：DBServerIP + monitorLocalPath中的最后目录（已共享的目录）+ monitorFileRelativeFullPath
                                tr = tr.Replace("@monitorFileLocalPath", localIP.Equals(MonitorFileListens[i].DBServerIP) ? monitorLocalPath : MonitorFileListens[i].DBServerIP
                                    + (MonitorFileListens[i].monitorLocalPath.Substring(MonitorFileListens[i].monitorLocalPath.LastIndexOf("\\")) + MonitorFileListens[i].monitorFileRelativeFullPath).Replace("\\", "\\\\"));

                                tr = tr.Replace("@id", MonitorFileListens[i].id);
                                tr = tr.Replace("@monitorServerID", MonitorFileListens[i].monitorServerID);
                                tableTR = tableTR + tr;
                            }
                        }
                    }
                    jsonData jd = new jsonData();
                    if(tableTR=="")
                    {
                        tableTR = "<tr><td colspan=\"5\" style=\"text-align:center;line-height:28px;\">データがありません。</td></tr>";
                    }
                    jd.Add("logList", tableTR)
                       .Add("pageCount", pageCount)
                       .Add("pindex", pindex)
                       .Add("totalCount", totalCount);
                    result = jd.ToString();
                    //result = JsonHelper.GetJson(MonitorFileListens);
                }
                catch (Exception ex)
                {
                    result = "-10";
                    logger.Error(ex.Message);
                }
            }
            Response.Write(result);
            Response.End();
            return null;
        }

        /// <summary>
        /// 最新的 查询文件
        /// </summary>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorServerID"></param>
        /// <param name="dirname"></param>
        /// <param name="pindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ActionResult Search_new(string monitorFileName, string monitorServerID,int pindex)
        {
            string result = string.Empty;
            //判断用户是否登陆
            if (Session["LoginId"] == null)
            {
                result = "-99";
            }
            else
            {
                #region 根据条件查询 并 拼凑html (xiecongwen 20141203
                object loginId = Session["LoginId"];
                string username = loginId as string;
                //save
                try
                {
                    //get the pagesize
                    int pagesize = 20;
                    //max entry count that should be search
                    int maxSize = 200;

                    #region 根据条件查询
                    //get the result match the condition
                    FileDownloadService fds = new FileDownloadService();
                    budbackup.Models.DownloadSearchResult ds = null;
                    //check session
                    object dsr = Session["DownloadSearchResult"];
                    if (dsr != null)
                    {
                         budbackup.Models.DownloadSearchResult dstemp = dsr as budbackup.Models.DownloadSearchResult;
                        if (dstemp.SearchPattern.Equals(monitorFileName) && dstemp.MonitorServerID.Equals(monitorServerID))
                        {
                            ds = dstemp;
                        }
                    }

                    //get from db
                    if (ds == null)
                    {
                        ds = fds.GetMatchList(monitorFileName, monitorServerID, maxSize,username);
                        //save into session
                        if (ds != null && ds.Count > 0)
                        {
                            ds.SearchPattern = monitorFileName;
                            ds.MonitorServerID = monitorServerID;
                            Session["DownloadSearchResult"] = ds;
                        }
                    }
                    #endregion

                    #region 拼凑html语句
                    jsonData jd = new jsonData();

                    StringBuilder tableTR = new StringBuilder();
                    if (ds != null && ds.Count > 0)
                    {
                        string fileTRFormat = "<tr>" +
                                     "           <td class=\"cel1\" style=\"text-align:center;line-height:28px;\"><input type=\"checkbox\" value='{0}' class=\"filePath\" name=\"monitorFileLocalPath[]\" title=\"{4}\"/></td>" +
                                     "           <td class=\"cel2\">{1}</td>" +
                                     "           <td class=\"cel2\">{2}</td>" +
                                     "           <td class=\"cel3\">{3}</td>" +
                                     //"           <td class=\"cel4\">{4}</td>" +
                                     "       </tr>";
                        #region resolve get the page

                        int min = (pindex - 1) * pagesize;
                        int max = pindex * pagesize-1;

                        int folderListCount = ds.FolderInfoList.Count;
                        //folder フォルダー
                        while (folderListCount > min)
                        {
                            if (min > max) break;

                            budbackup.Models.DFolderInfo dfi = ds.FolderInfoList[min];
                            //dfi.MacPath.Replace(@"\", @"\\") escape
                            tableTR.Append(string.Format(fileTRFormat, dfi.WinPath.Replace(@"\", @"\\"), "フォルダー", dfi.Name, dfi.MacPath.Replace(@"\", @"\\"),min));

                            min++;
                        }
                        
                        //file ファイル
                        if (max > folderListCount)
                        {
                            min -= folderListCount;
                            max -= folderListCount;
                            while (ds.FileInfoList.Count > min)
                            {
                                if (min > max) break;

                                budbackup.Models.DFileInfo dfi = ds.FileInfoList[min];

                                tableTR.Append(string.Format(fileTRFormat, dfi.WinPath.Replace(@"\", @"\\"), "ファイル", dfi.Name, dfi.MacPath.Replace(@"\", @"\\"), min + folderListCount));

                                min++;
                            }
                        }
                        //page element
                        int pageCount = (ds.Count + pagesize - 1)/pagesize;
                        jd.Add("logList", tableTR.ToString())
                          .Add("pageCount", pageCount)
                          .Add("pindex", pindex)
                          .Add("totalCount", ds.Count);

                        #endregion
                    }
                    else
                    {

                         tableTR.Append("<tr><td colspan=\"5\" style=\"text-align:center;line-height:28px;\">データがありません。</td></tr>");
                         jd.Add("logList", tableTR.ToString())
                           .Add("pageCount", 0)
                           .Add("pindex", pindex)
                           .Add("totalCount", 0);
                       
                    }

                    
                    result = jd.ToString();

                    #endregion
                   
                }
                catch (Exception ex)
                {
                    result = "-10";
                    logger.Error(ex.Message);
                }
                #endregion
            }

            Response.Write(result);
            Response.End();

            return null;
        }

        [HttpPost]
        public ActionResult Download() 
        {
            try
            {
               
                //get the path post
                string str = Request.Form["checkFilePath"];
                if (string.IsNullOrWhiteSpace(str))
                {
                    Response.End();
                    return null;
                }

                string zipfilePath = "";//path
                string zipfileName = "";
               

                /**
                * check  session 
                * 
                * if exists then compress the the files to a zip file
                * else resuming download the zip file
                * 
                * xiecongwen 20150112
                * 
                */

                object zipPathHt = Session["zipPathHt"];
                if (zipPathHt != null)
                {
                    Hashtable ht = zipPathHt as Hashtable;
                    if (ht.ContainsKey(str))
                    {
                        zipfilePath = ht[str] as string;
                        goto Download;
                    }
                }
                else
                {
                    Session["zipPathHt"] = new Hashtable();
                }

                
                    #region compress to zip file
                    string[] files = str.Split(',');

                    files = GetString(files);

                    //get the backup server info (xcw 20141202
                    FileDownloadService fds = new FileDownloadService();
                    budbackup.Models.ServerShareInfo ssi = fds.GetSSI();

                    //guarantee the access connection
                    Common.PinvokeWindowsNetworking.connectToRemote(ssi.UNCBase, ssi.Username, ssi.Passwd);

                    //get the uncpath and insert into list
                    //download files from ftp that set——2014-06-04 wjd add
                    List<string> fileFTP = new List<string>();
                    foreach (string f in files)
                    {
                        string uncPath = ssi.UNCBase + f.Substring(f.IndexOf("\\"));//添加IP，在所建立的网络位置上下载

                        fileFTP.Add(uncPath);

                    }

                    #region compress and write it to client
                    if (fileFTP.Count() > 0)
                    {
                        files = fileFTP.ToArray();
                        #region temp Path
                        string tmpPath = ConfigurationManager.AppSettings["TmpPath"];
                        try
                        {
                            if (!Directory.Exists(tmpPath))
                            {
                                Directory.CreateDirectory(tmpPath);
                            }
                            else//删除超过一周的文件
                            {
                                foreach (string file in Directory.GetFiles(tmpPath))
                                {
                                    if ((DateTime.Now - System.IO.File.GetCreationTime(file)).Days >= 7)
                                    {
                                        System.IO.File.Delete(file);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                        string ZipedFileName = tmpPath + "\\" + DateTime.Now.ToFileTime().ToString() + Common.RandomCode.GetCode(1) + ".zip";
                        #endregion

                        ZipIonicHelper.ZipAdvanced(files, ZipedFileName, ssi.UNCBase);
                        zipfilePath = ZipedFileName;//path
                        zipfileName = Path.GetFileName(ZipedFileName);

                        (Session["zipPathHt"] as Hashtable).Add(str, zipfilePath);

                    }
                    else
                    {
                        zipfilePath = files[0];//path
                        zipfileName = Path.GetFileName(files[0]);
                    }
                    #endregion
                    #endregion
                

                Download: //resuming download
                {
                        zipfileName = Path.GetFileName(zipfilePath);
                        if (System.IO.File.Exists(zipfilePath))
                        {
                            Response.Clear();
                            int BUFFERSize = 1024 * 1024 * 2;
                            // Buffer to read 10K bytes in chunk:
                            byte[] buffer = new Byte[BUFFERSize];
                            // Length of the file:
                            int length;

                            // Total bytes to read:
                            long dataToRead;

                            FileStream iStream = new System.IO.FileStream(zipfilePath, System.IO.FileMode.Open,
                                System.IO.FileAccess.Read, System.IO.FileShare.Read);
                            // Total bytes to read:
                             dataToRead = iStream.Length;
                             long p = 0;
                             if (Request.Headers["Range"] != null)
                             {
                                 Response.StatusCode = 206;
                                 p = long.Parse(Request.Headers["Range"].Replace("bytes=", "").Replace("-", ""));
                             }
                             
                             Response.AddHeader("Content-Range", "bytes " + p.ToString() + "-" + ((long)(dataToRead - 1)).ToString() + "/" + dataToRead.ToString());
                             
                             Response.AddHeader("Content-Length", ((long)(dataToRead - p)).ToString());
                             Response.ContentType = "application/octet-stream";
                             Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(Request.ContentEncoding.GetBytes(zipfileName)));
                             //use this to tell client that file is sending
                             String TOKEN = "downloadToken";
                             HttpCookie dtCookie = new HttpCookie(TOKEN);
                             dtCookie.Value = Request.Form[TOKEN];
                             Response.Cookies.Add(dtCookie);

                             iStream.Position = p;
                             dataToRead = dataToRead - p;
                             // Read the bytes.
                             while (dataToRead > 0)
                             {
                                 // Verify that the client is connected.
                                 if (Response.IsClientConnected)
                                 {
                                     // Read the data in buffer.
                                     length = iStream.Read(buffer, 0, BUFFERSize);

                                     // Write the data to the current output stream.
                                     Response.OutputStream.Write(buffer, 0, length);

                                     // Flush the data to the HTML output.
                                     Response.Flush();

                                     buffer = new Byte[BUFFERSize];
                                     dataToRead = dataToRead - length;
                                 }
                                 else
                                 {
                                     //prevent infinite loop if user disconnects
                                     dataToRead = -1;
                                 }
                             }

                             iStream.Close();
                             Response.End();
                             return null;
                        }
                    else
                    {
                        //Session["fileexists"] = "no";
                        return RedirectToAction("Index", "FileDownload");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()+ex.Message);
            }

            return RedirectToAction("Index", "FileDownload");
        }
        protected string GetFileName(string path)
        {
            return path.Substring(path.LastIndexOf('\\'));
        }
        protected string[] GetString(string[] values)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < values.Length; i++)//遍历数组成员
            {
                if (list.IndexOf(values[i]) == -1 && values[i] != "")//对每个成员做一次新数组查询如果没有相等的则加到新数组
                    list.Add(values[i]);

            }
            return list.ToArray();
        }

        /// <summary>
        /// Windows文件无效字符转为“〓全角字符〓”
        /// </summary>
        /// <param name="singleByteChar"></param>
        /// <returns></returns>
        private string ConvertToDoubleByteChar(string singleByteChar)
        {
            //特殊字符替换
            string dbBC = InvalidFileChange(singleByteChar);
            // 濁点/半濁点処理
            dbBC = budbackup.CommonWeb.Japanese.NormalizeSoundSymbol(dbBC);

            return dbBC;
        }

        /// <summary>
        /// ファイルパスの変換
        /// </summary>
        /// <param name="fileName"></param>
        private string InvalidFileChange(string filePath)
        {
            string resultFilePath = "";
            string[] filePathList = filePath.Split(new string[1] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            char[] invalidPathChars = Path.GetInvalidFileNameChars();
            try
            {
                if (filePathList.Count() > 0)
                {
                    for (int i = 0; i < filePathList.Count(); i++)
                    {
                        // 無効な文字のチェック
                        foreach (char someChar in invalidPathChars)
                        {
                            if (filePathList[i].IndexOf(someChar) > -1)
                            {
                                // 全角変換
                                string zenkakuInvalidCharacter = Strings.StrConv(filePathList[i][filePathList[i].IndexOf(someChar)].ToString(), VbStrConv.Wide, 0);
                                filePathList[i] = filePathList[i].Replace(filePathList[i][filePathList[i].IndexOf(someChar)].ToString(), "〓" + zenkakuInvalidCharacter + "〓");
                            }
                            if (filePathList[i].IndexOf(@"'") > -1)
                            {
                                // 全角変換
                                string zenkakuInvalidCharacter = Strings.StrConv(filePathList[i][filePathList[i].IndexOf(@"'")].ToString(), VbStrConv.Wide, 0);
                                filePathList[i] = filePathList[i].Replace(filePathList[i][filePathList[i].IndexOf(@"'")].ToString(), "〓" + zenkakuInvalidCharacter + "〓");
                            }
                        }
                    }
                }
                foreach (string pathSec in filePathList)
                {
                    if (String.IsNullOrEmpty(resultFilePath))
                    {
                        resultFilePath = pathSec;
                    }
                    else
                    {
                        resultFilePath = resultFilePath + @"\" + pathSec;
                    }
                }
                if (String.IsNullOrEmpty(resultFilePath))
                {
                    resultFilePath = filePath;
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(ex.Message);
                resultFilePath = filePath;
            }
            return resultFilePath;
        }
    }
}