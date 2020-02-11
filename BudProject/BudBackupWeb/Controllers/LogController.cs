using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using IBLL;
using budbackup.CommonWeb;
using log4net;
using System.Text.RegularExpressions;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

namespace budbackup.Controllers
{
    public class LogController : BaseController
    {
        //
        // GET: /Log/
        private readonly ILogService logService = BLLFactory.ServiceAccess.CreateLogService();
        private readonly IBackupServerGroupService groupService = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
        private readonly IBackupServerService backupServerService = BLLFactory.ServiceAccess.CreateBackupServerService();
        private readonly ITransferLogService transferLogService = BLLFactory.ServiceAccess.CreateTransferLogService();
        private readonly ILog logger = LogManager.GetLogger(typeof(LogController));
        /// <summary>
        /// メインページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                ViewData["groupList"] = groupService.GetBackupServerGroupList();
                //ViewData["backupServerList"] = backupServerService.GetBackupServerList();
            }
            catch (Exception ex)
            {
                ViewData["groupList"] = new List<BackupServerGroup>();
                logger.Error(ex.Message);
            }
            return View();
        }

        /// <summary>
        /// 絞り込み
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="displayFlg"></param>
        /// <param name="transferFlg"></param>
        /// <param name="stateFlg"></param>
        /// <param name="logFlg"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReadLog(string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int transferFlg, int stateFlg, int logFlg, string name)
        {
            string strResult = string.Empty;
            IList<Log> logList = new List<Log>();
            IList<TransferLog> transferLogList = new List<TransferLog>();
            try
            {
                tmStart = startTimeFormat(tmStart);
                tmEnd = endTimeFormat(tmEnd);
                if (logFlg == 0)
                {
                    //logList = logService.GetLogListByProc(groupId, dtStart, dtEnd, tmStart, tmEnd, displayFlg, transferFlg, stateFlg, logFlg);
                    logList = logService.GetLogListByProc(groupId, dtStart, dtEnd, tmStart, tmEnd, displayFlg, transferFlg, stateFlg, logFlg, name);
                    if (logList != null && logList.Count > 0)
                    {
                        strResult = JsonHelper.GetJson<IList<Log>>(logList);
                    }
                }
                else
                {
                    transferLogList = transferLogService.GetTransferLogList(groupId, dtStart, dtEnd, tmStart, tmEnd, name,
                        stateFlg);
                    if (transferLogList != null && transferLogList.Count > 0)
                    {
                        strResult = JsonHelper.GetJson<IList<TransferLog>>(transferLogList);
                    }
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

        [HttpPost]
        public ActionResult ReadConLog(int pindex, int pagesize, string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int transferFlg, int stateFlg, int logFlg, string name)
        {
            string strResult = string.Empty;
            IList<Log> logList = new List<Log>();
            IList<TransferLog> transferLogList = new List<TransferLog>();
            try
            {
                tmStart = startTimeFormat(tmStart);
                tmEnd = endTimeFormat(tmEnd);
                if (logFlg == 0)
                {
                    //logList = logService.GetLogListByProc(groupId, dtStart, dtEnd, tmStart, tmEnd, displayFlg, transferFlg, stateFlg, logFlg);
                    logList = logService.GetLogListByProc(pindex, pagesize, groupId, dtStart, dtEnd, tmStart, tmEnd, displayFlg, transferFlg, stateFlg, logFlg, name);
                    int totalCount = 0;
                    foreach(Log l in logList){
                        totalCount = l.totalCount;
                        break;
                    }
                    if (logList != null && logList.Count > 0)
                    {

                        string tableTR = "";
                        string logTR = "";
                        logTR = "<tr><td class=\"cel1\">@trId</td>" +
                                    "           <td class=\"cel2\"><div style=\"width:200px;word-break:break-all; word-wrap:break-word;overflow: auto;\">@backupServerFileName</div></td>" +
                                    "           <td class=\"cel3\"><div style=\"width:400px;word-break:break-all; word-wrap:break-word;overflow: auto;\">@backupServerFilePath</div></td>" +
                                    "           <td class=\"cel4\">@backupServerFileSize Byte</td>" +
                                    //"           <td class=\"cel5\">@copyStartTime</td>" +
                                    //"           <td class=\"cel6\">@copyEndTime</td>" +
                                    "           <td class=\"cel7\">@backupStartTime</td>" +
                                    //"           <td class=\"cel8\">@backupEndTime</td>" +
                                    "           <td class=\"cel9\" style=\"display: none;\">@backupTime秒</td>" +
                                    "           <td class=\"cel10\">@state</td>" +
                                    "           <td class=\"cel11\">@status</td>" +
                                    "       </tr>";
                        string tr;
                        for (int i = 0; i < logList.Count; i++) {
                            logList[i].monitorFilePath = logList[i].monitorFilePath.Replace("\\","\\\\");
                            tr = logTR;
                            tr = tr.Replace("@backupServerFileSize", logList[i].backupServerFileSize);
                            //tr = tr.Replace("@copyStartTime", logList[i].copyStartTime);
                            //tr = tr.Replace("@copyEndTime", logList[i].copyEndTime);
                            tr = tr.Replace("@backupStartTime", logList[i].backupStartTime);
                            //tr = tr.Replace("@backupEndTime", logList[i].backupEndTime);
                            tr = tr.Replace("@backupTime", logList[i].backupTime);
                            tr = tr.Replace("@trId", (i + 1 + (pindex - 1) * pagesize).ToString());
                            tr = tr.Replace("@state", logList[i].backupFlg == 1 ? "OK" : "NG");
                            tr = tr.Replace("@backupServerFileName", logList[i].monitorFileName);
                            tr = tr.Replace("@backupServerFilePath", logList[i].monitorFilePath);
                            tr = tr.Replace("@status", logList[i].monitorFileStatus);
                            tableTR = tableTR + tr;
                        }

                        //strResult = JsonHelper.GetJson<IList<Log>>(logList);
                        int pageCount = (int)Math.Ceiling((double)totalCount / pagesize);
                        Object obj = new Object();
                        jsonData jd = new jsonData();
                        jd.Add("logList", tableTR)
                           .Add("pageCount", pageCount)
                           .Add("pindex", pindex)
                           .Add("totalCount", totalCount);

                        strResult = jd.ToString();
                    }
                }
                else
                {
                    transferLogList = transferLogService.GetTransferLogListByProc(pindex, pagesize, groupId, dtStart, dtEnd, tmStart, tmEnd, displayFlg, transferFlg, stateFlg, logFlg, name);
                    if (transferLogList != null && transferLogList.Count > 0)
                    {
                        strResult = JsonHelper.GetJson<IList<TransferLog>>(transferLogList);
                        
                    }
                }
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
        /// start time format
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        private string startTimeFormat(string timeStr) 
        {
            Regex reg = new Regex("^[0-2]{1}[0-6]{1}:[0-5]{1}[0-9]{1}");
            Regex reg2 = new Regex("^[0-2]{1}[0-6]{1}");
            Regex reg3 = new Regex("^[0-9]{1}");
            Regex reg4 = new Regex("^[0-2]{1}[0-6]{1}:[0-5]{1}[0-9]{1}:[0-5]{1}[0-9]{1}");
            if (timeStr != "")
            {
                if (!reg4.Match(timeStr).Success)
                {
                    if (reg.Match(timeStr).Success)
                    {
                        timeStr += ":00";
                    }
                    else if (reg2.Match(timeStr).Success)
                    {
                        timeStr += ":00:00";
                    }
                    else if (reg3.Match(timeStr).Success && timeStr.Length == 1)
                    {
                        timeStr = "0" + timeStr + ":00:00";
                    }
                }
            }
            return timeStr;
        }

        /// <summary>
        /// end time format
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        private string endTimeFormat(string timeStr)
        {
            Regex reg = new Regex("^[0-2]{1}[0-6]{1}:[0-5]{1}[0-9]{1}");
            Regex reg2 = new Regex("^[0-2]{1}[0-6]{1}");
            Regex reg3 = new Regex("^[0-9]{1}");
            Regex reg4 = new Regex("^[0-2]{1}[0-6]{1}:[0-5]{1}[0-9]{1}:[0-5]{1}[0-9]{1}");
            if (timeStr != "")
            {
                if (!reg4.Match(timeStr).Success)
                {
                    if (reg.Match(timeStr).Success)
                    {
                        timeStr += ":59";
                    }
                    else if (reg2.Match(timeStr).Success)
                    {
                        timeStr += ":59:59";
                    }
                    else if (reg3.Match(timeStr).Success && timeStr.Length == 1)
                    {
                        timeStr = "0" + timeStr + ":59:59";
                    }
                }
            }
            return timeStr;
        }
    }
}
