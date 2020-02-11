using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BudFileCheckListen.Common.Util;
using log4net;
using System.Reflection;
using BudFileCheckListen.Models;

namespace BudFileCheckListen.BLL
{
    /// <summary>
    /// 问题：当前在每一个monitoserver对应的本地路径上实行FileSystemWatcher监控，照成queue线程过多，访问数据库的连接过多，以至于死锁
    /// 解决：使用代理模式及单例模式，保证只使用一个队列
    /// </summary>
    class FileQueueProxy
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private LogTableManager ltm = LogTableManager.Instance;
        private MFLTableManager mtm = MFLTableManager.Instance;
        private ReactiveQueue<FileArgs> rqueue = new ReactiveQueue<FileArgs>();
        private FileQueueProxy() {
            //设置监听消息上限
            rqueue.MaxFactor = 10;
            rqueue.DequeueHandler += new DequeueEventHandler<FileArgs>(rqueue_DequeueHandler);
        }

        public static readonly FileQueueProxy Instance = new FileQueueProxy();
        /// <summary>
        ///ReactiveQueue
        /// </summary>
        /// <param name="item"></param>
        void rqueue_DequeueHandler(List<FileArgs> items)
        {
            try
            {
                //用于处理重复消息 （当更新文件内容时，会产生多个消息）
                var knownKeys = new HashSet<string>();
                //依次处理消息
                foreach (var item in items)
                {
                    //判断消息是否有价值
                    if (!FileListenHelper.IsMessageValuable(item.FileSystemEventArgs)) continue;

                    if (knownKeys.Add(item.FileSystemEventArgs.Name))
                    {
                        ltm.Handle(item.MonitorServer, item.FileSystemEventArgs);
                        mtm.Handle(item.MonitorServer, item.FileSystemEventArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex,""));
            }
        }

        public bool Enqueue(FileArgs fa)
        {
            return rqueue.Enqueue(fa);
        }
    }
}
