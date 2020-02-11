using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BudCopyListen.Common;
using log4net;
using System.Reflection;
using BudCopyListen.Models;
using BudCopyListen.Entities;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace BudCopyListen
{
    public partial class FrmMain : Form
    {
        /// <summary>
        /// 起動のパラメータ監視ID
        /// </summary>
        private string paraMonitorID;
        /// <summary>
        /// 起動のパラメータ監視IP
        /// </summary>
        private string paraMonitorIP;
        /// <summary>
        /// 起動のパラメータ監視のユーザー
        /// </summary>
        private string paraMonitorUser;
        /// <summary>
        /// 起動のパラメータ監視のパスワード
        /// </summary>
        private string paraMonitorPassword;
        /// <summary>
        /// 起動のパラメータ監視のRootDirectory
        /// </summary>
        private string paraMonitorRootDirectory;
        /// <summary>
        /// 起動のパラメータ監視の元パス
        /// </summary>
        private string paraMonitorSrcPath;
        /// <summary>
        /// 起動のパラメータ監視の先パス
        /// </summary>
        private string paraMonitorTargetPath;
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// コピー監視時間
        /// </summary>
        private string paraCopyListenTime;
        /// <summary>
        /// 転送監視時間
        /// </summary>
        private string paraTransferTime;
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string MTNum = System.Configuration.ConfigurationManager.AppSettings["MTNum"];
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string RNum = System.Configuration.ConfigurationManager.AppSettings["RNum"];
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string WNum = System.Configuration.ConfigurationManager.AppSettings["WNum"];
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private List<int> ProcessIDList = new List<int>();

        public FrmMain(string[] arg)
        {
            InitializeComponent();
            if (arg.Count() > 0)
            {
                paraMonitorID = arg[0];
                paraMonitorIP = arg[1];
                paraMonitorUser = arg[2];
                paraMonitorPassword = arg[3];
                paraMonitorSrcPath = arg[4];
                paraMonitorTargetPath = arg[5];
                paraMonitorRootDirectory = arg[6];
                paraCopyListenTime = arg[7];
                paraTransferTime = arg[8];
                //MessageBox.Show(paraMonitorID + paraMonitorIP + paraMonitorUser + paraMonitorPassword + paraMonitorSrcPath + paraMonitorTargetPath + paraMonitorRootDirectory + paraCopyListenTime + paraTransferTime);
            }
            else
            {
                //paraMonitorID = "1";
                //paraMonitorIP = "192.168.0.7";
                //paraMonitorUser = "wangdan";
                //paraMonitorPassword = "u571@wang";
                //paraMonitorSrcPath = "\\\\192.168.0.7\\TestABC";
                //paraMonitorTargetPath = "C:\\TestFastCopy\\";
                //paraMonitorRootDirectory = "J:";
                //paraCopyListenTime = "1";
                //paraTransferTime = "1";
                MessageBox.Show("パラメータの設定は異常になりました。管理者に連絡してください。");
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Process[] ps = Process.GetProcesses();
            try
            {
                foreach (Process item in ps)
                {
                    if (item.ProcessName.Equals("Robocopy.exe"))
                    {
                        item.Kill();
                    }
                }
                this.Close();
                this.Dispose();
                Application.Exit();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                if (ObjectConnect(paraMonitorSrcPath, paraMonitorRootDirectory, paraMonitorUser, paraMonitorPassword))
                {
                    FileCopyListen(paraMonitorSrcPath, paraMonitorTargetPath);
                }
            }
            catch (Exception exx)
            {
                logger.Error(exx.Message);
            }
        }

        /// <summary>
        /// ファイルの比較
        /// </summary>
        /// <param name="startSrcPath"></param>
        /// <param name="startBakPath"></param>
        private void FileCopyListen(string startSrcPath, string startBakPath)
        {
            MonitorServerFolderManager monitorServerFolderManager = new MonitorServerFolderManager();
            MonitorFileListenManager monitorFileListenManager = new MonitorFileListenManager();
            FileTypeSetManager fileTypeSetManager = new FileTypeSetManager();
            BackupServerGroupManager backupServerGroupManager = new BackupServerGroupManager();
            BackupServerGroupDetailManager backupServerGroupDetailManager = new BackupServerGroupDetailManager();
            BackupServerManager backupServerManager = new BackupServerManager();
            // 転送情報の抽出
            try
            {
                backupServerGroup backupServerGroup = backupServerGroupManager.GetBackupServerGroup(Int32.Parse(paraMonitorID));
                List<backupServerGroupDetail> backupServerGroupDetailList = backupServerGroupDetailManager.GetBackupServerGroupDetailList(backupServerGroup.id);
                // 監視対象の抽出
                List<monitorServerFolder> monitorPathlist = monitorServerFolderManager.GetByMonitorObject(Int32.Parse(paraMonitorID), "99", 1, 0);
                //
                if (monitorPathlist.Count > 0)
                {
                    int monitorPathNum = monitorPathlist.Count;
                    for (int i = 0; i < monitorPathNum; i++)
                    {
                        try
                        {
                            if (String.IsNullOrEmpty(monitorPathlist[i].monitorFileName))
                            {
                                //
                                FileSetInfo fileSetInfo = fileTypeSetManager.CheckFileTypeSet(paraMonitorRootDirectory, Int32.Parse(paraMonitorID), 0);
                                if (fileSetInfo.DirectoryPathList.Count() > 0)
                                {
                                    if (fileSetInfo.DirectoryPathList.Contains(paraMonitorRootDirectory))
                                    {
                                        // ファイル除外リスト
                                        List<string> fileExtensionList = fileSetInfo.DirectoryFileExtensionList[paraMonitorRootDirectory] as List<string>;
                                        // ローカルコピー
                                        CopyListenExclusionFile(startSrcPath.TrimEnd('\\'), startBakPath.TrimEnd('\\'), fileExtensionList, paraCopyListenTime, MTNum, RNum, WNum);
                                        // 
                                        Thread.Sleep(10000);
                                        // 転送コピー
                                        for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                                        {
                                            backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                            string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                            CopyListenExclusionFile(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), fileExtensionList, paraTransferTime, MTNum, RNum, WNum);
                                        }
                                    }
                                    else
                                    {
                                        // ローカルコピー
                                        CopyListen(startSrcPath.TrimEnd('\\'), startBakPath.TrimEnd('\\'), paraCopyListenTime, MTNum, RNum, WNum);
                                        // 
                                        Thread.Sleep(10000);
                                        // 転送コピー
                                        for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                                        {
                                            backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                            string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                            CopyListen(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), paraTransferTime, MTNum, RNum, WNum);
                                        }
                                    }
                                }
                                else
                                {
                                    // ローカルコピー
                                    CopyListen(startSrcPath.TrimEnd('\\'), startBakPath.TrimEnd('\\'), paraCopyListenTime, MTNum, RNum, WNum);
                                    // 
                                    Thread.Sleep(10000);
                                    // 転送コピー
                                    for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                                    {
                                        backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                        string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                        CopyListen(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), paraTransferTime, MTNum, RNum, WNum);
                                    }
                                }
                                break;
                            }
                            else
                            {
                                // コピー元
                                string objectPath = Path.Combine(monitorPathlist[i].monitorFilePath + "\\", monitorPathlist[i].monitorFileName);
                                string copySrcPath = PathFileConvert(paraMonitorRootDirectory, startSrcPath, objectPath);
                                // コピー先
                                string copyTargetPath = PathFileConvert(paraMonitorRootDirectory, startBakPath, objectPath);
                                //
                                FileSetInfo fileSetInfo = fileTypeSetManager.CheckFileTypeSet(objectPath, Int32.Parse(paraMonitorID), 0);
                                if (fileSetInfo.DirectoryPathList.Count() > 0)
                                {
                                    // フォルダー除外リスト
                                    List<string> folderExtensionList = fileSetInfo.DirectoryPathList;
                                    // 監視元の除外フォルダーリスト
                                    List<string> folderPathConvertExtensionList = new List<string>();
                                    // コピー先の除外フォルダーリスト
                                    List<string> folderPathConvertExtensionForLocalList = new List<string>();
                                    if (folderExtensionList.Count > 0)
                                    {
                                        foreach (string folderExtensionPath in folderExtensionList)
                                        {
                                            if (!folderExtensionPath.Equals(objectPath))
                                            {
                                                if (!String.IsNullOrEmpty(RelativePathFileConvert(folderExtensionPath)))
                                                {
                                                    folderPathConvertExtensionList.Add(RelativePathFileConvert(folderExtensionPath));
                                                    folderPathConvertExtensionForLocalList.Add(RelativePathFileConvert(folderExtensionPath));
                                                }
                                            }
                                        }
                                    }
                                    //
                                    if (fileSetInfo.DirectoryPathList.Contains(objectPath))
                                    {
                                        // ファイル除外リスト
                                        List<string> fileExtensionList = fileSetInfo.DirectoryFileExtensionList[objectPath] as List<string>;
                                        // ローカルコピー
                                        CopyListenExclusionFolderFile(copySrcPath.TrimEnd('\\'), copyTargetPath.TrimEnd('\\'), folderPathConvertExtensionList, fileExtensionList, paraCopyListenTime, MTNum, RNum, WNum);
                                        // 
                                        Thread.Sleep(10000);
                                        // 転送コピー
                                        for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                                        {
                                            backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                            string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                            // 転送先
                                            string TransferTargetPath = PathFileConvert(startBakPath, TransferCopyPath, copyTargetPath);
                                            CopyListenExclusionFolderFile(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), folderPathConvertExtensionForLocalList, fileExtensionList,
                                                paraTransferTime, MTNum, RNum, WNum);
                                        }
                                        //
                                        // 特別処理
                                        foreach (string dirPathKey in fileSetInfo.DirectoryPathList)
                                        {
                                            if (!objectPath.Equals(dirPathKey))
                                            {
                                                // コピー元
                                                string copyExtensionSrcPath = PathFileConvert(paraMonitorRootDirectory, startSrcPath, dirPathKey);
                                                // コピー先
                                                string copyExtensionTargetPath = PathFileConvert(paraMonitorRootDirectory, startBakPath, dirPathKey);
                                                //
                                                List<string> fileSubExtensionList = fileSetInfo.DirectoryFileExtensionList[dirPathKey] as List<string>;
                                                // ローカルコピー
                                                CopyListenExclusionFile(copyExtensionSrcPath.TrimEnd('\\'), copyExtensionTargetPath.TrimEnd('\\'), fileSubExtensionList,
                                                    paraCopyListenTime, MTNum, RNum, WNum);
                                                // 
                                                Thread.Sleep(10000);
                                                // 転送コピー
                                                for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                                                {
                                                    backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                                    string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                                    // 転送先
                                                    string TransferTargetPath = PathFileConvert(startBakPath, TransferCopyPath, copyExtensionTargetPath);
                                                    CopyListenExclusionFile(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), fileSubExtensionList,
                                                        paraTransferTime, MTNum, RNum, WNum);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // 特別処理
                                        foreach (string dirPathKey in fileSetInfo.DirectoryPathList)
                                        {
                                            // コピー元
                                            string copyExtensionSrcPath = PathFileConvert(paraMonitorRootDirectory, startSrcPath, dirPathKey);
                                            // コピー先
                                            string copyExtensionTargetPath = PathFileConvert(paraMonitorRootDirectory, startBakPath, dirPathKey);
                                            //
                                            List<string> fileExtensionList = fileSetInfo.DirectoryFileExtensionList[dirPathKey] as List<string>;
                                            // ローカルコピー
                                            CopyListenExclusionFile(copyExtensionSrcPath.TrimEnd('\\'), copyExtensionTargetPath.TrimEnd('\\'), fileExtensionList,
                                                paraCopyListenTime, MTNum, RNum, WNum);
                                            // 
                                            Thread.Sleep(10000);
                                            // 転送コピー
                                            for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                                            {
                                                backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                                string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                                // 転送先
                                                string TransferTargetPath = PathFileConvert(startBakPath, TransferCopyPath, copyExtensionTargetPath);
                                                CopyListenExclusionFile(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), fileExtensionList,
                                                    paraTransferTime, MTNum, RNum, WNum);
                                            }
                                        }
                                        //
                                        // ローカルコピー
                                        // ファイル除外リスト
                                        List<string> fileExtensionNullList = new List<string>();
                                        CopyListenExclusionFolderFile(copySrcPath.TrimEnd('\\'), copyTargetPath.TrimEnd('\\'), folderPathConvertExtensionList, fileExtensionNullList, paraCopyListenTime, MTNum, RNum, WNum);
                                        // 
                                        Thread.Sleep(10000);
                                        // 転送コピー
                                        for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                                        {
                                            backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                            string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                            // 転送先
                                            string TransferTargetPath = PathFileConvert(startBakPath, TransferCopyPath, copyTargetPath);
                                            CopyListenExclusionFolderFile(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), folderPathConvertExtensionForLocalList, fileExtensionNullList,
                                                paraTransferTime, MTNum, RNum, WNum);
                                        }
                                    }
                                }
                                else
                                {
                                    // ローカルコピー
                                    CopyListen(copySrcPath.TrimEnd('\\'), copyTargetPath.TrimEnd('\\'), paraCopyListenTime, MTNum, RNum, WNum);
                                    // 
                                    Thread.Sleep(10000);
                                    // 転送コピー
                                    for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                                    {
                                        backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                        string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                        // 転送先
                                        string TransferTargetPath = PathFileConvert(startBakPath, TransferCopyPath, copyTargetPath);
                                        CopyListen(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), paraTransferTime, MTNum, RNum, WNum);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Error(monitorPathlist[i].monitorFilePath + e.Message);
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        /// <summary>
        /// IP接続
        /// </summary>
        private bool ObjectConnect(string MonitorSrcPath, string MonitorRootDirectory, string MonitorUser, string MonitorPassword)
        {
            bool connectResult = false;
            try
            {
                //接続テスト
                bool result = true;
                int connectTime = 0;
                while (result)
                {
                    int status = NetworkConnection.Connect(paraMonitorSrcPath, paraMonitorRootDirectory, paraMonitorUser, paraMonitorPassword);
                    if (status == (int)ERROR_ID.ERROR_SUCCESS)
                    {
                        result = false;
                        connectTime = 0;
                        connectResult = true;
                    }
                    else
                    {
                        if (connectTime > 20)
                        {
                            NetworkConnection.Disconnect(paraMonitorSrcPath);
                            logger.Info("[" + paraMonitorIP + "]は接続できません");
                            result = false;
                        }
                        connectTime++;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return connectResult;
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        /// <param name="listentime"></param>
        /// <param name="threadnum"></param>
        /// <param name="retrynum"></param>
        /// <param name="retrytime"></param>
        void CopyListen(string srcpath, string exportpath, string listentime, string threadnum, string retrynum, string retrytime)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // 配置ファイル名
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-" + RandomCode.GetCode(10) + ".log";
                string datenowDir = DateTime.Now.ToString("yyyy-MM-dd");
                // 今のログパス
                string logpath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir + "\\" + fileName;
                LogFile(logpath);
                // コマンド処理
                dircommand.Append("robocopy ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\" ");
                dircommand.Append(" /MIR ");
                dircommand.Append(" /MOT:" + listentime + " ");
                dircommand.Append(" /MT[:" + threadnum + "] ");
                dircommand.Append(" /R:" + retrynum + " ");
                dircommand.Append(" /W:" + retrytime + " ");
                dircommand.Append(" /LOG+:");
                dircommand.Append("\"" + logpath + "\"");
                dircommand.Append(" /NP /NDL /TEE /XJD /XJF ");
                // BAT作成
                string batFileName = RandomCode.GetCode(10) + ".bat";
                string batDir = DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                // 今のプログラムパス
                string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                CreateBAT batClass = new CreateBAT(batPath);
                batClass.Write(dircommand.ToString());
                //// プロセス設定
                //defaultUserDeskTop.StartInfo.UserName = "BudAdmin";
                //System.Security.SecureString password = new System.Security.SecureString();
                //char[] pass = "BudBk@96".ToCharArray();
                //foreach (char c in pass)
                //{
                //    password.AppendChar(c);
                //}
                //defaultUserDeskTop.StartInfo.Password = password;
                //
                defaultUserDeskTop.StartInfo.FileName = "cmd.exe";
                defaultUserDeskTop.StartInfo.UseShellExecute = false;
                defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (defaultUserDeskTop.Start()) 
                {
                    defaultUserDeskTop.StandardInput.WriteLine(dircommand);
                    ProcessIDList.Add(defaultUserDeskTop.Id);
                }
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        /// <param name="exclusionFolderList"></param>
        /// <param name="exclusionFileList"></param>
        /// <param name="listentime"></param>
        /// <param name="threadnum"></param>
        /// <param name="retrynum"></param>
        /// <param name="retrytime"></param>
        void CopyListenExclusionFolderFile(string srcpath, string exportpath, List<string> exclusionFolderList, List<string> exclusionFileList,
            string listentime, string threadnum, string retrynum, string retrytime)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // 配置ファイル名
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-" + RandomCode.GetCode(8) + ".log";
                string datenowDir = DateTime.Now.ToString("yyyy-MM-dd");
                // 今のログパス
                string logpath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir + "\\" + fileName;
                LogFile(logpath);
                //
                dircommand.Append("robocopy ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\" ");
                dircommand.Append(" /MIR ");
                dircommand.Append(" /MOT:" + listentime + " ");
                dircommand.Append(" /MT[:" + threadnum + "] ");
                // フォルダー除外
                if (exclusionFolderList.Count > 0)
                {
                    string folderExclusionArray = "";
                    foreach (string folderpath in exclusionFolderList)
                    {
                        folderExclusionArray = folderExclusionArray + folderpath + " ";
                    }
                    dircommand.Append(" /XD " + folderExclusionArray);
                }
                // ファイル除外
                if (exclusionFileList.Count > 0)
                {
                    string fileExclusionArray = "";
                    foreach (string folderpath in exclusionFileList)
                    {
                        fileExclusionArray = fileExclusionArray + "*" + folderpath + " ";
                    }
                    dircommand.Append(" /XF " + fileExclusionArray);
                }
                //
                dircommand.Append(" /R:" + retrynum + " ");
                dircommand.Append(" /W:" + retrytime + " ");
                dircommand.Append(" /LOG+:");
                dircommand.Append("\"" + logpath + "\"");
                dircommand.Append(" /NP /NDL /TEE /XJD /XJF ");
                // BAT作成
                //string batFileName = RandomCode.GetCode(10) + ".bat";
                //string batDir = DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                //// 今のプログラムパス
                //string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                //CreateBAT batClass = new CreateBAT(batPath);
                //batClass.Write(dircommand.ToString());
                // プロセス設定
                defaultUserDeskTop.StartInfo.FileName = "cmd.exe";
                defaultUserDeskTop.StartInfo.UseShellExecute = false;
                defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //
                if (defaultUserDeskTop.Start())
                {
                    defaultUserDeskTop.StandardInput.WriteLine(dircommand);
                    ProcessIDList.Add(defaultUserDeskTop.Id);
                }
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        /// <param name="exclusionFileList"></param>
        /// <param name="listentime"></param>
        /// <param name="threadnum"></param>
        /// <param name="retrynum"></param>
        /// <param name="retrytime"></param>
        void CopyListenExclusionFile(string srcpath, string exportpath, List<string> exclusionFileList,
            string listentime, string threadnum, string retrynum, string retrytime)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // 配置ファイル名
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-" + RandomCode.GetCode(8) + ".log";
                string datenowDir = DateTime.Now.ToString("yyyy-MM-dd");
                // 今のログパス
                string logpath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir + "\\" + fileName;
                LogFile(logpath);
                dircommand.Append("robocopy ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\" ");
                dircommand.Append(" /MIR ");
                dircommand.Append(" /MOT:" + listentime + " ");
                dircommand.Append(" /MT[:" + threadnum + "] ");
                // ファイル除外
                if (exclusionFileList.Count > 0)
                {
                    string fileExclusionArray = "";
                    foreach (string folderpath in exclusionFileList)
                    {
                        fileExclusionArray = fileExclusionArray + "*" + folderpath + " ";
                    }
                    dircommand.Append(" /XF " + fileExclusionArray);
                }
                dircommand.Append(" /R:" + retrynum + " ");
                dircommand.Append(" /W:" + retrytime + " ");
                dircommand.Append(" /LOG+:");
                dircommand.Append("\"" + logpath + "\"");
                dircommand.Append(" /NP /NDL /TEE /XJD /XJF ");
                // BAT作成
                //string batFileName = RandomCode.GetCode(10) + ".bat";
                //string batDir = DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                //// 今のプログラムパス
                //string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                //CreateBAT batClass = new CreateBAT(batPath);
                //batClass.Write(dircommand.ToString());
                // プロセス設定
                defaultUserDeskTop.StartInfo.FileName = "cmd.exe";
                defaultUserDeskTop.StartInfo.UseShellExecute = false;
                defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (defaultUserDeskTop.Start())
                {
                    defaultUserDeskTop.StandardInput.WriteLine(dircommand);
                    ProcessIDList.Add(defaultUserDeskTop.Id);
                }
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="targetpath"></param>
        /// <param name="objectpath"></param>
        private string PathFileConvert(string srcpath, string targetpath, string objectpath)
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
                returnFilePath = Path.Combine(Path.Combine(targetpath, dirpath), subpath[subpath.Length - 1]);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnFilePath;
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="objectpath"></param>
        private string RelativePathFileConvert(string objectpath)
        {
            string returnFilePath = "";
            try
            {
                string[] subpath = objectpath.Split('\\');
                if (subpath.Count() > 0)
                {
                    returnFilePath = subpath[subpath.Length - 1];
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnFilePath;
        }


        /// <summary> 
        /// Logファイル
        /// </summary> 
        /// <param   name= "FilePath "> </param> 
        public void LogFile(string FilePath)
        {
            FileInfo fileInfo = new FileInfo(FilePath);
            try
            {
                if (fileInfo.Exists)
                {
                    File.Delete(FilePath);
                }
                else
                {
                    if (!Directory.Exists(fileInfo.DirectoryName))
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
