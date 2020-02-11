using System;
using System.IO;
using System.Collections;
using System.Threading;
using log4net;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using IBLL;
using Model;
using Common;

namespace BudFileTransfer
{
    public class TransferFileorDel
    {
        /// <summary>
        /// monitorid
        /// </summary>
        private string _monitorid;
        /// <summary>
        /// monitorid
        /// </summary>
        private string _backupgroupid;
        /// <summary>
        /// 時間コンポーネント
        /// </summary>
        System.Timers.Timer m_timer = null;
        /// <summary>
        /// コンポーネント起動判断
        /// </summary>
        public bool EnableRaisingEvents { get; set; }
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// MonitorServerFileService
        /// </summary>
        private IMonitorServerFileService MonitorServerFileService = BLLFactory.ServiceAccess.CreateMonitorServerFileService();
        /// <summary>
        /// BackupServerService
        /// </summary>
        private IBackupServerService BackupServerService = BLLFactory.ServiceAccess.CreateBackupServerService();
        /// <summary>
        /// MonitorServerService
        /// </summary>
        private IMonitorServerService MonitorServerService = BLLFactory.ServiceAccess.CreateMonitorServerService();
        /// <summary>
        /// LogService
        /// </summary>
        private ILogService LogService = BLLFactory.ServiceAccess.CreateLogService();
        /// <summary>
        /// BackupServerFileService
        /// </summary>
        private IBackupServerFileService BackupServerFileService = BLLFactory.ServiceAccess.CreateBackupServerFileService();
        /// <summary>
        /// TransferNum
        /// </summary>
        private int transferNum = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["TransferNum"]);
        /// <summary>
        /// 文字列のディフォルト値
        /// </summary>
        const string DEFAULTCHAR_VALUE = "";
        /// <summary>
        /// 日付時間フィールドのディフォルト値
        /// </summary>
        const string DEFAULTDATETIME_VALUE = "1900-01-01 00:00:00.000";
        /// <summary>
        /// 数字フィールドのディフォルト値
        /// </summary>
        const int DEFAULTINT_VALUE = 0;

        /// <summary>
        /// 初期化
        /// </summary>
        public TransferFileorDel(string monitorid, string backupgroupid, int timecount)
        {
            //time処理の設定、ここで起動していない状態です。
            _monitorid = monitorid;
            _backupgroupid = backupgroupid;
            m_timer = new System.Timers.Timer(timecount);
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(WatchStart);
            m_timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatchStart(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (EnableRaisingEvents)
                {
                    m_timer.Enabled = false;
                    TransferMain();
                    m_timer.Enabled = true;
                }
            }
            catch (Exception exb)
            {
                logger.Error(exb.Message);
                m_timer.Enabled = true;
            }
        }

