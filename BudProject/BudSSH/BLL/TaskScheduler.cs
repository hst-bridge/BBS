using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.DAL.IBLL;
using System.IO;
using BudSSH.Model;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using log4net;
using System.Reflection;
using BudSSH.Common.Util;
using System.Threading;
using System.Diagnostics;

namespace BudSSH.BLL
{
    /// <summary>
    /// 用于任务调度
    /// 
    /// 根据情况调度程序
    /// </summary>
    class TaskScheduler
    {
        private TaskScheduler()
        {

        }
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public static readonly TaskScheduler Instance = new TaskScheduler();

        /// <summary>
        /// 启动
        /// </summary>
        public void Execute()
        {
            Signal.IsSystemStoping = false;

            /**
             * 
             * ssh 有三个功能
             * 1.定时读robocopy 日志，解析并从mac下载文件
             * 2.由于ssh文件是否删除，无法被动获取到信息，故定时扫描数据库sshpathinfo表，主动判断mac上文件是否仍然存在
             * 3.当前为了保证目标路径文件系统的完整性，实现方式为ssh下载到一个备份目标，然后再将文件复制到目标路径，
             *    由于robocopy不认可sshcopy的文件，每次robocopy运行时会主动删除ssh复制过来的文件，故当前实现方式是，每天当robocopy停止向目标路径同步时
             *    ssh将特殊文件再次复制到相关目标路径时，以保证在所有程序运行结束时，目标文件路径的正确性及完整性
             *    
             * 以上三个功能，有个共同的特点：一天只运行一次，执行时间不定
             * 现在实现方式改为：开始运行时，为三个功能各自配备一个线程，互不干扰
             * 
             * xiecongwen 20141219
             */

            /**
             * 
             * 创建三个线程
             *
             */

            /// <summary>
            /// 用于读取日志，解析并从Mac机下载文件
            /// </summary>
            new Thread(() => LogAnalyse()).Start();

            //用于保证本地Robocopy目标路径的完整
            new Thread(() => SyncLocalSSHFileAndTargetDirectory()).Start();

            //于适当时间扫描数据库，判断文件在mac机上是否已经删除
            new Thread(() => SyncLocalSSHFileAndMac()).Start();


            //上面定义了三个任务
            Signal.TotalTaskCount = 3;

            //用于监视任务执行状况
            new Thread(() => MonitorTask()).Start();
            
        }

        /// <summary>
        /// 监视任务执行状况
        /// </summary>
        private void MonitorTask()
        {
            while(true)
            {
                if (Signal.IsSystemStoping) break;

                //所有任务执行完成后，停止任务，关闭程序
                if (Signal.CompletedTaskCount > 0 && Signal.CompletedTaskCount >= Signal.TotalTaskCount)
                {
                    Stop();

                    Process.GetCurrentProcess().Close();
                    Process.GetCurrentProcess().Kill();
                }

                Thread.Sleep(60000);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            Signal.IsSystemStoping = true;

            SqlConnection.ClearAllPools();

        }

        /// <summary>
        /// 调度执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogAnalyse()
        {

            try
            {
                while (true)
                {
                    if (Signal.IsSystemStoping) break;
                    //加载最新配置
                    Config config = ConfigManager.GetCurrentConfig();

                    #region 调用解析器
                    LogAnalyser lal = new LogAnalyser();
                    lal.Analyse(config);
                    #endregion

                    Thread.Sleep(60000);

                }
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }


        }

        void SyncLocalSSHFileAndMac()
        {
            try
            {
                while (true)
                {
                    if (Signal.IsSystemStoping) break;
                    //加载最新配置
                    Config config = ConfigManager.GetCurrentConfig();
                    LocalSyncManager lsm = new LocalSyncManager();

                    #region 同步 数据库
                    lsm.LookBackSync(config);
                    #endregion

                    Thread.Sleep(60000);

                }
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }
        }
        /// <summary>
        /// 1.用于保证本地Robocopy目标路径的完整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SyncLocalSSHFileAndTargetDirectory()
        {

            try
            {
                while (true)
                {
                    if (Signal.IsSystemStoping) break;
                    //加载最新配置
                    Config config = ConfigManager.GetCurrentConfig();
                    LocalSyncManager lsm = new LocalSyncManager();

                    #region 同步SSH输出 到 robocopy 目的路径
                    lsm.LocalSync(config);
                    #endregion

                    Thread.Sleep(60000);

                }
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }

        }


    }
}
