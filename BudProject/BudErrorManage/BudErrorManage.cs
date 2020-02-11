using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using System.IO;
using BudErrorManage.Models;
using BudErrorManage.Entities;
using System.Text.RegularExpressions;
using System.Diagnostics;
using BudErrorManage.Common;
using System.Threading;
using System.Security.Principal;
using System.Security.AccessControl;
using Microsoft.VisualBasic;
using BudErrorManage.SSH;
using System.Collections;
using Maverick.SSH;
using Maverick.SFTP;
using BudErrorManage.BLL;

namespace BudErrorManage
{
    public partial class BudErrorManage : Form
    {
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// SynchronizingTime
        /// </summary>
        private int SynchronizingTime = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["SynchronizingTime"]);
        /// <summary>
        /// LogPath
        /// </summary>
        private string logPath = System.Configuration.ConfigurationManager.AppSettings["LogPath"];
        /// <summary>
        /// backupServerGroupManager
        /// </summary>
        BackupServerGroupManager backupServerGroupManager = new BackupServerGroupManager();
        /// <summary>
        /// monitorServerManager
        /// </summary>
        MonitorServerManager monitorServerManager = new MonitorServerManager();
        /// <summary>
        /// 文字列のディフォルト値
        /// </summary>
        const string DEFAULTCHAR_VALUE = "";
        /// <summary>
        /// 日付時間フィールドのディフォルト値
        /// </summary>
        DateTime DEFAULTDATETIME_VALUE = new DateTime(1900, 1, 1, 0, 0, 0);
        /// <summary>
        /// 検索時間
        /// </summary>
        DateTime dateTimeForSearch;
        /// <summary>
        /// 数字フィールドのディフォルト値
        /// </summary>
        const int DEFAULTINT_VALUE = 0;
        /// <summary>
        /// 削除ログファイルリスト(削除エラー、次に再度削除)
        /// </summary>
        private List<string> DelFileForErrorDelList = new List<string>();
        /// <summary>
        /// パス処理済の判断リスト
        /// </summary>
        private List<string> errorDirDoneList = new List<string>();

        public BudErrorManage()
        {
            InitializeComponent();
        }

