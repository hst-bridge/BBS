using System.Reflection;
using System.Configuration;

namespace BLLFactory
{
    public sealed class ServiceAccess
    {
        //BLL命名空间
        private static readonly string BLLPath = ConfigurationManager.AppSettings["BLLPath"];
        /// <summary>
        /// 私有构造函数
        /// </summary>
        private ServiceAccess() { }
        /// <summary>
        /// ユーザー
        /// </summary>
        /// <returns></returns>
        public static IBLL.IUserInfoService CreateUserInfoService()
        {
            string className = BLLPath + ".UserInfoService";
            return (IBLL.IUserInfoService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ元ファイル
        /// </summary>
        /// <returns></returns>
        public static IBLL.IMonitorServerFileService CreateMonitorServerFileService()
        {
            string className = BLLPath + ".MonitorServerFileService";
            return (IBLL.IMonitorServerFileService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ元
        /// </summary>
        /// <returns></returns>
        public static IBLL.IMonitorServerService CreateMonitorServerService()
        {
            string className = BLLPath + ".MonitorServerService";
            return (IBLL.IMonitorServerService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象グループ明細
        /// </summary>
        /// <returns></returns>
        public static IBLL.IBackupServerGroupDetailService CreateBackupServerGroupDetailService()
        {
            string className = BLLPath + ".BackupServerGroupDetailService";
            return (IBLL.IBackupServerGroupDetailService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象グループ
        /// </summary>
        /// <returns></returns>
        public static IBLL.IBackupServerGroupService CreateBackupServerGroupService()
        {
            string className = BLLPath + ".BackupServerGroupService";
            return (IBLL.IBackupServerGroupService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象ファイル
        /// </summary>
        /// <returns></returns>
        public static IBLL.IBackupServerFileService CreateBackupServerFileService()
        {
            string className = BLLPath + ".BackupServerFileService";
            return (IBLL.IBackupServerFileService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象
        /// </summary>
        /// <returns></returns>
        public static IBLL.IBackupServerService CreateBackupServer()
        {
            string className = BLLPath + ".BackupServerService";
            return (IBLL.IBackupServerService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// バックアップ先対象
        /// </summary>
        /// <returns></returns>
        public static IBLL.IBackupServerService CreateBackupServerService()
        {
            string className = BLLPath + ".BackupServerService";
            return (IBLL.IBackupServerService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// ログ
        /// </summary>
        /// <returns></returns>
        public static IBLL.ILogService CreateLogService()
        {
            string className = BLLPath + ".LogService";
            return (IBLL.ILogService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// 転送容量
        /// </summary>
        /// <returns></returns>
        public static IBLL.ITransferLogService CreateTransferLogService()
        {
            string className = BLLPath + ".TransferLogService";
            return (IBLL.ITransferLogService)Assembly.Load(BLLPath).CreateInstance(className);
        }
		/// <summary>
        /// バックアップ元フォルダ
        /// </summary>
        /// <returns></returns>
        public static IBLL.IMonitorServerFolderService CreateMonitorServerFolderService()
        {
            string className = BLLPath + ".MonitorServerFolderService";
            return (IBLL.IMonitorServerFolderService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        public static IBLL.IFileTypeSetService CreateFileTypeSetService()
        {
            string className = BLLPath + ".FileTypeSetService";
            return (IBLL.IFileTypeSetService)Assembly.Load(BLLPath).CreateInstance(className);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IBLL.IMonitorFileListenService CreateMonitorFileListenService()
        {
            string className = BLLPath + ".MonitorFileListenService";
            return (IBLL.IMonitorFileListenService)Assembly.Load(BLLPath).CreateInstance(className);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IBLL.IManualBackupServerService CreateManualBackupServerService()
        {
            string className = BLLPath + ".ManualBackupServerService";
            return (IBLL.IManualBackupServerService)Assembly.Load(BLLPath).CreateInstance(className);
        }
    }
}