        /// <summary>
        /// 転送ファイルのチェック
        /// </summary>
        /// <param name="dirs"></param>
        private void TransferMain()
        {
            //バックアップ先list
            IList<BackupServer> BackupServer = BackupServerService.GetGroupBackupServerList(_backupgroupid);
            //バックアップ元
            MonitorServer MonitorServer = MonitorServerService.GetMonitorServerById(Int32.Parse(_monitorid));
            string strSenderDir = MonitorServer.monitorLocalPath + "\\";
            //バックアップ元のファイル変更list
            IList<MonitorServerFile> monitorServerFileForDelList = MonitorServerFileService.GetMonitorServerFileDelList(_monitorid);
            IList<MonitorServerFile> monitorServerFileForTopDirList = MonitorServerFileService.GetMonitorServerFileTopDirList(_monitorid, strSenderDir.TrimEnd('\\'));
            string requestUrl = System.Configuration.ConfigurationManager.AppSettings["ssbapi"];
            string account = System.Configuration.ConfigurationManager.AppSettings["ssbaccount"];
            string password = System.Configuration.ConfigurationManager.AppSettings["ssbpassword"];

            foreach (BackupServer backup in BackupServer)
            {
                try
                {
                    SkeedFileTransfer filetransfer = new SkeedFileTransfer(requestUrl, account, password);
                    var jsonSession = filetransfer.connect(backup.backupServerIP);

                    if (jsonSession.ToString() != string.Empty && jsonSession.result == 0)
                    {
                        //批量传输
                        string strFileName = "";
                        string fileId = "";
                        string strDelFileName = "";
                        string fileDelId = "";
                        foreach (MonitorServerFile file in monitorServerFileForDelList)
                        {
                            strDelFileName = strDelFileName + "&files=./" + file.monitorFilePath.Replace(strSenderDir, "");
                            fileDelId = fileDelId + file.id + ",";
                        }
                        foreach (MonitorServerFile file in monitorServerFileForTopDirList)
                        {
                            strFileName = strFileName + "&files=" + file.monitorFilePath;
                            fileId = fileId + file.id + ",";
                        }
                        if (strDelFileName != "")
                        {
                            //SSB Delete
                            DateTime startTime = DateTime.Now;
                            dynamic jsonResult = filetransfer.deleteFiles(jsonSession.session.sessionId, strSenderDir, backup.startFile, strDelFileName.TrimStart('&'));
                            DateTime endTime = DateTime.Now;
                            TimeSpan timespan = endTime - startTime;
                            if (jsonResult.ToString() != string.Empty && jsonResult.result == 0)
                            {
                                foreach (MonitorServerFile file in monitorServerFileForDelList)
                                {
                                    try
                                    {
                                        //更新传输flg
                                        MonitorServerFileService.UpdateTransferFlg(file.id, 1);
                                        //insert log and backupserverfile
                                        //InsertTransferInfo(file, backup, startTime, endTime, timespan, 1);
                                        //logger.Info("["+file.monitorFilePath + "] is deleted to [" + backup.backupServerIP + "] OK");
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex.Message);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    foreach (MonitorServerFile file in monitorServerFileForDelList)
                                    {
                                        //更新传输flg
                                        MonitorServerFileService.UpdateTransferFlg(file.id, 2);
                                        //insert log and backupserverfile
                                        //InsertTransferInfo(file, backup, startTime, endTime, timespan, 2);
                                        //logger.Info("[" + file.monitorFilePath + "] is deleted to [" + backup.backupServerIP + "] NG");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.Message);
                                    continue;
                                }
                            }
                        }
                        if (strFileName != "")
                        {
                            //SSB Upload
                            DateTime startTime = DateTime.Now;
                            dynamic jsonResult = filetransfer.transferFiles(jsonSession.session.sessionId, strSenderDir, strFileName);
                            DateTime endTime = DateTime.Now;
                            TimeSpan timespan = endTime - startTime;
                            if (jsonResult.ToString() != string.Empty && jsonResult.result == 0)
                            {
                                foreach (MonitorServerFile file in monitorServerFileForTopDirList)
                                {
                                    try
                                    {
                                        //更新传输flg
                                        MonitorServerFileService.UpdateTransferFlg(file.id, 1);
                                        //insert log and backupserverfile
                                        //InsertTransferInfo(file, backup, startTime, endTime, timespan, 1);
                                        //logger.Info("[" + file.monitorFilePath + "] is uploaded to [" + backup.backupServerIP + "] OK");    
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex.Message);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                foreach (MonitorServerFile file in monitorServerFileForTopDirList)
                                {
                                    try
                                    {
                                        //更新传输flg
                                        MonitorServerFileService.UpdateTransferFlg(file.id, 2);
                                        //insert log and backupserverfile
                                        //InsertTransferInfo(file, backup, startTime, endTime, timespan, 2);
                                        //logger.Info("[" + file.monitorFilePath + "] is uploaded to [" + backup.backupServerIP + "] NG");
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex.Message);
                                        continue;
                                    }
                                }
                            }
                        }                  
                        var jsonSession2 = filetransfer.disconnect(jsonSession.session.sessionId);
                    }
                    else
                    {
                        logger.Error("[" + backup.backupServerIP + "]は接続できません");
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// insert log and backupserverfile
        /// </summary>
        /// <param name="file"></param>
        /// <param name="backup"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timespan"></param>
        /// <param name="transferFlg"></param>
        private void InsertTransferInfo(MonitorServerFile file, BackupServer backup, DateTime startTime, DateTime endTime, TimeSpan timespan, int transferFlg)
        {
            //insert log
            try
            {
                Log Log = new Log();
                Log.monitorServerID = file.monitorServerID;
                Log.monitorFileName = file.monitorFileName;
                Log.monitorFilePath = file.monitorFilePath;
                Log.monitorFileType = file.monitorFileType;
                Log.monitorFileSize = file.monitorFileSize;
                Log.monitorTime = file.monitorStartTime;
                Log.transferFlg = transferFlg;
                Log.backupServerGroupID = Int32.Parse(_backupgroupid);
                Log.backupServerID = Int32.Parse(backup.id);
                Log.backupServerFileName = file.monitorFileName;
                Log.backupServerFilePath = backup.startFile;
                Log.backupServerFileType = file.monitorFileType;
                Log.backupServerFileSize = file.monitorFileSize;
                Log.backupStartTime = startTime.ToString();
                Log.backupEndTime = endTime.ToString();
                Log.backupTime = timespan.TotalMilliseconds.ToString() + "ms";
                Log.backupFlg = transferFlg;
                Log.copyStartTime = startTime.ToString();
                Log.copyEndTime = endTime.ToString();
                Log.copyTime = timespan.TotalMilliseconds.ToString() + "ms";
                Log.copyFlg = transferFlg;
                Log.deleteFlg = DEFAULTINT_VALUE;
                Log.deleter = DEFAULTCHAR_VALUE;
                Log.deleteDate = DEFAULTDATETIME_VALUE;
                Log.creater = "exe";
                Log.createDate = CommonUtil.DateTimeNowToString();
                Log.updater = "exe";
                Log.updateDate = CommonUtil.DateTimeNowToString();
                Log.restorer = DEFAULTCHAR_VALUE;
                Log.restoreDate = DEFAULTDATETIME_VALUE;
                LogService.InsertLog(Log);
                //insert backupserverfile
                BackupServerFile BackupServerFile = new BackupServerFile();
                BackupServerFile.backupServerGroupID = Int32.Parse(_backupgroupid);
                BackupServerFile.backupServerID = Int32.Parse(backup.id);
                BackupServerFile.backupServerFileName = file.monitorFileName;
                BackupServerFile.backupServerFilePath = backup.startFile;
                BackupServerFile.backupServerFileType = file.monitorFileType;
                BackupServerFile.backupServerFileSize = file.monitorFileSize;
                BackupServerFile.backupStartTime = startTime.ToString();
                BackupServerFile.backupEndTime = endTime.ToString();
                BackupServerFile.backupTime = timespan.TotalMilliseconds.ToString() + "ms";
                BackupServerFile.backupFlg = transferFlg;
                BackupServerFile.copyStartTime = startTime.ToString();
                BackupServerFile.copyEndTime = endTime.ToString();
                BackupServerFile.copyTime = timespan.TotalMilliseconds.ToString() + "ms";
                BackupServerFile.copyFlg = transferFlg;
                BackupServerFile.deleteFlg = DEFAULTINT_VALUE;
                BackupServerFile.deleter = DEFAULTCHAR_VALUE;
                BackupServerFile.deleteDate = DEFAULTDATETIME_VALUE;
                BackupServerFile.creater = "exe";
                BackupServerFile.createDate = CommonUtil.DateTimeNowToString();
                BackupServerFile.updater = "exe";
                BackupServerFile.updateDate = CommonUtil.DateTimeNowToString();
                BackupServerFile.restorer = DEFAULTCHAR_VALUE;
                BackupServerFile.restoreDate = DEFAULTDATETIME_VALUE;
                BackupServerFileService.InsertBackupServerFile(BackupServerFile);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }
    }
}
