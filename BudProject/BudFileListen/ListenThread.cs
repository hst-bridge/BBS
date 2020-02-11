using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using Common;
using IBLL;
using Model;
using log4net;
using System.Reflection;

namespace BudFileListen
{
    public class ListenThread
    {
        /// <summary>
        /// ID
        /// </summary>
        private int _id;
        /// <summary>
        /// IP
        /// </summary>
        private string _ipaddress;
        /// <summary>
        /// loginuser
        /// </summary>
        private string _loginuser;
        /// <summary>
        /// loginpassword
        /// </summary>
        private string _loginpassword;
        /// <summary>
        /// rootdirectory
        /// </summary>
        private string _rootdirectory;
        /// <summary>
        /// localpath
        /// </summary>
        private string _localpath;
        /// <summary>
        /// copyinit
        /// </summary>
        private int _copyinit;
        /// <summary>
        /// copydir
        /// </summary>
        private string _copydir;
        /// <summary>
        /// Time
        /// </summary>
        private int TimeoutMillis = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["listentime"]);
        /// <summary>
        /// FileSystemWatcher
        /// </summary>
        private System.IO.FileSystemWatcher fsw = null;
        /// <summary>
        /// Threading.Timer
        /// </summary>
        private System.Threading.Timer m_timer = null;
        /// <summary>
        /// fileinfo list
        /// </summary>
        private SortedList htfiles = new SortedList();
        /// <summary>
        /// Error
        /// </summary>
        private bool errorFlag = true;
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Construction method
        /// </summary>
        /// <param name="ipaddress">ipaddress</param>
        /// <param name="loginuser">loginuser</param>
        /// <param name="loginpassword">loginpassword</param>
        /// <param name="rootdirectory">rootdirectory</param>
        public ListenThread(int monitorServerId,string ipaddress, string loginuser, string loginpassword, string rootdirectory,string localpath,int copyinit,string copydir)
        {
            _id = monitorServerId;
            _ipaddress = ipaddress;
            _loginuser = loginuser;
            _loginpassword = loginpassword;
            _rootdirectory = rootdirectory;
            _localpath = localpath;
            _copyinit = copyinit;
            _copydir = copydir;
        }

