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
using BudFileCheckListenRemote.DAL;

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
        //private static ReactiveQueue<FileArgs> msQueue = new ReactiveQueue<FileArgs>();
        //用于存储路径信息
        private static ReactiveQueue<PathDesc> pathQueue = new ReactiveQueue<PathDesc>();
        static SyncManager()
        {
            pathQueue.DequeueHandler += new DequeueEventHandler<PathDesc>(pathQueue_DequeueHandler);
            pathQueue.MaxFactor = 30;
        }

        public SyncManager()
        {
            //msQueue.DequeueHandler += new DequeueEventHandler<FileArgs>(Synchronise);
        }


        public void Sync()
        {
            DaySync();
            WeekendSync();
        }

        /// <summary>
        /// 用于每天同步 删除pathBk表中job已删除的文件
        /// </summary>
        public void DaySync()
        {
            lock (_lock)
            {
                //获取配置时间
                string syncTime = ConfigUtil.AppSetting("daysyncTime");
                if (TimeCheckHelper.CheckTime("daysyncTime", syncTime))
                {

                    try
                    {
                        #region 判断是否有顶级文件夹未传送
                        new System.Threading.Thread(() =>
                            {
                                CheckTopFolder();
                            }).Start();
                        #endregion
                        #region delete pathBk overdue entry
                        Common.LogManager.WriteLog(Common.LogFile.Trace, "delete pathBk overdue entry start:" + DateTime.Now.ToString());

                        //获取今天产生的delete event entry log文件名
                        string path = AppDomain.CurrentDomain.BaseDirectory + @"log\FSWDeleteEvent" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                        StreamReader sr = new StreamReader(path);
                        while (!sr.EndOfStream)
                        {
                            string entry = sr.ReadLine();
                            if (entry.Contains("msID:"))
                            {
                                int firstIndex = entry.IndexOf("msID:") + 5;
                                int secondIndex = entry.IndexOf(";relativePath:");
                                //获取msid 
                                string msid = entry.Substring(firstIndex, secondIndex - firstIndex);
                                //获取relativePath
                                string relativePath = entry.Substring(secondIndex + 14);
                                try
                                {
                                    //删除pathBk里相同的项
                                    SyncDAL sd = new SyncDAL();
                                    sd.DeletePath(msid, relativePath);
                                }
                                catch (Exception ex)
                                {
                                    Common.LogManager.WriteLog(Common.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
                                }
                            }

                        }

                        sr.Close();


                        Common.LogManager.WriteLog(Common.LogFile.Trace, "delete pathBk overdue entry end:" + DateTime.Now.ToString());
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Common.LogManager.WriteLog(Common.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
                    }
                }
            }

        }
        /// <summary>
        /// 同步 此同步只用于下载查询
        /// </summary>
        public void WeekendSync()
        {
            //获取同步设置dayofweek
            string dayOfWeek = ConfigUtil.AppSetting("dayOfWeek");
            if (!string.IsNullOrWhiteSpace(dayOfWeek) && Regex.IsMatch(dayOfWeek, @"^[0-6]$"))
            {
                 DayOfWeek dow = (DayOfWeek)int.Parse(dayOfWeek);
                 if (DateTime.Now.DayOfWeek != dow) return;
            }
            else
            {
                Common.LogManager.WriteLog(Common.LogFile.Trace, "please set dayOfWeek correctly  :" + DateTime.Now.ToString());
                return;
            }
           
            lock (_lock)
            {
                //获取配置时间
                string syncTime = ConfigUtil.AppSetting("syncTime");
                if (TimeCheckHelper.CheckTime("syncTime", syncTime))
                {

                    Common.LogManager.WriteLog(Common.LogFile.Trace, "Sync start:" + DateTime.Now.ToString());
                    ////同步数据库 SSH输出 Mac （特殊字符文件）
                    //StartSync();
                    /**
                     * xiecongwen 20150102
                     * 由于获取文件信息，及进行数据库校验速度过慢，特进行如下修改:
                     * 
                     * 1.将ignore日志文件中的文件路径规整到相关数据库表 filelisten log （暂时忽略）
                     * 2.删除pathBK表中所有数据（truncate），将监控目录路径信息备份到此数据库（类似于快照）
                     *   每周执行一次 20150111
                     */
                    //ResolveIgnoreList();

                    BackupPath();
                    //重置monitorfilelisten表
                    TruncateFileListen();
                    Common.LogManager.WriteLog(Common.LogFile.Trace, "Sync end:" + DateTime.Now.ToString());
                }
            }
        }

        /// <summary>
        /// 将忽略文件列表补充到对应数据库
        /// xiecongwen 20150102
        /// </summary>
        private void ResolveIgnoreList()
        {

        }

        /// <summary>
        /// 检验是否有的顶层文件夹被忽略
        /// </summary>
        private void CheckTopFolder()
        {
            try
            {
                
                foreach (var ms in new DBService.MonitorServerService().GetMonitorServerListWithAttached())
                {
                    //check
                    DirectoryInfo targetDirInfo = new DirectoryInfo(ms.monitorLocalPath);
                    if (targetDirInfo.Exists)
                    {
                        if (targetDirInfo.LastWriteTime.Date < DateTime.Now.Date)
                        {
                            //如果没有传送监控，则发邮件提醒
                            //new System.Threading.Thread(() =>
                            //{
                                //send mail
                                FileListenHelper.SendMail(targetDirInfo.FullName + " 異常な配信" + " LastWriteTime:" + targetDirInfo.LastWriteTime.ToString(), "異常な配信");
                            //}).Start();
                        }
                    }
                    else
                    {
                        //如果没有传送监控，则发邮件提醒
                        //new System.Threading.Thread(() =>
                        //{
                            //send mail
                            FileListenHelper.SendMail(ms.monitorLocalPath + " 異常な配信" + " LastWriteTime:", "異常な配信");
                        //}).Start();


                    }
                }

            }
            catch (Exception ex)
            {
                Common.LogManager.WriteLog(Common.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        /// <summary>
        /// 备份路径
        /// </summary>
        private void BackupPath()
        {
            try
            {
                //删除原有备份 进行初始化
                TruncatePathBk();
                //获取路径，并插入
                 bool isOk = false;
                 foreach (var ms in new DBService.MonitorServerService().GetMonitorServerListWithAttached())
                 {
                     if (Signal.IsSystemStoping) break;

                     DirectoryInfo targetDirInfo = new DirectoryInfo(ms.monitorLocalPath);
                     /**
                      *  此处应当使用 Directory.EnumerateFiles 直接返回路径，这样就不需要用
                      *  FSUtil.GetFullPath 多此一举了
                      */
                     foreach (FileInfo fi in targetDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                     {

                         if (Signal.IsSystemStoping) break;

                         //过滤不必要文件 由于当文件夹下没有文件时 客户扔希望下载，顾不再过滤 xiecongwen 20150111
                         //if (UnnecessaryList.Contains(fi.Name)) continue;

                         //将路径放入队列
                         string fullPath = FSUtil.GetFullPath(fi);
                         PathDesc pd = new PathDesc() { msID = ms.id, relativePath = fullPath.Substring(ms.monitorLocalPath.Length)};
                     again: isOk = pathQueue.Enqueue(pd);
                         if (!isOk)
                         {
                             //由于文件过多，需要等待一下
                             System.Threading.Thread.Sleep(200);
                             goto again;
                         }
                     }
                 }

            }
            catch (Exception ex)
            {
                Common.LogManager.WriteLog(Common.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        /// <summary>
        /// 删除昨天的路径备份并初始化
        /// </summary>
        private void TruncatePathBk()
        {
            SyncDAL sd = new SyncDAL();
            sd.TruncatePathBk();
        }

        /// <summary>
        /// 删除monitorfilelisten 表并初始化
        /// </summary>
        private void TruncateFileListen()
        {
            SyncDAL sd = new SyncDAL();
            sd.TruncateFileListen();
        }

        private class PathDesc
        {
            public int msID;
            public string relativePath;
        }

        static void pathQueue_DequeueHandler(List<PathDesc> items)
        {
            try
            {
                //获取sql
                StringBuilder sb = new StringBuilder("insert into pathBK values ");
                items.ForEach(x => sb.Append("(" + x.msID + ",N'" + x.relativePath.Replace("'", "''") + "'),"));
                sb.Remove(sb.Length-1,1);

                SyncDAL sd = new SyncDAL();
                sd.SavePath(sb.ToString());
            }
            catch (Exception ex)
            {
                Common.LogManager.WriteLog(Common.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        /// <summary>
        /// 此方法暂时放弃 xiecongwen 20150102
        /// </summary>
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
                        //if (!IsExistsInDB(fullPath, ms))
                        //{
                        //    isOk = false;
                        //again: isOk = msQueue.Enqueue(new FileArgs()
                        //{
                        //    MonitorServer = ms,
                        //    FileSystemEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, ms.monitorLocalPath, fullPath.Substring(ms.monitorLocalPath.Length + 1))
                        //});
                        //    if (!isOk)
                        //    {
                        //        //由于文件过多，需要等待一下
                        //        System.Threading.Thread.Sleep(2000);
                        //        goto again;
                        //    }
                        //}
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
        /// 如果出错，则连续判断三次
        /// 
        /// 此方法暂时放弃 xiecongwen 20150102
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        private bool IsExistsInDB(string path,monitorServer ms)
        {
            bool isExists = false;
            int i= 2;
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
