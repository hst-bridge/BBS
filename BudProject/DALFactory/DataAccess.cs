using System.Reflection;
using System.Configuration;

namespace DALFactory
{
    public sealed class DataAccess
    {
        //DAL命名空间
        private static readonly string DALPath = ConfigurationManager.AppSettings["DALPath"];
        /// <summary>
        /// 私有构造函数
        /// </summary>
        private DataAccess() { }
        /// <summary>
        /// ユーザー
        /// </summary>
        /// <returns></returns>
        public static IDAL.IUserInfo CreateUserInfo()
        {
            string className =DALPath+ ".UserInfoD";
            return (IDAL.IUserInfo)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ元ファイル
        /// </summary>
        /// <returns></returns>
        public static IDAL.IMonitorServerFile CreateMonitorServerFile()
        {
            string className = DALPath + ".MonitorServerFileD";
            return (IDAL.IMonitorServerFile)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ元
        /// </summary>
        /// <returns></returns>
        public static IDAL.IMonitorServer CreateMonitorServer()
        {
            string className = DALPath + ".MonitorServerD";
            return (IDAL.IMonitorServer)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象グループ明細
        /// </summary>
        /// <returns></returns>
        public static IDAL.IBackupServerGroupDetail CreateBackupServerGroupDetail()
        {
            string className = DALPath + ".BackupServerGroupDetailD";
            return (IDAL.IBackupServerGroupDetail)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象グループ
        /// </summary>
        /// <returns></returns>
        public static IDAL.IBackupServerGroup CreateBackupServerGroup()
        {
            string className = DALPath + ".BackupServerGroupD";
            return (IDAL.IBackupServerGroup)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象ファイル
        /// </summary>
        /// <returns></returns>
        public static IDAL.IBackupServerFile CreateBackupServerFile()
        {
            string className = DALPath + ".BackupServerFileD";
            return (IDAL.IBackupServerFile)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象
        /// </summary>
        /// <returns></returns>
        public static IDAL.IBackupServer CreateBackupServer()
        {
            string className = DALPath + ".BackupServerD";
            return (IDAL.IBackupServer)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// ログ
        /// </summary>
        /// <returns></returns>
        public static IDAL.ILog CreateLog()
        {
            string className = DALPath + ".LogD";
            return (IDAL.ILog)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ元フォルダーテーブル
        /// </summary>
        /// <returns></returns>
        public static IDAL.IMonitorServerFolder CreateMonitorServerFolder()
        {
            string className = DALPath + ".MonitorServerFolderD";
            return (IDAL.IMonitorServerFolder)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// 転送容量
        /// </summary>
        /// <returns></returns>
        public static IDAL.ITransferLog CreateTransferLog()
        {
            string className = DALPath + ".TransferLogD";
            return (IDAL.ITransferLog)Assembly.Load(DALPath).CreateInstance(className);
        }
        public static IDAL.IFileTypeSet CreateFileTypeSet() 
        {
            string className = DALPath + ".FileTypeSetD";
            return (IDAL.IFileTypeSet)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ転送サーバ関係
        /// </summary>
        /// <returns></returns>
        public static IDAL.IMonitorBackupServer CreateMonitorBackupServer()
        {
            string className = DALPath + ".MonitorBackupServerD";
            return (IDAL.IMonitorBackupServer)Assembly.Load(DALPath).CreateInstance(className);
        }
        /// <summary>
        /// ファイルダウンロード
        /// </summary>
        /// <returns></returns>
        public static IDAL.IMonitorFileListen CreateMonitorFileListen()
        {
            string className = DALPath + ".MonitorFileListenD";
            return (IDAL.IMonitorFileListen)Assembly.Load(DALPath).CreateInstance(className);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IDAL.IManualBackupServer CreateManualBackupServer()
        {
            string className = DALPath + ".ManualBackupServerD";
            return (IDAL.IManualBackupServer)Assembly.Load(DALPath).CreateInstance(className);
        }
    }
}
