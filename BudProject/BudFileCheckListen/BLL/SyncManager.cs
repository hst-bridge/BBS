using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BudFileCheckListen.Entities;
using BudFileCheckListen.Common.Util;
using System.Text.RegularExpressions;
using BudFileCheckListen.Common.Helper;
using BudFileCheckListen.Models;
using BudFileCheckListen.Common.FileSystem;

namespace BudFileCheckListen.BLL
{
    /// <summary>
    /// 用于同步本地文件与数据库记录，保证数据库记录的正确性及完整性
    /// </summary>
    class SyncManager
    {

        private static volatile object _lock = new object();
        private LogTableManager ltm = LogTableManager.Instance;
        private MFLTableManager mtm = MFLTableManager.Instance;
        private static string[] UnnecessaryList =  { 
           ".DS_Store",".com.apple.timemachine.supported","Icon"
        };
        //用于同步文件系统和数据库 ps:添加文件系统遗漏的，删除数据库中多余的
        private static ReactiveQueue<FileArgs> msQueue = new ReactiveQueue<FileArgs>();

        public SyncManager()
        {
            msQueue.DequeueHandler += new DequeueEventHandler<FileArgs>(Synchronise);
        }

        /// <summary>
        /// 同步
        /// </summary>
        public void Sync()
        {
            lock (_lock)
            {
                //获取配置时间
                string syncTime = ConfigUtil.AppSetting("syncTime");
                if (TimeCheckHelper.CheckTime("syncTime", syncTime))
                {

                    Common.LogManager.WriteLog(Common.LogFile.Trace, "Sync start:" + DateTime.Now.ToString());
                    //同步数据库 SSH输出 Mac （特殊字符文件）
                    StartSync();
                    Common.LogManager.WriteLog(Common.LogFile.Trace, "Sync end:" + DateTime.Now.ToString());
                }
            }
        }

        private void StartSync()
        {
            /**
             * 
             * 依次扫描监控路径,并判断数据库中是否存在
             * 
             */
            try
            {
                bool isOk = false;
                foreach (var ms in new DBService.MonitorServerService().GetMonitorServerListWithAttached())
                {
                    if (Signal.IsSystemStoping) break;

                    DirectoryInfo targetDirInfo = new DirectoryInfo(ms.monitorLocalPath);
                    foreach (FileInfo fi in targetDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                    {

                        if (Signal.IsSystemStoping) break;

                        //过滤不必要文件
                        if (UnnecessaryList.Contains(fi.Name)) continue;

                        //判断数据库是否存在
                        string fullPath = FSUtil.GetFullPath(fi);
                        if (!IsExistsInDB(fullPath, ms))
                        {
                            isOk = false;
                            again: isOk = msQueue.Enqueue(new FileArgs()
                            {
                                MonitorServer = ms,
                                FileSystemEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, ms.monitorLocalPath, fullPath.Substring(ms.monitorLocalPath.Length + 1))
                            });
                             if (!isOk)
                             {
                                 //由于文件过多，需要等待一下
                                 System.Threading.Thread.Sleep(2000);
                                 goto again;
                             }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogManager.WriteLog(Common.LogFile.Error,MessageUtil.GetExceptionMsg(ex,""));
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        private bool IsExistsInDB(string path,monitorServer ms)
        {
            bool isExists = false;
            int i = 2;
            bool isOk = true;
            do
            {
                try
                {
                    DBService.MonitorFileListenService mflservice = new DBService.MonitorFileListenService();
                    string relativeFullPath = path.Substring(ms.monitorLocalPath.Length);
                    isExists = mflservice.IsPathExists(ms.id, relativeFullPath);
                    isOk = true;
                }
                catch (Exception ex)
                {
                    Common.LogManager.WriteLog(Common.LogFile.Error, "Path:" + path + Environment.NewLine + MessageUtil.GetExceptionMsg(ex, ""));
                    isExists = false;
                    isOk = false;
                }

            } while (!isOk && i-- > 0);

            return isExists;
        }

        /// <summary>
        /// 同步 
        /// </summary>
        private void Synchronise(List<FileArgs> items)
        {
            try
            {
                //依次处理消息
                foreach (var item in items)
                {
                    ltm.Handle(item.MonitorServer, item.FileSystemEventArgs);
                    mtm.Handle(item.MonitorServer, item.FileSystemEventArgs);
                }
            }
            catch (Exception ex)
            {
                Common.LogManager.WriteLog(Common.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        


    }
}
