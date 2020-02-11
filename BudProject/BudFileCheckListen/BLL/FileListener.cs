using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudFileCheckListen.DBService;
using System.IO;
using BudFileCheckListen.Common;
using BudFileCheckListen.Entities;
using System.Reflection;
using log4net;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using BudFileCheckListen.Models;
using BudFileCheckListen.Common.FileSystem;
using BudFileCheckListen.Common.Constant;
using BudFileCheckListen.Common.Util;

namespace BudFileCheckListen.BLL
{

    public delegate bool CallBack(Exception ex);
    /// <summary>
    /// 用于监听文件变化，并将状态更新到数据库
    /// </summary>
    class FileListener
    {

        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// pathlist
        /// </summary>
        private string pathListFileName = System.Configuration.ConfigurationManager.AppSettings["DirPathList"].ToString();

        

        public static readonly FileListener Instance = new FileListener();

        private List<FileSystemWatcher> fswlist = new List<FileSystemWatcher>();

        #region 线程控制
        /**
         * 当前逻辑 
         * 先判断本地目录下是否有文件，
         *   如果没有，则直接建立监视
         *   如果有，则判断数据库中对应monitorServerID是否有记录，
         *            如果没有，则直接插入数据库
         *            如果有，则需进行比较核对并更新数据库
         */
        //用于控制是否退出 届时将强制关闭工作线程
        private CancellationTokenSource cts = new CancellationTokenSource();
        //暂停工作线程
        AutoResetEvent evtPause = new AutoResetEvent(false);
        //恢复工作线程
        AutoResetEvent evtResume = new AutoResetEvent(false);
        //控制线程
        private Thread MainThread = null;
        //全局信号
        private volatile bool isStoping = false;
        #endregion