        /// <summary>
        /// watch
        /// </summary>
        public bool InitWatcher()
        {
            bool result = false;
            try
            {
                if (Directory.Exists(_localpath) || File.Exists(_localpath))
                {
                    fsw = new System.IO.FileSystemWatcher();
                    // path
                    fsw.Path = _localpath;
                    /* Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories. */
                    fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
                    // Only watch text files.
                    fsw.Filter = "*.*";
                    // Add event handlers.
                    fsw.Changed += new FileSystemEventHandler(fsw_Changed);
                    fsw.Created += new FileSystemEventHandler(fsw_Changed);
                    fsw.Deleted += new FileSystemEventHandler(fsw_Changed);
                    fsw.Renamed += new RenamedEventHandler(fsw_Renamed);
                    fsw.Error += new ErrorEventHandler(fsw_Error);
                    fsw.IncludeSubdirectories = true;
                    fsw.InternalBufferSize = 64;

                    // Create the timer that will be used to deliver events. Set as disabled
                    if (m_timer == null)
                    {
                        //time処理の設定、ここで起動していない状態です。
                        m_timer = new System.Threading.Timer(new TimerCallback(OnWatchedFileChange), null, Timeout.Infinite, Timeout.Infinite);
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                errorFlag = false;
                logger.Error(ex.Message);
            }
            return result;
        }
        /// <summary>
        /// バックアップ起動
        /// </summary>
        public void Start()
        {
            string serverFolderPath = @"\\" + _ipaddress + @"\" + _rootdirectory.TrimStart('\\');
            try
            {
                //接続テスト
                bool result = true;
                int connectTime = 0;
                while (result)
                {
                    int status = NetworkConnection.Connect(serverFolderPath, _localpath, _loginuser, _loginpassword);
                    if (status == (int)ERROR_ID.ERROR_SUCCESS)
                    {
                        result = false;
                        connectTime = 0;
                        //copy
                        if (_copyinit == 0)
                        {
                            CopyDirectory _Info = new CopyDirectory(_localpath, _copydir);
                            _Info.MyCopyEnd += new CopyDirectory.CopyEnd(_Info_MyCopyEnd);
                            _Info.StarCopy();
                        }
                        if (InitWatcher())
                        {
                            // Begin watching.
                            fsw.EnableRaisingEvents = true;
                        }
                    }
                    else
                    {
                        if (connectTime > 20)
                        {
                            NetworkConnection.Disconnect(_localpath);
                            logger.Info("[" + _ipaddress + "]は接続できません");
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
        }

        /// <summary>
        /// バックアップスドップ
        /// </summary>
        public void Stop()
        {
            if (fsw != null)
            {
                fsw.EnableRaisingEvents = false;
                fsw.Dispose();
                m_timer.Dispose();
                fsw = null;
            }
        }
        /// <summary>
        /// 初始化copy完成
        /// 初始化copy完成后，需要全部传输一次，所以插入一条待传输数据
        /// </summary>
        void _Info_MyCopyEnd()
        {
            IMonitorServerService MonitorServerService = BLLFactory.ServiceAccess.CreateMonitorServerService();
            IMonitorServerFileService MonitorServerFileService = BLLFactory.ServiceAccess.CreateMonitorServerFileService();
            MonitorServerService.UpdateMonitorServerCopyInit(_id);
            MonitorServerFile monitorServerFile = new MonitorServerFile();
            monitorServerFile.monitorServerID = _id;
            monitorServerFile.monitorFileName = Path.GetFileName(_copydir);
            monitorServerFile.monitorFilePath = Path.GetDirectoryName(_copydir);
            monitorServerFile.monitorFileType = "";
            monitorServerFile.monitorFileSize = "0";
            monitorServerFile.monitorFileStatus = 1;
            monitorServerFile.monitorStartTime = CommonUtil.DateTimeNowToString();
            monitorServerFile.transferFlg = 0;
            monitorServerFile.deleteFlg = 0;
            monitorServerFile.creater = "exe";
            monitorServerFile.createDate = CommonUtil.DateTimeNowToString();
            monitorServerFile.updater = "exe";
            monitorServerFile.updateDate = CommonUtil.DateTimeNowToString();
            MonitorServerFileService.InsertMonitorServerFile(monitorServerFile,_localpath);
        }
        /// <summary>
        ///  ファイルが作成/変更/削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            //  Type判断
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                OnCreated(sender, e);
            }
            else if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                OnChanged(sender, e);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                OnDeleted(sender, e);
            }
        }
        /// <summary>
        /// ファイルがrename
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void fsw_Renamed(object source, RenamedEventArgs e)
        {
            try
            {
                Mutex mutex = new Mutex(false, "FSW");
                mutex.WaitOne();
                if (htfiles.Contains(e.FullPath))
                {
                    htfiles.Remove(e.FullPath);
                    htfiles.Add(e.FullPath, e);
                }
                else
                {
                    htfiles.Remove(e.OldFullPath);
                    htfiles.Add(e.FullPath, e);
                }
                mutex.ReleaseMutex();

                // reset thread :1回のみ執行
                m_timer.Change(TimeoutMillis, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                errorFlag = false;
                logger.Error(ex.Message);
            }
        }
        /// <summary>
        /// エラーの場合に発生します
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void fsw_Error(object source, ErrorEventArgs e)
        {
            logger.Error(e.GetException().Message);
            if (InitWatcher())
            {
                // Begin watching.
                fsw.EnableRaisingEvents = true;
            }
        }
        /// <summary>
        /// ファイルまたはディレクトリが作成された場合に発生します
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void OnCreated(object source, FileSystemEventArgs e)
        {
            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            if (htfiles.Contains(e.FullPath))
            {
                htfiles.Remove(e.FullPath);
                htfiles.Add(e.FullPath, e);
            }
            else
            {
                htfiles.Add(e.FullPath, e);
            }
            mutex.ReleaseMutex();
            
            // reset thread :1回のみ執行
            m_timer.Change(TimeoutMillis, Timeout.Infinite);
        }
        /// <summary>
        /// ファイルまたはディレクトリのサイズ、システム属性、最後の書き込み時刻、最後のアクセス時刻、
        /// またはセキュリティ アクセス許可に変更が加えられた場合に発生します。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void OnChanged(object source, FileSystemEventArgs e)
        {
            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            if (htfiles.Contains(e.FullPath))
            {
                if (((FileSystemEventArgs)htfiles[e.FullPath]).ChangeType.Equals(WatcherChangeTypes.Changed))
                {
                    htfiles.Remove(e.FullPath);
                    htfiles.Add(e.FullPath, e);
                }
            }
            else
            {
                htfiles.Add(e.FullPath, e);
            }
            mutex.ReleaseMutex();
            
            // reset thread :1回のみ執行
            m_timer.Change(TimeoutMillis, Timeout.Infinite);
        }
        /// <summary>
        /// ファイルまたはディレクトリが削除された場合に発生します。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void OnDeleted(object source, FileSystemEventArgs e)
        {
            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            if (htfiles.Contains(e.FullPath))
            {
                htfiles.Remove(e.FullPath);
                htfiles.Add(e.FullPath, e);
            }
            else
            {
                htfiles.Add(e.FullPath, e);
            }
            mutex.ReleaseMutex();

            // reset thread :1回のみ執行
            m_timer.Change(TimeoutMillis, Timeout.Infinite);
        }
        /// <summary>
        /// 状態バックアップ
        /// </summary>
        public bool GetState()
        {
            return errorFlag;
        }
        /// <summary>
        /// 变化处理
        /// </summary>
        /// <param name="state"></param>
        private void OnWatchedFileChange(object state)
        {
            IMonitorServerFileService MonitorServerFileService = BLLFactory.ServiceAccess.CreateMonitorServerFileService();
            IMonitorServerFolderService MonitorServerFolderService = BLLFactory.ServiceAccess.CreateMonitorServerFolderService();
            // File Info
            SortedList htfilescopy = (SortedList)htfiles.Clone();
            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            mutex.ReleaseMutex();

            foreach (DictionaryEntry file in htfilescopy)
            {
                try
                {
                    FileSystemEventArgs fileevent = (FileSystemEventArgs)file.Value;
                    //源路径
                    string filepath = System.IO.Path.GetFullPath(file.Key.ToString());
                    //备份路径
                    string backuppath = Path.Combine(_copydir, fileevent.Name);

                    MonitorServerFile monitorServerFile = new MonitorServerFile();
                    MonitorServerFolder monitorServerFolder = new MonitorServerFolder();

                    // copy機能
                    if (fileevent.ChangeType.Equals(WatcherChangeTypes.Created))
                    {
                        // Copy方法変更　2014/01/30 変更
                        //bool result = FileSystem.FileCopy(filepath, backuppath);
                        CopyDirectory fileCopy = new CopyDirectory(filepath, backuppath);
                        fileCopy.StarCopy();
                        //IO問題であれば、次のファイルを処理する
                        if (fileCopy._Errorlist.Count > 0)
                        {
                            foreach (string errorMessage in fileCopy._Errorlist)
                            {
                                logger.Error(errorMessage);
                            }
                            continue;
                        }
                        //if (!result)
                        //{
                        //    continue;
                        //}
                        //判断是否是目录
                        if (Directory.Exists(filepath))
                        {
                            monitorServerFile.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFile.monitorFileSize = "0";
                            monitorServerFile.monitorFileStatus = 1;
                            //---
                            monitorServerFolder.monitorServerID = _id;
                            monitorServerFolder.monitorFileName = Path.GetFileName(fileevent.Name);
                            monitorServerFolder.monitorFilePath = filepath.TrimEnd(("\\" + Path.GetFileName(fileevent.Name)).ToCharArray());
                            monitorServerFolder.monitorFileType = "99";
                            monitorServerFolder.initFlg = "0";
                            monitorServerFolder.monitorFlg = "0";
                            monitorServerFolder.creater = "admin";
                            monitorServerFolder.updater = "admin";
                            monitorServerFolder.createDate = CommonUtil.DateTimeNowToString();
                            monitorServerFolder.updateDate = CommonUtil.DateTimeNowToString();
                        }
                        else
                        {
                            System.IO.FileInfo fileinfo = new System.IO.FileInfo(backuppath);
                            monitorServerFile.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFile.monitorFileSize = fileinfo.Length.ToString();
                            monitorServerFile.monitorFileStatus = 1;
                            //---
                            monitorServerFolder.monitorServerID = _id;
                            monitorServerFolder.monitorFileName = Path.GetFileName(fileevent.Name);
                            monitorServerFolder.monitorFilePath = filepath.TrimEnd(("\\" + Path.GetFileName(fileevent.Name)).ToCharArray());
                            monitorServerFolder.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFolder.initFlg = "0";
                            monitorServerFolder.monitorFlg = "1";
                            monitorServerFolder.creater = "admin";
                            monitorServerFolder.updater = "admin";
                            monitorServerFolder.createDate = CommonUtil.DateTimeNowToString();
                            monitorServerFolder.updateDate = CommonUtil.DateTimeNowToString();
                        }
                        //新增文件默认为监视，添加check
                        MonitorServerFolderService.InsertMonitorServerFolder(monitorServerFolder);
                    }
                    else if (fileevent.ChangeType.Equals(WatcherChangeTypes.Changed))
                    {
                        System.IO.FileInfo fileinfo = new System.IO.FileInfo(backuppath);
                        if (fileinfo.IsReadOnly)
                        {
                            continue;
                        }
                        fileinfo.IsReadOnly = true;
                        // Copy方法変更　2014/01/30 変更
                        //bool result = FileSystem.FileCopy(filepath, backuppath);
                        CopyDirectory fileCopy = new CopyDirectory(filepath, backuppath);
                        fileCopy.StarCopy();
                        //IO問題であれば、次のファイルを処理する
                        if (fileCopy._Errorlist.Count > 0)
                        {
                            foreach (string errorMessage in fileCopy._Errorlist)
                            {
                                logger.Error(errorMessage);
                            }
                            continue;
                        }
                        //if (!result)
                        //{
                        //    continue;
                        //}
                        //判断是否是目录
                        if (Directory.Exists(filepath))
                        {
                            monitorServerFile.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFile.monitorFileSize = "0";
                            monitorServerFile.monitorFileStatus = 2;
                        }
                        else
                        {                            
                            monitorServerFile.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFile.monitorFileSize = fileinfo.Length.ToString();
                            monitorServerFile.monitorFileStatus = 2;
                        }
                    }
                    else if (fileevent.ChangeType.Equals(WatcherChangeTypes.Deleted))
                    {
                        //判断被删除的目录是否存在
                        if (Directory.Exists(backuppath))
                        {
                            monitorServerFile.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFile.monitorFileSize = "0";
                            bool result = FileSystem.FileDelete(backuppath);
                            //如果出现IO冲突，去执行下一个
                            if (!result)
                            {
                                continue;
                            }
                            monitorServerFile.monitorFileStatus = 3;
                        }
                        //判断被删除的文件是否存在
                        else if (File.Exists(backuppath))
                        {
                            System.IO.FileInfo fileinfo = new System.IO.FileInfo(backuppath);
                            fileinfo.IsReadOnly = false;
                            monitorServerFile.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFile.monitorFileSize = fileinfo.Length.ToString();
                            bool result = FileSystem.FileDelete(backuppath);
                            //如果出现IO冲突，去执行下一个
                            if (!result)
                            {
                                continue;
                            }
                            monitorServerFile.monitorFileStatus = 3;
                        }
                        else
                        {
                            // File Info Remove
                            htfiles.Remove(file.Key);
                            continue;
                        }
                    }
                    else if (fileevent.ChangeType.Equals(WatcherChangeTypes.Renamed))
                    {
                        RenamedEventArgs refileevent = (RenamedEventArgs)file.Value;
                        string oldfilename = refileevent.OldName.ToString();
                        string olddir = Path.Combine(_copydir, oldfilename);
                        if (Path.GetExtension(olddir) == string.Empty)
                        {
                            //old file dir info insert
                            if (Directory.Exists(olddir))
                            {                                
                                MonitorServerFile monitorServerFile1 = new MonitorServerFile();
                                monitorServerFile1.monitorServerID = _id;
                                monitorServerFile1.monitorFileName = Path.GetFileName(oldfilename);
                                monitorServerFile1.monitorFilePath = olddir;                                
                                monitorServerFile1.monitorFileType = Path.GetExtension(olddir);
                                monitorServerFile1.monitorFileSize = "0";
                                monitorServerFile1.monitorStartTime = CommonUtil.DateTimeNowToString();
                                monitorServerFile1.monitorFileStatus = 3;
                                monitorServerFile1.transferFlg = 0;
                                monitorServerFile1.deleteFlg = 0;
                                monitorServerFile1.creater = "exe";
                                monitorServerFile1.createDate = CommonUtil.DateTimeNowToString();
                                monitorServerFile1.updater = "exe";
                                monitorServerFile1.updateDate = CommonUtil.DateTimeNowToString();
                                MonitorServerFileService.InsertMonitorServerFile(monitorServerFile1,filepath);
                                bool result = FileSystem.FileDelete(olddir);
                                //如果出现IO冲突，去执行下一个
                                if (!result)
                                {
                                    continue;
                                }
                            }
                            // Copy方法変更　2014/01/30 変更
                            //bool result1 = FileSystem.FileCopy(filepath, backuppath);
                            CopyDirectory fileCopy = new CopyDirectory(filepath, backuppath);
                            fileCopy.StarCopy();
                            //IO問題であれば、次のファイルを処理する
                            if (fileCopy._Errorlist.Count > 0)
                            {
                                foreach (string errorMessage in fileCopy._Errorlist)
                                {
                                    logger.Error(errorMessage);
                                }
                                continue;
                            }
                            //if (!result1)
                            //{
                            //    continue;
                            //}
                            //new file dir info insert                            
                            monitorServerFile.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFile.monitorFileSize = "0";
                            monitorServerFile.monitorFileStatus = 4;
                            //---
                            monitorServerFolder.monitorServerID = _id;
                            monitorServerFolder.monitorFileName = Path.GetFileName(fileevent.Name);
                            monitorServerFolder.monitorFilePath = filepath.TrimEnd(("\\" + Path.GetFileName(fileevent.Name)).ToCharArray());
                            monitorServerFolder.monitorFileType = "99";
                            monitorServerFolder.initFlg = "0";
                            monitorServerFolder.monitorFlg = "0";
                            monitorServerFolder.creater = "admin";
                            monitorServerFolder.updater = "admin";
                            monitorServerFolder.createDate = CommonUtil.DateTimeNowToString();
                            monitorServerFolder.updateDate = CommonUtil.DateTimeNowToString();
                        }
                        else
                        {
                            //old file info insert
                            if (File.Exists(olddir))
                            {                                
                                MonitorServerFile monitorServerFile1 = new MonitorServerFile();
                                monitorServerFile1.monitorServerID = _id;
                                monitorServerFile1.monitorFileName = Path.GetFileName(oldfilename);
                                monitorServerFile1.monitorFilePath = olddir;
                                System.IO.FileInfo fileinfo1 = new System.IO.FileInfo(olddir);
                                monitorServerFile1.monitorFileType = Path.GetExtension(olddir);
                                monitorServerFile1.monitorFileSize = fileinfo1.Length.ToString();
                                monitorServerFile1.monitorStartTime = CommonUtil.DateTimeNowToString();
                                monitorServerFile1.monitorFileStatus = 3;
                                monitorServerFile1.transferFlg = 0;
                                monitorServerFile1.deleteFlg = 0;
                                monitorServerFile1.creater = "exe";
                                monitorServerFile1.createDate = CommonUtil.DateTimeNowToString();
                                monitorServerFile1.updater = "exe";
                                monitorServerFile1.updateDate = CommonUtil.DateTimeNowToString();
                                MonitorServerFileService.InsertMonitorServerFile(monitorServerFile1,filepath);
                                fileinfo1.IsReadOnly = false;
                                bool result = FileSystem.FileDelete(olddir);
                                //如果出现IO冲突，去执行下一个
                                if (!result)
                                {
                                    continue;
                                }
                            }
                            // Copy方法変更　2014/01/30 変更
                            //bool result1 = FileSystem.FileCopy(filepath, backuppath);
                            CopyDirectory fileCopy = new CopyDirectory(filepath, backuppath);
                            fileCopy.StarCopy();
                            //IO問題であれば、次のファイルを処理する
                            if (fileCopy._Errorlist.Count > 0)
                            {
                                foreach (string errorMessage in fileCopy._Errorlist)
                                {
                                    logger.Error(errorMessage);
                                }
                                continue;
                            }
                            //if (!result1)
                            //{
                            //    continue;
                            //}
                            //new file info insert
                            System.IO.FileInfo fileinfo = new System.IO.FileInfo(backuppath);
                            monitorServerFile.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFile.monitorFileSize = fileinfo.Length.ToString();
                            monitorServerFile.monitorFileStatus = 4;
                            //---
                            monitorServerFolder.monitorServerID = _id;
                            monitorServerFolder.monitorFileName = Path.GetFileName(fileevent.Name);
                            monitorServerFolder.monitorFilePath = filepath.TrimEnd(("\\" + Path.GetFileName(fileevent.Name)).ToCharArray());
                            monitorServerFolder.monitorFileType = Path.GetExtension(backuppath);
                            monitorServerFolder.initFlg = "0";
                            monitorServerFolder.monitorFlg = "1";
                            monitorServerFolder.creater = "admin";
                            monitorServerFolder.updater = "admin";
                            monitorServerFolder.createDate = CommonUtil.DateTimeNowToString();
                            monitorServerFolder.updateDate = CommonUtil.DateTimeNowToString();
                        }
                        //重命名后文件默认为监视，添加check
                        MonitorServerFolderService.InsertMonitorServerFolder(monitorServerFolder);
                    }
                    //insert database
                    monitorServerFile.monitorServerID = _id;
                    monitorServerFile.monitorFileName = Path.GetFileName(fileevent.Name);
                    monitorServerFile.monitorFilePath = backuppath;
                    monitorServerFile.monitorStartTime = CommonUtil.DateTimeNowToString();
                    monitorServerFile.transferFlg = 0;
                    monitorServerFile.deleteFlg = 0;
                    monitorServerFile.creater = "exe";
                    monitorServerFile.createDate = CommonUtil.DateTimeNowToString();
                    monitorServerFile.updater = "exe";
                    monitorServerFile.updateDate = CommonUtil.DateTimeNowToString();
                    MonitorServerFileService.InsertMonitorServerFile(monitorServerFile,filepath);
                    // File Info Remove
                    htfiles.Remove(file.Key);
                }
                catch(Exception e)
                {
                    logger.Error(e.Message);
                    continue;
                }
            }            
        }
    }
}
