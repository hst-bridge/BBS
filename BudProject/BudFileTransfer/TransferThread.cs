using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Model;
using IBLL;
using Common;
using log4net;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BudFileTransfer
{
    public class TransferThread
    {
        /// <summary>
        /// monitorid
        /// </summary>
        private string monitorid;
        /// <summary>
        /// monitorid
        /// </summary>
        private string backupgroupid;
        /// <summary>
        /// Time
        /// </summary>
        private int TimeoutMillis = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["transfertime"]);        
        /// <summary>
        /// Threading.Timer
        /// </summary>
        System.Timers.Timer m_timer = null;
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IMonitorServerService MonitorServerService = BLLFactory.ServiceAccess.CreateMonitorServerService();
        private IBackupServerService BackupServerService = BLLFactory.ServiceAccess.CreateBackupServerService();
        private IMonitorServerFileService MonitorServerFileService = BLLFactory.ServiceAccess.CreateMonitorServerFileService();
        private ILogService LogService = BLLFactory.ServiceAccess.CreateLogService();
        private IBackupServerFileService BackupServerFileService = BLLFactory.ServiceAccess.CreateBackupServerFileService();
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
        /// TransferResultListen
        /// </summary>
        private TransferResultListen trl = null;
        /// <summary>
        /// TransferFileorDel
        /// </summary>
        private TransferFileorDel tfd = null;
        /// <summary>
        /// TransferResultListenTime
        /// </summary>
        private int TransferResultListenMillis = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["TransferResultListenTime"]);
        /// <summary>
        /// TransferResultListenTime
        /// </summary>
        private int TransferResultFileorDelMillis = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["TransferFileorDelTime"]);
        /// <summary>
        /// 文字列のディフォルト値
        /// </summary>
        const string DIFFRESULT = "{\"result\":0,\"dirrsyncs\":[]}";
        // セッションリスト
        List<string> sessionList = new List<string>();
        // 転送情報
        Hashtable sessionConnectList = new Hashtable();
        /// <summary>
        /// 転送configパス
        /// </summary>
        string TransferConfigPath = System.Configuration.ConfigurationManager.AppSettings["ssbpath"];
        /// <summary>
        /// 転送EXEパス
        /// </summary>
        string TransferEXEPath = System.Configuration.ConfigurationManager.AppSettings["ssbput"];
        /// <summary>
        /// minbps
        /// </summary>
        string minbpsset = System.Configuration.ConfigurationManager.AppSettings["minbps"];
        /// <summary>
        /// maxbps
        /// </summary>
        string maxbpsset = System.Configuration.ConfigurationManager.AppSettings["maxbps"];
        /// <summary>
        /// initbps
        /// </summary>
        string initbpsset = System.Configuration.ConfigurationManager.AppSettings["initbps"];
        /// <summary>
        /// SSBTaskMaxNum
        /// </summary>
        int SSBTaskMaxNum = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["SSBTaskMaxNum"]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_monitorid"></param>
        /// <param name="_backupgroupid"></param>
        public TransferThread(string _monitorid, string _backupgroupid)
        {
            monitorid = _monitorid;
            backupgroupid = _backupgroupid;
        }

        /// <summary>
        /// watch
        /// </summary>
        public bool InitWatcher()
        {
            bool result = false;
            try
            {
                trl = new TransferResultListen(monitorid, backupgroupid, TransferResultListenMillis);
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// watch
        /// </summary>
        public bool InitTransferFileorDel()
        {
            bool result = false;
            try
            {
                tfd = new TransferFileorDel(monitorid, backupgroupid, TransferResultFileorDelMillis);
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void TransferStart()
        {
            //time処理の設定、ここで起動していない状態です。
            m_timer = new System.Timers.Timer(TimeoutMillis);
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTansferFile);
            m_timer.Start();
            if (InitWatcher())
            {
                // Begin watching.
                trl.EnableRaisingEvents = true;
            }
            if (InitTransferFileorDel())
            {
                // Begin watching.
                tfd.EnableRaisingEvents = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void TransferStop()
        {
            m_timer.Stop();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTansferFile(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_timer.Enabled = false;
            //バックアップ先list
            IList<BackupServer> BackupServer = BackupServerService.GetGroupBackupServerList(backupgroupid);
            //バックアップ元
            MonitorServer MonitorServer = MonitorServerService.GetMonitorServerById(Int32.Parse(monitorid));
            string strSenderDir = MonitorServer.monitorLocalPath + "\\";
            //バックアップ元のファイル変更list
            IList<MonitorServerFile> monitorServerFileList = MonitorServerFileService.GetMonitorServerFileSSBPutList(monitorid, strSenderDir.TrimEnd('\\'));
            string requestUrl = System.Configuration.ConfigurationManager.AppSettings["ssbapi"];
            string account = System.Configuration.ConfigurationManager.AppSettings["ssbaccount"];
            string password = System.Configuration.ConfigurationManager.AppSettings["ssbpassword"];

            try
            {
                if (Directory.GetDirectories(strSenderDir).Length > 0 || Directory.GetFiles(strSenderDir).Length > 0)
                {
                    foreach (BackupServer backup in BackupServer)
                    {
                        try
                        {
                            SkeedFileTransfer filetransfer = new SkeedFileTransfer(requestUrl, account, password);
                            var jsonSession = filetransfer.connect(backup.backupServerIP);
                            //
                            if (monitorServerFileList.Count > 0)
                            {
                                // linq
                                var dirList = monitorServerFileList.ToList().Distinct(new DirectoryNameComparer());
                                // 転送情報
                                Hashtable transferInfo = new Hashtable();
                                if (dirList.Count() > 0)
                                {
                                    foreach (var dirname in dirList)
                                    {
                                        if (jsonSession.ToString() != string.Empty && jsonSession.result == 0)
                                        {
                                            dynamic jsonResult = filetransfer.doingTransferBatchesList();
                                            if (jsonResult.ToString() != string.Empty && jsonResult.result == 0)
                                            {
                                                try
                                                {
                                                    filetransfer.disconnect(jsonSession.session.sessionId);
                                                }
                                                catch (Exception ex)
                                                {
                                                }
                                                int taskeNum = Regex.Matches(jsonResult.ToString(), "taskId").Count;
                                                if (taskeNum <= SSBTaskMaxNum)
                                                {
                                                    //SSB Upload
                                                    DateTime startTime = DateTime.Now;
                                                    // 削除の場合
                                                    bool transferresult;
                                                    string remotepath = RemotePathConvert(strSenderDir, dirname.monitorFileDirectory, backup.ssbpath);
                                                    transferresult = TransferDir(backup.backupServerIP, dirname.monitorFileDirectory, remotepath, minbpsset, maxbpsset, initbpsset);
                                                    DateTime endTime = DateTime.Now;
                                                    TimeSpan timespan = endTime - startTime;
                                                    TransferResult transferResult = new TransferResult();
                                                    if (transferresult)
                                                    {
                                                        transferResult.OKResult = true;
                                                        MonitorServerFileService.UpdateAllTransferFlg(dirname.monitorFileDirectory, 1);
                                                    }
                                                    else
                                                    {
                                                        transferResult.OKResult = false;
                                                        MonitorServerFileService.UpdateAllTransferFlg(dirname.monitorFileDirectory, 2);
                                                    }
                                                    transferResult.StartTime = startTime;
                                                    transferResult.EndTime = endTime;
                                                    transferResult.TimeSpan = timespan;
                                                    transferInfo.Add(dirname.monitorFileDirectory, transferResult);
                                                }
                                            }
                                        }
                                    }
                                    // ファイル更新
                                    foreach (MonitorServerFile file in monitorServerFileList)
                                    {
                                        try
                                        {
                                            TransferResult dirResult = transferInfo[file.monitorFileDirectory] as TransferResult;
                                            if (dirResult.OKResult)
                                            {
                                                MonitorServerFileService.UpdateTransferFlg(file.id, 1);
                                                //insert log and backupserverfile
                                                //InsertTransferInfo(file, backup, dirResult.StartTime, dirResult.EndTime, dirResult.TimeSpan, 1);
                                            }
                                            else
                                            {
                                                MonitorServerFileService.UpdateTransferFlg(file.id, 2);
                                                //insert log and backupserverfile
                                                //InsertTransferInfo(file, backup, dirResult.StartTime, dirResult.EndTime, dirResult.TimeSpan, 2);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex.Message);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            m_timer.Enabled = true;
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
                Log.backupServerGroupID = Int32.Parse(backupgroupid);
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
                BackupServerFile.backupServerGroupID = Int32.Parse(backupgroupid);
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
            catch(Exception e)
            {
                logger.Error(e.Message);
            }
        }

        private string RemotePathConvert(string srcpath, string objectpath, string remotestartpath)
        {
            string returnFilePath = "";
            try
            {
                string[] mainpath = srcpath.Split('\\');
                string[] subpath = objectpath.Split('\\');
                string dirpath = "";
                bool indexflg = false;
                int mainIndex = mainpath.Length - 1;
                // 一番後ろのパス取得
                for (int j = mainpath.Length - 1; j >= 0; j--)
                {
                    if (!String.IsNullOrEmpty(mainpath[j]))
                    {
                        mainIndex = j;
                        break;
                    }
                }
                // パスの繋がり
                for (int i = 0; i < subpath.Length; i++)
                {
                    if (subpath[i].Equals(mainpath[mainIndex]))
                    {
                        indexflg = true;
                        continue;
                    }

                    if (indexflg)
                    {
                        if (i < subpath.Length - 1)
                        {
                            dirpath += subpath[i] + "\\";
                        }
                    }
                }
                //returnFilePath = remotestartpath + "\\" + dirpath + subpath[subpath.Length - 1];
                returnFilePath = remotestartpath + "\\" + dirpath;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnFilePath;
        }

        private string RelativePathConvert(string srcpath, string objectpath)
        {
            string returnFilePath = "";
            try
            {
                string[] mainpath = srcpath.Split('\\');
                string[] subpath = objectpath.Split('\\');
                string dirpath = "";
                bool indexflg = false;
                int mainIndex = mainpath.Length - 1;
                // 一番後ろのパス取得
                for (int j = mainpath.Length - 1; j >= 0; j--)
                {
                    if (!String.IsNullOrEmpty(mainpath[j]))
                    {
                        mainIndex = j;
                        break;
                    }
                }
                // パスの繋がり
                for (int i = 0; i < subpath.Length; i++)
                {
                    if (subpath[i].Equals(mainpath[mainIndex]))
                    {
                        indexflg = true;
                        continue;
                    }

                    if (indexflg)
                    {
                        if (i < subpath.Length - 1)
                        {
                            dirpath += subpath[i] + "\\";
                        }
                    }
                }
                //returnFilePath = remotestartpath + "\\" + dirpath + subpath[subpath.Length - 1];
                returnFilePath = dirpath;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnFilePath;
        }

        /// <summary>
        /// 転送フォルダー
        /// </summary>
        /// <param name="node"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        /// <param name= "最小レード">minbps</param> 
        /// <param name= "最大レード">maxbps</param> 
        /// <param name= "最初レード">initbps</param> 
        private bool TransferDir(string node, string strSenderDir, string strReceiverDir, string minbps, string maxbps, string initbps)
        {
            bool resultFlg = true;
            try
            {
                // confファイル作成機能
                // 配置ファイル名
                string fileName = RandomCode.GetCode(8) + ".conf";
                string datenowDir = DateTime.Now.ToString("yyyy-MM-dd");
                // 今のプログラムパス
                string configPath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir + "\\" + fileName;
                //string configPath = "C:\\Program Files\\Skeed\\SkeedSilverBullet Service\\utils\\" + datenowDir + "\\" + fileName;
                TransferConfFile tcf = new TransferConfFile(configPath);
                tcf.Write(node, strSenderDir, strReceiverDir, minbps, maxbps, initbps);

                // ファイル転送の機能
                // パラメータを指定して実行
                //ProcessStartInfo psInfo = new ProcessStartInfo();
                //psInfo.FileName = @"""C:\Program Files\Skeed\SkeedSilverBullet Service\utils\ssbput.exe""";
                //psInfo.Arguments = @"--configfile " + "\"" + configPath + "\"";
                //psInfo.CreateNoWindow = false; // コンソール・ウィンドウを開かない
                //psInfo.UseShellExecute = true; // シェル機能を使用しない
                //psInfo.RedirectStandardInput = true;
                //psInfo.RedirectStandardOutput = true;
                //Process.Start(psInfo);
                //StringBuilder dircommand = new StringBuilder();
                //dircommand.Append("ssbput ");
                //dircommand.Append("--configfile ");
                //dircommand.Append("\"" + configPath + "\"");

                Process defaultUserDeskTop = new Process();
                defaultUserDeskTop.StartInfo.FileName = @"C:\\Program Files\\Skeed\SkeedSilverBullet Service\\utils\\ssbput.exe";
                defaultUserDeskTop.StartInfo.Arguments = @"--configfile " + "\"" + configPath + "\"";
                defaultUserDeskTop.StartInfo.UseShellExecute = false;
                defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.Start();
                defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                defaultUserDeskTop.Close();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                resultFlg = false; 
            }
            return resultFlg;
        }
    }
}
