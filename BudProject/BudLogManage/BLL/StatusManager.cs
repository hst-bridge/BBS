using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IBLL;
using BudLogManage.Model;
using System.IO;
using log4net;
using System.Reflection;

namespace BudLogManage.BLL
{
    /// <summary>
    /// 用于管理日志处理状态
    /// </summary>
    public static class StatusManager
    {
        private static List<string> filesList = new List<string>();
        private static readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void SetPathUpdate()
        {
            new System.Threading.Thread(x =>
            {
                try
                {
                    ILogAnalyser ila = TaskScheduler.Instance.CurrentLogAnalyser;
                    if (ila != null)
                    {
                        ila.FilesTotalCount = 0;
                        ila.FilesReadedCount = 0;
                    }
                    //清空缓存
                    Cache.Cache.OpersList.Clear();
                    ConfigManager.Init();
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }).Start();
        }
        public static int FilesTotalCount
        {
            get
            {
                
                ILogAnalyser ila = TaskScheduler.Instance.CurrentLogAnalyser;
                if (ila != null) return ila.FilesTotalCount;
                

                return GetFilesTotalCount();
            }
        }
        public static int FilesReadedCount
        {
            get
            {

                ILogAnalyser ila = TaskScheduler.Instance.CurrentLogAnalyser;
                if (ila != null) return ila.FilesReadedCount;

                return 0;
            }
        }

        /// <summary>
        /// 获取需要解析的所有文件
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private static int GetFilesTotalCount()
        {
            return FilesManager.GetFiles(ConfigManager.GetCurrentConfig()).Count;
        }
    }
}
