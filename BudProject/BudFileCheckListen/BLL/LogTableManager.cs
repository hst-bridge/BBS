using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudFileCheckListen.Entities;
using BudFileCheckListen.DBService;
using BudFileCheckListen.Common.Constant;
using log4net;
using System.Reflection;
using System.IO;
using BudFileCheckListen.Common.FileSystem;
using BudFileCheckListen.Common.Util;
using BudFileCheckListen.Models;

namespace BudFileCheckListen.BLL
{
    /// <summary>
    /// 用于向Log表插入日志
    /// </summary>
    class LogTableManager
    {

        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ReactiveQueue<log> rqueue = new ReactiveQueue<log>();
        private LogTableManager()
        {
            rqueue.DequeueHandler += new DequeueEventHandler<log>(rqueue_DequeueHandler);
        }

        public static readonly LogTableManager Instance = new LogTableManager();

        /// <summary>
        /// 将log插入到数据库
        /// </summary>
        /// <param name="items"></param>
        void rqueue_DequeueHandler(List<log> items)
        {
            try
            {
                LogService logManager = new LogService();
                logManager.Insert(items);
            }
            catch (Exception ex)
            {
                logger.Error("LogTableManager"+Environment.NewLine+MessageUtil.GetExceptionMsg(ex,""));
            }
        }
        /// <summary>
        /// 识别消息，插入到数据库
        /// </summary>
        public void Handle(monitorServer ms,FileSystemEventArgs item)
        {
            if (item.ChangeType == WatcherChangeTypes.Deleted) return;

            //封装log对象 并压入队列
            InsertLog(ms, item);
        }
        /// <summary>
        /// 封装log对象 并压入队列
        /// </summary>
        /// <param name="file"></param>
        private void InsertLog(monitorServer ms, FileSystemEventArgs item)
        {
            //insert log
            try
            {
                FileInfoStruct fi = FSUtil.FindSpecialFileInfo(item.FullPath);

                log logInfo = new log();
                logInfo.monitorServerID = ms.id;
                logInfo.monitorFileName = fi.Name;
                logInfo.monitorFileStatus = "正常完了";

                if (item.Name.Contains('\\'))
                {
                    logInfo.monitorFilePath = ms.monitorDrive + "\\" + item.Name.Substring(0, item.Name.LastIndexOf('\\'));
                }
                else
                {
                    logInfo.monitorFilePath = ms.monitorDrive;
                }
                logInfo.monitorFileType = fi.Extension;
                logInfo.monitorFileSize = fi.Length.ToString();
                logInfo.monitorTime = DateTime.Now;
                logInfo.transferFlg = (short)1;
                //根据monitorServer 查询backupServerGroup表monitorServerID 获取其id
                logInfo.backupServerGroupID = ms.backupServerGroupID;

                //根据backupServerGroupID 查询backupServerGroupDetail表 获取backupServerID
                logInfo.backupServerID = ms.backupServerID;
                logInfo.backupServerFileName = fi.Name;
                
                logInfo.backupServerFilePath = DefaultValue.DEFAULTCHAR_VALUE;
                logInfo.backupServerFileType = fi.Extension;
                logInfo.backupServerFileSize = fi.Length.ToString();
                logInfo.backupStartTime = DateTime.Now;
                logInfo.backupEndTime = DateTime.Now;
                logInfo.backupTime = (logInfo.backupEndTime - logInfo.backupStartTime).TotalMilliseconds.ToString() + "ms";
                logInfo.backupFlg = (short)1;
                logInfo.copyStartTime = logInfo.backupStartTime;
                logInfo.copyEndTime = logInfo.backupEndTime;
                logInfo.copyTime = logInfo.backupTime;
                
                logInfo.copyFlg = 2;
                
                logInfo.deleteFlg = DefaultValue.DEFAULTINT_VALUE;
                logInfo.deleter = DefaultValue.DEFAULTCHAR_VALUE;
                logInfo.deleteDate = DefaultValue.DEFAULTDATETIME_VALUE;
                logInfo.creater = "exe";
                logInfo.createDate = DateTime.Now;
                logInfo.updater = "exe";
                logInfo.updateDate = DateTime.Now;
                logInfo.restorer = DefaultValue.DEFAULTCHAR_VALUE;
                logInfo.restoreDate = DefaultValue.DEFAULTDATETIME_VALUE;
                rqueue.Enqueue(logInfo);
            }
            catch (Exception e)
            {
                logger.Error("LogTableManager" + Environment.NewLine + item.FullPath + Environment.NewLine + MessageUtil.GetExceptionMsg(e, ""));
            }
        }

        ~LogTableManager()
        {
            //清理
            rqueue.Close();
        }
    }
}