        private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            Application.Exit();
        }

        private void BudErrorManage_Load(object sender, EventArgs e)
        {
            try
            {
                //
                timerErrorManage.Interval = SynchronizingTime;
                timerErrorManage.Elapsed += new System.Timers.ElapsedEventHandler(SynchronizingErrorDo);
                timerErrorManage.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 同期化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SynchronizingErrorDo(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // 同期のTIMER中止
                timerErrorManage.Enabled = false;
                // エラー削除執行
                foreach (string errorFilePath in DelFileForErrorDelList)
                {
                    FileDelete(errorFilePath);
                }
                #region get file list
                // 昨日と一昨日のファイル処理
                DirectoryInfo yesterdayLogDir = new DirectoryInfo(logPath + @"\" + "RoboCopyLogToCopyBK-" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                if (yesterdayLogDir.Exists)
                {
                    // ファイルリスト
                    FileInfo[] yesterdayLogFileInfoList = yesterdayLogDir.GetFiles("*.log", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo yesterdayLogFileInfo in yesterdayLogFileInfoList)
                    {
                        try
                        {
                            string yesterdayLogFileBKPath = Path.Combine(
                                logPath + @"\" + "RoboCopyLogToCopyBK-" + DateTime.Now.ToString("yyyy-MM-dd"),
                                yesterdayLogFileInfo.FullName.Substring(yesterdayLogFileInfo.FullName.LastIndexOf(@"\") + 1));
                            yesterdayLogFileInfo.CopyTo(yesterdayLogFileBKPath, true);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                            continue;
                        }
                    }
                    //
                    DeleteFile(yesterdayLogDir.FullName);
                }
                //
                string copyLogPath = logPath + @"\" + "RoboCopyLogToCopyBK-" + DateTime.Now.ToString("yyyy-MM-dd"); ;
                //
                DirectoryInfo copyLogDir = new DirectoryInfo(copyLogPath);
                //
                IEnumerable<FileInfo> copyLogList = copyLogDir.GetFiles("*.log", SearchOption.TopDirectoryOnly);
                #endregion
                if (copyLogList.Count() > 0)
                {
                    string testdd = "";
                    foreach (FileInfo fileInfo in copyLogList)
                    {
                        try
                        {
                            string[] txtAllLines = System.IO.File.ReadAllLines(@fileInfo.FullName, Encoding.GetEncoding(932));
                            //
                            string searchSrcPathKey = @"コピー元";
                            IEnumerable<string> srcPathList = RunQuery(txtAllLines, searchSrcPathKey);
                            if (srcPathList.Count() > 0)
                            {
                                string[] srcPathLineArray = srcPathList.First().Split(':');
                                monitorServer errorForMonitorServer = LogToMonitorServer(srcPathLineArray[1].Trim());
                                //
                                if (errorForMonitorServer != null)
                                {
                                    string monitorSrcPath = @"\\" + errorForMonitorServer.monitorServerIP + @"\" + errorForMonitorServer.startFile.TrimStart('\\');
                                    string monitorTargetPath = errorForMonitorServer.monitorLocalPath;
                                    string strIPAddress = errorForMonitorServer.monitorServerIP;
                                    string strLoginUser = errorForMonitorServer.account;
                                    string strLoginPass = errorForMonitorServer.password;
                                    string macSrcPath = errorForMonitorServer.monitorMacPath;
                                    backupServerGroup errorForBackupServerGroup = backupServerGroupManager.GetBackupServerGroup(errorForMonitorServer.id);
                                    //
                                    string searchKey = @"エラー";
                                    //
                                    IEnumerable<string> errorInfoList = RunQuery(txtAllLines, searchKey);
                                    if (errorInfoList.Count() > 0)
                                    {
                                        // 初期化
                                        errorDirDoneList.Clear();
                                        //
                                        foreach (string errorline in errorInfoList)
                                        {
                                            #region
                                            // エラーファイルパス
                                            string errorFilePath = "";
                                            string[] lineArray = errorline.Split(' ');
                                            if (lineArray.Count() == 7)
                                            {
                                                errorFilePath = lineArray[6];
                                            }
                                            else if (lineArray.Count() > 7)
                                            {
                                                errorFilePath = lineArray[6];
                                                for (int i = 7; i < lineArray.Count(); i++)
                                                {
                                                    errorFilePath = errorFilePath + " " + lineArray[7];
                                                }
                                            }
                                            // パス情報
                                            if (!String.IsNullOrEmpty(errorFilePath))
                                            {
                                                //
                                                testdd = errorFilePath;
                                                //if ((errorFilePath.IndexOf(@"\") > -1) && (errorFilePath.IndexOf(".") > -1))
                                                if ((errorFilePath.IndexOf(@"\") > -1))
                                                {
                                                    #region
                                                    string errorFileDir = errorFilePath.Substring(0, errorFilePath.LastIndexOf(@"\"));
                                                    string errorFileName = errorFilePath.Substring(errorFilePath.LastIndexOf(@"\") + 1);
                                                    string errorFileNameExtension = "";
                                                    //
                                                    // フォルダーの場合
                                                    if (!String.IsNullOrEmpty(errorFileDir) && String.IsNullOrEmpty(errorFileName))
                                                    {
                                                        if (errorFileDir.IndexOf(strIPAddress) > -1)
                                                        {
                                                            if (!errorDirDoneList.Contains(errorFileDir))
                                                            {
                                                                #region
                                                                errorDirDoneList.Add(errorFileDir);
                                                                //
                                                                int InvlidDirNum = errorFileDir.Split('?').Count() - 1;
                                                                if (InvlidDirNum == 0)
                                                                {
                                                                    SSHCopy(strIPAddress, strLoginUser, strLoginPass, monitorSrcPath, errorFileDir, macSrcPath, monitorTargetPath,
                                                                        errorForMonitorServer, errorForBackupServerGroup, errorFileDir);
                                                                }
                                                                else
                                                                {
                                                                    string invalidSrcDirPath = errorFileDir.Substring(0, errorFilePath.IndexOf(@"?"));
                                                                    if (invalidSrcDirPath.IndexOf(@"\") > -1)
                                                                    {
                                                                        string errorParentDir = invalidSrcDirPath.Substring(0, invalidSrcDirPath.LastIndexOf(@"\"));
                                                                        if ((errorParentDir.IndexOf(strIPAddress) > -1))
                                                                        {
                                                                            SSHCopy(strIPAddress, strLoginUser, strLoginPass, monitorSrcPath, errorParentDir, macSrcPath, monitorTargetPath,
                                                                                errorForMonitorServer, errorForBackupServerGroup, errorFileDir);
                                                                        }
                                                                    }
                                                                }
                                                                #endregion
                                                            }
                                                        }
                                                    }
                                                    // 異常Character情報
                                                    if (!String.IsNullOrEmpty(errorFileDir) && !String.IsNullOrEmpty(errorFileName))
                                                    {
                                                        if (errorFilePath.IndexOf(strIPAddress) > -1)
                                                        {
                                                            if (String.Compare(errorFileName.Trim('\r'), ".DS_Store", true) != 0
                                                                && String.Compare(errorFileName.Trim('\r'), ".com.apple.timemachine.supported", true) != 0
                                                                && String.Compare(errorFileName.Trim('\r'), "Icon", true) != 0)
                                                            {
                                                                if (!errorDirDoneList.Contains(errorFileDir))
                                                                {
                                                                    #region
                                                                    //
                                                                    errorDirDoneList.Add(errorFileDir);
                                                                    // 文字化けの数
                                                                    int InvlidDirNum = errorFileDir.Split('?').Count() - 1;
                                                                    int InvlidFileNameNum = errorFileName.Split('?').Count() - 1;
                                                                    //
                                                                    if (InvlidDirNum == 0)
                                                                    {
                                                                        // 無用ファイルの判断
                                                                        if (String.Compare(errorFileName.Trim('\r'), ".DS_Store", true) != 0
                                                                            && String.Compare(errorFileName.Trim('\r'), ".com.apple.timemachine.supported", true) != 0
                                                                            && String.Compare(errorFileName.Trim('\r'), "Icon", true) != 0)
                                                                        {
                                                                            #region
                                                                            try
                                                                            {
                                                                                //
                                                                                if (errorFileName.IndexOf(".") > -1)
                                                                                {
                                                                                    errorFileNameExtension = errorFileName.Substring(errorFileName.IndexOf(@".") + 1);
                                                                                }
                                                                                else
                                                                                {
                                                                                    errorFileNameExtension = "";
                                                                                }
                                                                                // ファイルコピー
                                                                                SSHCopyOnlyFile(strIPAddress, strLoginUser, strLoginPass, monitorSrcPath, errorFilePath, macSrcPath, monitorTargetPath,
                                                                                    InvlidFileNameNum, errorForMonitorServer, errorForBackupServerGroup, errorFileDir, errorFileNameExtension);
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                logger.Error(ex.Message);
                                                                                continue;
                                                                            }
                                                                            #endregion
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        string invalidSrcDirPath = errorFileDir.Substring(0, errorFilePath.IndexOf(@"?"));
                                                                        if (invalidSrcDirPath.IndexOf(@"\") > -1)
                                                                        {
                                                                            string errorParentDir = invalidSrcDirPath.Substring(0, invalidSrcDirPath.LastIndexOf(@"\"));
                                                                            if ((errorParentDir.IndexOf(strIPAddress) > -1))
                                                                            {
                                                                                SSHCopy(strIPAddress, strLoginUser, strLoginPass, monitorSrcPath, errorParentDir, macSrcPath, monitorTargetPath,
                                                                                    errorForMonitorServer, errorForBackupServerGroup, errorFileDir);
                                                                            }
                                                                        }
                                                                    }
                                                                    #endregion
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            // ファイル削除
                            FileDelete(@fileInfo.FullName);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(testdd + ex.Message);
                            continue;
                        }
                    }
                }

                // 同期のTIMER起動
                timerErrorManage.Enabled = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                timerErrorManage.Enabled = true;
            }
        }

        /// <summary> 
        /// ファイル削除
        /// </summary> 
        /// <param   name= "DirPath "> </param> 
        public void DeleteFile(string DirPath)
        {
            try
            {
                string pattern = "*.log";
                string[] strFileName = Directory.GetFiles(DirPath, pattern);
                foreach (var item in strFileName)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// SSHのコピー
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="loginUser"></param>
        /// <param name="loginPass"></param>
        /// <param name="srcPath"></param>
        /// <param name="objectPath"></param>
        /// <param name="macPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="errorMonitorServer"></param>
        /// <param name="errorBackupServerGroup"></param>
        /// <param name="errorFileDir"></param>
        void SSHCopy(string ipAddress, string loginUser, string loginPass, 
            string srcPath, string objectPath, string macPath, string targetPath,
            monitorServer errorMonitorServer, backupServerGroup errorBackupServerGroup, string errorFileDir)
        {
            SSHClient ssh = null;
            SFTPClient sftp = null;
            var sshlog = new Common.Util.SSHLogManager();
            try
            {
                SSHConnector con = SSHConnector.Create();
                ssh = con.Connect(new TcpClientTransport(ipAddress, 22), loginUser, true);

                PasswordAuthentication pwd = new PasswordAuthentication();
                pwd.Password = loginPass;

                // Authenticate the user
                if (ssh.Authenticate(pwd) == AuthenticationResult.COMPLETE)
                {
                    // Open up a session and do something..
                    sftp = new SFTPClient(ssh);

                    try
                    {
                        //
                        MacDirInfo targetDirInfo = MacDirInfoConvert(srcPath, objectPath, macPath);
                        //
                        SFTPFile[] sftpFileInfoList = sftp.Ls(targetDirInfo.DirectoryPath);
                        //
                        if (sftpFileInfoList != null && sftpFileInfoList.Count() > 0)
                        {
                            #region traverse
                            //
                            foreach (SFTPFile sftpFileInfo in sftpFileInfoList)
                            {
                                try
                                {
                                    if (String.Compare(sftpFileInfo.Filename.Trim('\r'), ".DS_Store", true) != 0
                                        && String.Compare(sftpFileInfo.Filename.Trim('\r'), ".com.apple.timemachine.supported", true) != 0
                                        && String.Compare(sftpFileInfo.Filename.Trim('\r'), "Icon", true) != 0)
                                    {
                                        DateTime dateM = Time_T2DateTime((uint)sftpFileInfo.Attributes.ModifiedTime);
                                        DateTime dateA = Time_T2DateTime((uint)sftpFileInfo.Attributes.AccessedTime);
                                        string targetFilePath = PathConvert(sftpFileInfo.AbsolutePath, macPath, targetPath);
                                        string targetFileConvert = InvalidFileChange(targetFilePath);
                                        // 濁点/半濁点処理
                                        targetFileConvert = Japanese.NormalizeSoundSymbol(targetFileConvert);
                                        FileInfo targetFile = new FileInfo(targetFileConvert);
                                        if (!Directory.Exists(targetFile.DirectoryName))
                                        {
                                            Directory.CreateDirectory(targetFile.DirectoryName);
                                        }
                                        #region
                                        try
                                        {
                                            if (!File.Exists(targetFile.FullName))
                                            {
                                                sftp.GetFile(sftpFileInfo.AbsolutePath, targetFile.FullName, true);
                                                if (targetFile.Exists)
                                                {
                                                    targetFile.CreationTime = dateM;
                                                    targetFile.LastWriteTime = dateM;
                                                    targetFile.LastAccessTime = dateA;
                                                }
                                                else
                                                {
                                                    #region
                                                    string errorFileNameExtension = "";
                                                    if (sftpFileInfo.Filename.IndexOf(".") > -1)
                                                    {
                                                        errorFileNameExtension = sftpFileInfo.Filename.Substring(sftpFileInfo.Filename.IndexOf(@".") + 1);
                                                    }
                                                    Thread.Sleep(2000);
                                                    bool copyconfirm = true;
                                                    int confirmCoount = 0;
                                                    while (copyconfirm)
                                                    {
                                                        #region
                                                        if (confirmCoount < 3)
                                                        {
                                                            if (targetFile.Exists)
                                                            {
                                                                DateTime fileFirstTime = targetFile.LastWriteTime;
                                                                Thread.Sleep(2000);
                                                                DateTime fileSecondTime = targetFile.LastWriteTime;
                                                                if (fileFirstTime.Equals(fileSecondTime))
                                                                {
                                                                    targetFile.CreationTime = dateM;
                                                                    targetFile.LastWriteTime = dateM;
                                                                    targetFile.LastAccessTime = dateA;
                                                                    copyconfirm = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Thread.Sleep(2000);
                                                            }
                                                            confirmCoount++;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                        #endregion
                                                    }
                                                    if (copyconfirm)
                                                    {
                                                        string errorTargetToSrcFile = Japanese.NormalizeSoundSymbol(TargetToSrcPathFileConvert(targetPath, srcPath, targetFilePath));
                                                        string errorSrcDir = errorTargetToSrcFile.Substring(0, errorTargetToSrcFile.LastIndexOf(@"\"));
                                                        InsertTransferInfo(errorMonitorServer, errorBackupServerGroup, errorSrcDir, targetFile.Name, errorFileNameExtension, 0);
                                                    }
                                                    #endregion
                                                }
                                            }

                                            sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Success, Message = targetFile.FullName });
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex.Message);
                                            sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = errorMonitorServer.monitorServerIP + ", " + sftpFileInfo.AbsolutePath });
                                            string errorFileNameExtension = "";
                                            if (sftpFileInfo.Filename.IndexOf(".") > -1)
                                            {
                                                errorFileNameExtension = sftpFileInfo.Filename.Substring(sftpFileInfo.Filename.IndexOf(@".") + 1);
                                            }
                                            string errorTargetToSrcFile = Japanese.NormalizeSoundSymbol(TargetToSrcPathFileConvert(targetPath, srcPath, targetFilePath));
                                            string errorSrcDir = errorTargetToSrcFile.Substring(0, errorTargetToSrcFile.LastIndexOf(@"\"));
                                            InsertTransferInfo(errorMonitorServer, errorBackupServerGroup, errorSrcDir, targetFile.Name, errorFileNameExtension, 0);
                                            continue;
                                        }
                                        #endregion
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = errorMonitorServer.monitorServerIP + ", " + sftpFileInfo.AbsolutePath });
                                    logger.Error(sftpFileInfo.AbsolutePath + ex.Message);
                                    continue;
                                }
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = objectPath });
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                if (sftp != null && !sftp.IsClosed)
                {
                    sftp.Quit();
                }
                if (ssh != null && ssh.IsConnected)
                {
                    ssh.Disconnect();
                }
            }
           
        }

        /// <summary>
        /// SSHのコピー
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="loginUser"></param>
        /// <param name="loginPass"></param>
        /// <param name="srcPath"></param>
        /// <param name="objectPath"></param>
        /// <param name="macPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="InvlidFileNameNum"></param>
        /// <param name="errorMonitorServer"></param>
        /// <param name="errorBackupServerGroup"></param>
        /// <param name="errorFileDir"></param>
        /// <param name="errorFileNameExtension"></param>
        void SSHCopyOnlyFile(string ipAddress, string loginUser, string loginPass, string srcPath, 
            string objectPath, string macPath, string targetPath, int InvlidFileNameNum,
            monitorServer errorMonitorServer, backupServerGroup errorBackupServerGroup, string errorFileDir, string errorFileNameExtension)
        {
            SFTPProxy sftpProxy = null;
            var sshlog = new Common.Util.SSHLogManager();
            try
            {
                sftpProxy = new SFTPProxy(ipAddress, loginUser, loginPass);

                    MacFileInfo targetFileInfo = MacFileInfoConvert(srcPath, objectPath, macPath);
                    //
                    SFTPFile[] sftpTopFileInfoList = sftpProxy.TopFileLs(targetFileInfo.DirectoryPath);
                    //
                    SFTPFile[] sftpTopDirInfoList = sftpProxy.TopDirLs(targetFileInfo.DirectoryPath);
                    //
                    //bool fileFlg = false;
                    // ファイル判断
                    #region ファイル判断
                    if (sftpTopFileInfoList != null && sftpTopFileInfoList.Count() > 0)
                    {
                        foreach (SFTPFile sftpFileInfo in sftpTopFileInfoList)
                        {
                            try
                            {
                                if (String.Compare(sftpFileInfo.Filename.Trim('\r'), ".DS_Store", true) != 0
                                    && String.Compare(sftpFileInfo.Filename.Trim('\r'), ".com.apple.timemachine.supported", true) != 0
                                    && String.Compare(sftpFileInfo.Filename.Trim('\r'), "Icon", true) != 0)
                                {
                                    //
                                    DateTime dateM = Time_T2DateTime((uint)sftpFileInfo.Attributes.ModifiedTime);
                                    DateTime dateA = Time_T2DateTime((uint)sftpFileInfo.Attributes.AccessedTime);
                                    string targetFilePath = PathConvert(sftpFileInfo.AbsolutePath, macPath, targetPath);
                                    string targetFileConvert = InvalidFileChange(targetFilePath);
                                    // 濁点/半濁点処理
                                    targetFileConvert = Japanese.NormalizeSoundSymbol(targetFileConvert);
                                    FileInfo targetFile = new FileInfo(targetFileConvert);
                                    if (!Directory.Exists(targetFile.DirectoryName))
                                    {
                                        Directory.CreateDirectory(targetFile.DirectoryName);
                                    }
                                    #region
                                    try
                                    {
                                        if (!File.Exists(targetFile.FullName))
                                        {
                                            sftpProxy.GetFile(sftpFileInfo.AbsolutePath, targetFile.FullName, true);
                                            if (targetFile.Exists)
                                            {
                                                targetFile.CreationTime = dateM;
                                                targetFile.LastWriteTime = dateM;
                                                targetFile.LastAccessTime = dateA;
                                            }
                                            else
                                            {
                                                #region
                                                Thread.Sleep(2000);
                                                bool copyconfirm = true;
                                                int confirmCoount = 0;
                                                while (copyconfirm)
                                                {
                                                    #region
                                                    if (confirmCoount < 3)
                                                    {
                                                        if (targetFile.Exists)
                                                        {
                                                            DateTime fileFirstTime = targetFile.LastWriteTime;
                                                            Thread.Sleep(2000);
                                                            DateTime fileSecondTime = targetFile.LastWriteTime;
                                                            if (fileFirstTime.Equals(fileSecondTime))
                                                            {
                                                                targetFile.CreationTime = dateM;
                                                                targetFile.LastWriteTime = dateM;
                                                                targetFile.LastAccessTime = dateA;
                                                                copyconfirm = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Thread.Sleep(2000);
                                                        }
                                                        confirmCoount++;
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                    #endregion
                                                }
                                                if (copyconfirm)
                                                {
                                                    string errorTargetToSrcFile = Japanese.NormalizeSoundSymbol(TargetToSrcPathFileConvert(targetPath, srcPath, targetFilePath));
                                                    string errorSrcDir = errorTargetToSrcFile.Substring(0, errorTargetToSrcFile.LastIndexOf(@"\"));
                                                    InsertTransferInfo(errorMonitorServer, errorBackupServerGroup, errorSrcDir, targetFile.Name, errorFileNameExtension, 0);
                                                }
                                                #endregion
                                            }
                                        }

                                        //成功的
                                        sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Success, Message = targetFile.FullName });
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex.Message);
                                        string errorTargetToSrcFile = Japanese.NormalizeSoundSymbol(TargetToSrcPathFileConvert(targetPath, srcPath, targetFilePath));
                                        string errorSrcDir = errorTargetToSrcFile.Substring(0, errorTargetToSrcFile.LastIndexOf(@"\"));

                                        //ssh log 
                                        sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = errorTargetToSrcFile });

                                        InsertTransferInfo(errorMonitorServer, errorBackupServerGroup, errorSrcDir, targetFile.Name, errorFileNameExtension, 0);
                                        continue;
                                    }
                                    #endregion
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(sftpFileInfo.AbsolutePath + ex.Message);
                                continue;
                            }
                        }
                    }
                    #endregion
                    // フォルダ判断
                    //if (!fileFlg)
                    //{
                    #region フォルダ判断
                    if (sftpTopDirInfoList != null && sftpTopDirInfoList.Count() > 0)
                    {
                        foreach (SFTPFile sftpFileInfo in sftpTopDirInfoList)
                        {
                            try
                            {
                                //此处从代理中一部分一部分的获取
                                foreach (SFTPFile sftpDirToFileInfo in sftpProxy.Ls(sftpFileInfo))
                                {
                                    #region
                                    try
                                    {
                                        if (String.Compare(sftpDirToFileInfo.Filename.Trim('\r'), ".DS_Store", true) != 0
                                            && String.Compare(sftpDirToFileInfo.Filename.Trim('\r'), ".com.apple.timemachine.supported", true) != 0
                                            && String.Compare(sftpDirToFileInfo.Filename.Trim('\r'), "Icon", true) != 0)
                                        {
                                            DateTime dateM = Time_T2DateTime((uint)sftpDirToFileInfo.Attributes.ModifiedTime);
                                            DateTime dateA = Time_T2DateTime((uint)sftpDirToFileInfo.Attributes.AccessedTime);
                                            string targetFilePath = PathConvert(sftpDirToFileInfo.AbsolutePath, macPath, targetPath);
                                            string targetFileConvert = InvalidFileChange(targetFilePath);
                                            // 濁点/半濁点処理
                                            targetFileConvert = Japanese.NormalizeSoundSymbol(targetFileConvert);
                                            FileInfo targetFile = new FileInfo(targetFileConvert);
                                            if (!Directory.Exists(targetFile.DirectoryName))
                                            {
                                                Directory.CreateDirectory(targetFile.DirectoryName);
                                            }

                                            #region
                                            try
                                            {
                                                if (!File.Exists(targetFile.FullName))
                                                {
                                                    sftpProxy.GetFile(sftpDirToFileInfo.AbsolutePath, targetFile.FullName, true);
                                                    if (targetFile.Exists)
                                                    {
                                                        targetFile.CreationTime = dateM;
                                                        targetFile.LastWriteTime = dateM;
                                                        targetFile.LastAccessTime = dateA;
                                                    }
                                                    else
                                                    {
                                                        #region
                                                        string errorDirToFileNameExtension = "";
                                                        if (sftpDirToFileInfo.Filename.IndexOf(".") > -1)
                                                        {
                                                            errorDirToFileNameExtension = sftpDirToFileInfo.Filename.Substring(sftpDirToFileInfo.Filename.IndexOf(@".") + 1);
                                                        }
                                                        Thread.Sleep(2000);
                                                        bool copyconfirm = true;
                                                        int confirmCoount = 0;
                                                        while (copyconfirm)
                                                        {
                                                            #region
                                                            if (confirmCoount < 3)
                                                            {
                                                                if (targetFile.Exists)
                                                                {
                                                                    DateTime fileFirstTime = targetFile.LastWriteTime;
                                                                    Thread.Sleep(2000);
                                                                    DateTime fileSecondTime = targetFile.LastWriteTime;
                                                                    if (fileFirstTime.Equals(fileSecondTime))
                                                                    {
                                                                        targetFile.CreationTime = dateM;
                                                                        targetFile.LastWriteTime = dateM;
                                                                        targetFile.LastAccessTime = dateA;
                                                                        copyconfirm = false;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Thread.Sleep(2000);
                                                                }
                                                                confirmCoount++;
                                                            }
                                                            else
                                                            {
                                                                break;
                                                            }
                                                            #endregion
                                                        }
                                                        if (copyconfirm)
                                                        {
                                                            string errorTargetToSrcFile = Japanese.NormalizeSoundSymbol(TargetToSrcPathFileConvert(targetPath, srcPath, targetFilePath));
                                                            string errorSrcDir = errorTargetToSrcFile.Substring(0, errorTargetToSrcFile.LastIndexOf(@"\"));
                                                            InsertTransferInfo(errorMonitorServer, errorBackupServerGroup, errorSrcDir, targetFile.Name, errorDirToFileNameExtension, 0);
                                                        }
                                                        #endregion
                                                    }
                                                }
                                                sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Success, Message = targetFile.FullName });
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error(ex.Message);
                                                sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = errorMonitorServer.monitorServerIP + ", " + sftpDirToFileInfo.AbsolutePath });
                                                string errorDirToFileNameExtension = "";
                                                if (sftpDirToFileInfo.Filename.IndexOf(".") > -1)
                                                {
                                                    errorDirToFileNameExtension = sftpDirToFileInfo.Filename.Substring(sftpDirToFileInfo.Filename.IndexOf(@".") + 1);
                                                }
                                                string errorTargetToSrcFile = Japanese.NormalizeSoundSymbol(TargetToSrcPathFileConvert(targetPath, srcPath, targetFilePath));
                                                string errorSrcDir = errorTargetToSrcFile.Substring(0, errorTargetToSrcFile.LastIndexOf(@"\"));

                                                InsertTransferInfo(errorMonitorServer, errorBackupServerGroup, errorSrcDir, targetFile.Name, errorDirToFileNameExtension, 0);
                                                continue;
                                            }
                                            #endregion


                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = errorMonitorServer.monitorServerIP + ", " + sftpDirToFileInfo.AbsolutePath });
                                        logger.Error(sftpFileInfo.AbsolutePath + ex.Message);
                                        continue;
                                    }
                                    #endregion
                                }

                            }
                            catch (Exception ex)
                            {
                                logger.Error(sftpFileInfo.AbsolutePath + ex.Message);
                                continue;
                            }
                        }
                    }
                    #endregion
                    //}
            }
            catch (Exception ex)
            {
               sshlog.WriteLog(new Common.Util.SSHLog() { DateTime=DateTime.Now, LogType= Common.Util.SSHLogType.Failure, Message=objectPath });
               logger.Error(ex.Message);
            }
            finally
            {
                if (sftpProxy != null)
                {
                    sftpProxy.Close();
                }
                
            }
        }

        /// <summary>
        /// 時間変換関数
        /// </summary>
        /// <param name="time_t"></param>
        public static DateTime Time_T2DateTime(uint time_t)
        {
            long win32FileTime = 10000000 * (long)time_t + 116444736000000000;
            return DateTime.FromFileTimeUtc(win32FileTime).ToLocalTime();
        }

        string Utf8Convert(string srcstr)
        {
            byte[] buffer = Encoding.Default.GetBytes(srcstr);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// ファイルパスの変換
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="objectpath"></param>
        /// <param name="macpath"></param>
        private MacFileInfo MacFileInfoConvert(string srcpath, string objectpath, string macpath)
        {
            MacFileInfo returnFileInfo = new MacFileInfo();
            try
            {
                string[] mainpath = srcpath.Split(new string[1] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                string[] subpath = objectpath.Split(new string[1] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
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
                            dirpath += subpath[i] + "/";
                        }
                    }
                }
                returnFileInfo.FilePath = macpath + "/" + dirpath + subpath[subpath.Length - 1];
                returnFileInfo.DirectoryPath = macpath + "/" + dirpath.TrimEnd('/');
                returnFileInfo.FileName = subpath[subpath.Length - 1];
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnFileInfo;
        }

        /// <summary>
        /// フォルダーパスの変換
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="objectpath"></param>
        /// <param name="macpath"></param>
        private MacDirInfo MacDirInfoConvert(string srcpath, string objectpath, string macpath)
        {
            MacDirInfo returnDirInfo = new MacDirInfo();
            try
            {
                string[] mainpath = srcpath.Split(new string[1] {@"\"}, StringSplitOptions.RemoveEmptyEntries);
                string[] subpath = objectpath.Split(new string[1] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
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
                            dirpath += subpath[i] + "/";
                        }
                    }
                }
                returnDirInfo.DirectoryPath = macpath + "/" + dirpath + subpath[subpath.Length - 1];
                returnDirInfo.TopDirectoryPath = macpath + "/" + dirpath.TrimEnd('/');
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnDirInfo;
        }


        /// <summary>
        /// フォルダーパスの変換
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="macPath"></param>
        /// <param name="targetPath"></param>
        private string PathConvert(string srcPath, string macPath, string targetPath)
        {
            string returnFilePath = "";
            try
            {
                string[] mainpath = macPath.Split(new string[1] {@"/"}, StringSplitOptions.RemoveEmptyEntries);
                string[] subpath = srcPath.Split(new string[1] {@"/"}, StringSplitOptions.RemoveEmptyEntries);
                string dirpath = "";
                bool indexflg = false;
                for (int i = 0; i < subpath.Length; i++)
                {
                    if (subpath[i].Equals(mainpath[mainpath.Length - 1]))
                    {
                        indexflg = true;
                        continue;
                    }

                if (indexflg)
                    {
                        if (i < subpath.Length - 1)
                        {
                            dirpath += subpath[i] + @"\";
                        }
                    }
                }
                returnFilePath = targetPath + @"\" + dirpath + subpath[subpath.Length - 1];
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
        /// <param name="srcpath"></param>
        /// <param name="targetpath"></param>
        /// <param name="objectpath"></param>
        private string TargetToSrcPathFileConvert(string srcpath, string targetpath, string objectpath)
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
        /// ファイルパスの変換
        /// </summary>
        /// <param name="fileName"></param>
        string InvalidFileChange(string filePath)
        {
            string resultFilePath = "";
            string[] filePathList = filePath.Split(new string[1] {@"\"}, StringSplitOptions.RemoveEmptyEntries);
            char[] invalidPathChars = Path.GetInvalidFileNameChars();
            try
            {
                if (filePathList.Count() > 0)
                {
                    for (int i = 1; i < filePathList.Count(); i++)
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
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                resultFilePath = filePath;
            }
            return resultFilePath;
        }

        /// <summary>
        /// ログ削除
        /// </summary>
        /// <param name="dirpath"></param>
        void FileDelete(string filepath)
        {
            FileInfo fileInfo = new FileInfo(@filepath);
            FileSecurity filesec = fileInfo.GetAccessControl();
            // 管理者の権限
            filesec.AddAccessRule(new FileSystemAccessRule("administrators", FileSystemRights.FullControl, AccessControlType.Allow));
            filesec.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            fileInfo.SetAccessControl(filesec);
            try
            {
                fileInfo.Delete();
                if (DelFileForErrorDelList.Contains(filepath))
                {
                    DelFileForErrorDelList.Remove(filepath);
                }
            }
            catch (Exception ex)
            {
                if (!DelFileForErrorDelList.Contains(filepath))
                {
                    logger.Error(ex.Message);
                    DelFileForErrorDelList.Add(filepath);
                }
            }
        }

        /// <summary>
        /// LINQ処理
        /// </summary>
        /// <param name="srcPath"></param>
        monitorServer LogToMonitorServer(string srcPath)
        {
            monitorServer resultEntity = null;
            //
            List<monitorServer> monitorServerList = monitorServerManager.GetMonitorServerList();
            int monitorNum = monitorServerList.Count();
            if (monitorNum > 0)
            {
                for (int i = 0; i < monitorNum; i++)
                {
                    try
                    {
                        int monitorID = monitorServerList[i].id;
                        string monitorSrcPath = @"\\" + monitorServerList[i].monitorServerIP + @"\" + monitorServerList[i].startFile.TrimStart('\\');
                        if (srcPath.IndexOf(monitorSrcPath) > -1)
                        {
                            resultEntity = monitorServerList[i];
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        continue;
                    }
                }
            }
            return resultEntity;
        }

        

        /// <summary>
        /// LINQ処理
        /// </summary>
        /// <param name="lineList"></param>
        /// <param name="searchKey"></param>
        static IEnumerable<string> RunQuery(IEnumerable<string> lineList, string searchKey)
        {
            var lineQuery = from line in lineList
                             where line.Contains(searchKey) 
                             select line;

            return lineQuery;
        }

        /// <summary>
        /// log
        /// </summary>
        /// <param name="errorMonitorServer"></param>
        /// <param name="errorBackupServerGroup"></param>
        /// <param name="errorFileDir"></param>
        /// <param name="errorFileName"></param>
        /// <param name="errorFileNameExtension"></param>
        /// <param name="resultFlg"></param>
        private void InsertTransferInfo(monitorServer errorMonitorServer, backupServerGroup errorBackupServerGroup,
            string errorFileDir, string errorFileName, string errorFileNameExtension, int resultFlg)
        {
            try
            {
                Models.LogManager logManager = new Models.LogManager();
                List<log> loglist = logManager.GetLogData(errorMonitorServer.id, errorFileName, errorFileDir, 0);
                if (loglist.Count() <= 0)
                {
                    log logInfo = new log();
                    logInfo.monitorServerID = errorMonitorServer.id;
                    logInfo.monitorFileName = errorFileName;
                    // 臨時
                    //logInfo.monitorFileStatus = "エラー再コピー中";
                    logInfo.monitorFileStatus = "エラー再コピー不能";
                    logInfo.monitorFilePath = errorFileDir;
                    logInfo.monitorFileType = "";
                    logInfo.monitorFileSize = "";
                    logInfo.monitorTime = DateTime.Now;
                    // 検索用
                    dateTimeForSearch = logInfo.monitorTime;
                    logInfo.transferFlg = 0;
                    logInfo.backupServerGroupID = errorBackupServerGroup.id;
                    logInfo.backupServerID = 999;
                    logInfo.backupServerFileName = errorFileName;
                    logInfo.backupServerFilePath = "";
                    logInfo.backupServerFileType = "";
                    logInfo.backupServerFileSize = "";
                    logInfo.backupStartTime = DateTime.Now;
                    logInfo.backupEndTime = DEFAULTDATETIME_VALUE;
                    logInfo.backupTime = "0ms";
                    logInfo.backupFlg = 0;
                    logInfo.copyStartTime = DateTime.Now;
                    logInfo.copyEndTime = DEFAULTDATETIME_VALUE;
                    logInfo.copyTime = "0ms";
                    logInfo.copyFlg = 2;
                    logInfo.deleteFlg = DEFAULTINT_VALUE;
                    logInfo.deleter = DEFAULTCHAR_VALUE;
                    logInfo.deleteDate = DEFAULTDATETIME_VALUE;
                    logInfo.creater = "exe";
                    logInfo.createDate = DateTime.Now;
                    logInfo.updater = "exe";
                    logInfo.updateDate = DateTime.Now;
                    logInfo.restorer = DEFAULTCHAR_VALUE;
                    logInfo.restoreDate = DEFAULTDATETIME_VALUE;
                    logManager.Insert(logInfo);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        
        /// <summary>
        /// フォルダーとファイルの判断
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="targetStr"></param>
        /// <param name="invalidNum"></param>
        public bool GetSimilarity(string sourceStr, string targetStr, int invalidNum)
        {
            bool resultSimilarity = false;
            int equalNum = 0;
            if (sourceStr.Length == targetStr.Length)
            {
                char[] sourceStrArray = sourceStr.ToCharArray();
                char[] targetStrArray = targetStr.ToCharArray();
                for (int j = 0; j < sourceStr.Length; j++)
                {
                    if (sourceStr[j].Equals(targetStr[j]))
                    {
                        equalNum++;
                    }
                }
                if (invalidNum >= (sourceStr.Length - equalNum))
                {
                    resultSimilarity = true;
                }
            }
            return resultSimilarity;
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        void XCopy(string srcpath, string exportpath)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // コマンド処理
                dircommand.Append("xcopy /h ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\"");
                // BAT作成
                string batFileName = "XCOPY-" + Common.RandomCode.GetCode(16) + ".bat";
                string batDir = "XCopy-" + DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                // 今のプログラムパス
                string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                CreateBAT batClass = new CreateBAT(batPath);
                batClass.Write(dircommand.ToString());
                defaultUserDeskTop.StartInfo.FileName = batPath;
                defaultUserDeskTop.StartInfo.UseShellExecute = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                defaultUserDeskTop.Start();
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
                if (srcpath.Equals(objectpath))
                {
                    returnFilePath = Path.Combine(targetpath, dirpath);
                }
                else
                {
                    returnFilePath = Path.Combine(Path.Combine(targetpath, dirpath), subpath[subpath.Length - 1]);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnFilePath;
        }
    }
}