        private FileListener() {
            cts.Token.Register(() => { isStoping = true; });
            
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        public void Listen(CallBack action)
        {
            //如果是第一次 则需要建立控制线程
            if (MainThread == null)
            {
                MainThread = new Thread(() =>
                {
                    #region 工作线程
                    Thread doThread = new Thread(() =>
                    {
                        DO(action);
                    });
                    doThread.Start();
                    #region 响应外界信号，并控制工作线程
                    /**
                     * xiecongwen 20140704
                     * 
                     * 用于控制工作线程(主要是第一次更新的时候，即本地目录已经有文件）
                     * 工作线程扫描完之后，管理权交予FileSystemWatcher
                     * 
                     * 例外：如果此时BudFileListen程序在运行，并且不断有文件更新
                     *       工作线程可能难以结束，此处暂时决定只扫描一遍本地目录，之后便交予FileSystemWatcher
                     *       对于此期间已扫描过的文件的再次变更，采取忽略操作 
                     */
                    while (true)
                    {
                        #region 程序退出操作 则进行清理
                        if (cts.Token.IsCancellationRequested)
                        {
                            //关闭工作线程
                            if (doThread != null && doThread.IsAlive)
                            {
                                try
                                {
                                    if (doThread.ThreadState == System.Threading.ThreadState.Suspended)
                                    {
                                        doThread.Resume();
                                    }
                                    doThread.Abort();
                                    doThread.Join();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.Message);
                                }
                            }
                            //清理FileSystemWatcher
                            fswlist.ForEach((fsw) => { fsw.EnableRaisingEvents = false; fsw.Dispose(); });
                            logger.Info("thread stop");
                            break;
                        }
                        #endregion
                        #region 程序正常运行,用户点击停止 开始按钮
                        /**
                         * 判断用户是否点击停止操作
                         * 如果是：则判断工作线程是否结束，如果未结束，则挂起。下次用户点击开始时 恢复
                         *         如果已经结束，则关闭FileSystemWatcher的事件触发 下次用户点击开始时 恢复
                         */
                        if (evtPause.WaitOne(0))
                        {
                            #region pause
                            if (doThread != null && doThread.IsAlive)
                            {
                                try
                                {
                                    doThread.Suspend();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.Message);
                                }

                            }
                            //关闭事件监听
                            fswlist.ForEach((fsw) => fsw.EnableRaisingEvents = false);
                            
                            #endregion
                            
                        }
                        #region resume
                        if (evtResume.WaitOne(0))
                        {
                            if (doThread != null && doThread.IsAlive)
                            {
                                try
                                {
                                    doThread.Resume();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.Message);
                                }

                            }
                            //开启事件监听
                            fswlist.ForEach((fsw) => fsw.EnableRaisingEvents = true);
                            
                        }
                        #endregion
                        #endregion
                        Thread.Sleep(2000);
                    }
                    #endregion
                    #endregion
                });
                MainThread.Start();
                   
            }
            else
            {
                //恢复工作线程
                evtResume.Set();
            }
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            evtPause.Set();
        }

        /// <summary>
        /// 程序退出
        /// </summary>
        public void ShutDown()
        {
            cts.Cancel();
            Signal.IsSystemStoping = true;
        }
        /// <summary>
        /// 具体业务逻辑
        /// </summary>
        private void DO(CallBack action)
        {
            Signal.IsSystemStoping = false;

            try
            {
                Common.LogManager.WriteLog(LogFile.Bussiness, "Listen start");
                //判断是否有监视JOB端(即MAC机)
                foreach (var ms in new DBService.MonitorServerService().GetMonitorServerListWithAttached())
                {
                    Common.LogManager.WriteLog(LogFile.Bussiness, ms.id + " attend to Listening");

                    #region 建立监视
                    try
                    {

                        DirectoryInfo targetDirInfo = new DirectoryInfo(ms.monitorLocalPath);
                        #region 判断目标盘符是否有足够空间,并提示用户

                        if (!FileListenHelper.CheckDisk(targetDirInfo))
                        {
                            new System.Threading.Thread(() =>
                            {
                                //send mail
                                FileListenHelper.SendMail(targetDirInfo, ms);
                            }).Start();
                        }
                        #endregion

                        //将当前已有文件系统信息同步到数据库,然后建立监控
                        /*
                         *  由于时间原因，暂时认为此程序先开启，然后再开启BudFileListen 20140704
                         *  
                         *  当前逻辑是判断数据库中对应monitorserver是否为空 如果为空则扫描整个文件系统
                         *  并插入到数据库中 20140705
                         */
                        FileListenResolver flr = new FileListenResolver(ms);
                        FileCheck(ms, flr);
                        #region 设置监视器
                        FileSystemWatcher fsw = new FileSystemWatcher()
                        {
                            //只监听文件创建、删除、文件大小变化，忽略目录变化
                            NotifyFilter = NotifyFilters.Size | NotifyFilters.FileName,
                            //Filter="*",
                            Path = ms.monitorLocalPath,
                            IncludeSubdirectories = true
                        };
                        //设置事件
                        flr.SetEvent(fsw);
                        fsw.EnableRaisingEvents = true;
                        fswlist.Add(fsw);
                        #endregion

                    }
                    catch (ArgumentException aex)
                    {
                        Common.LogManager.WriteLog(LogFile.Bussiness, ms.id + " error:" + aex.Message);
                        //回调错误处理函数
                        if (action(aex))
                        {
                            //用户选择退出时，当前线程直接退出，并新建一个线程去清理资源
                            ThreadPool.QueueUserWorkItem((x) => { Thread.Sleep(1000); ShutDown(); });
                            break;
                        }

                        continue;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        action(ex);
                        break;
                    }
                    
                    //日志 通知 此monitorserver已经建立监听
                    Common.LogManager.WriteLog(LogFile.Bussiness, ms.id + " Listening");
                    #endregion
                }

                //开启定时同步线程
                new System.Threading.Thread(() => SyncFileSystemAndDB()).Start();
                
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                action(ex);
            }
            finally
            {
                //所有server 都处理了
                Common.LogManager.WriteLog(LogFile.Bussiness, "All servers have been processed");
            }
        }
        /// <summary>
        /// 处理
        /// </summary>
        private void FileCheck(monitorServer ms, FileListenResolver flr)
        {
            try
            {
               
                    #region 维护文件列表数据
                    try
                    {

                        //判断数据库中对应monitorServer是否为空，如果为空 则不进行比较，直接插入
                        FileListenService fls = new FileListenService();
                        if (fls.IsDBCleared(ms.id))
                        {
                             FileListenHelper.BackupFSWithoutCheck(ms, flr);
                        }
                        else
                        {
                            /**
                             * 数据库数据正确性及完整性维护逻辑 20140706 xiecongwen
                             * 进行比较 此处将不使用自制消息，即数据完整性交由数据库实现
                             * 为了提高效率：特将比较逻辑移到内存，即从数据库中取一部分数据
                             *          与文件系统进行比较 看是否存在
                             *          
                             * 两边数据相同：1.文件系统没有的，数据库要删除；文件系统有的，数据库要添加
                             * 
                             * 难点：查找文件是否在数据库中存在 原因：数据量大，且聚集为id
                             *    从根目录开始：  1.目录下文件2.子目录 依次递归
                             */


                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                    }
                    #endregion
                
                
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        
        /// <summary>
        /// 同步数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SyncFileSystemAndDB()
        {
            
            
            try
            {
                while (true)
                {
                    if (Signal.IsSystemStoping) break;

                    //同步
                    SyncManager sm = new SyncManager();
                    sm.Sync();

                    System.Threading.Thread.Sleep(60000);
                }
            }
            catch (Exception ex)
            {
               Common.LogManager.WriteLog(LogFile.System,MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        ~FileListener()
        {
            //关闭监听
            ShutDown();
        }
    }
}
