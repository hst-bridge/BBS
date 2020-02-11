using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudFileCheckListen.Entities;
using System.IO;
using System.Reflection;
using log4net;
using BudFileCheckListen.Models;

namespace BudFileCheckListen.BLL
{
    /// <summary>
    /// 用于接收并处理FileSystemWatcher的事件
    /// 介于有可能事件众多 特设置一个队列用于存储消息
    /// 并开一个线程去处理
    /// </summary>
    public class FileListenResolver
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private monitorServer ms = null;
       

        private static FileQueueProxy fqproxy = FileQueueProxy.Instance;
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="ms">监控的服务器</param>
        public FileListenResolver(monitorServer ms)
        {
            this.ms = ms;
        }
        

        /// <summary>
        /// 设置响应事件
        /// 由于window操作系统处理消息的方式是同步的（即响应一个之后才会响应第二个）
        /// </summary>
        /// <param name="fsw"></param>
        public void SetEvent(FileSystemWatcher fsw)
        {
           
            //监听 新建 改变 删除
            fsw.Created += new FileSystemEventHandler(OnProcess);
            fsw.Changed += new FileSystemEventHandler(OnProcess);
            fsw.Deleted += new FileSystemEventHandler(OnProcess);
            fsw.Error += new ErrorEventHandler(fsw_Error);
        }

        /// <summary>
        /// FileSystemWatcher 异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fsw_Error(object sender, ErrorEventArgs e)
        {
            logger.Warn(this.ms.monitorLocalPath + ":" + e.GetException().Message);
        }

        public void OnProcess(object source, FileSystemEventArgs e)
        {
            bool isOk = fqproxy.Enqueue(new FileArgs() { FileSystemEventArgs=e, MonitorServer=ms });
            if (!isOk)
            {
                //记录一些被忽略的事件
                Common.LogManager.WriteLog(Common.LogFile.Warning,"ignored:" + e.ChangeType.ToString()+" ;"+ e.FullPath);
            }
        }

        /// <summary>
        /// 此函数用于自定义事件
        /// 
        /// 返回值用于判断是否容量已经很大
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool OnProcess(FileSystemEventArgs e)
        {
            return fqproxy.Enqueue(new FileArgs() { FileSystemEventArgs = e, MonitorServer = ms });
        }


    }
}
