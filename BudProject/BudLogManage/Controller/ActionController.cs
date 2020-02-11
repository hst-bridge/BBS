using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.BLL;
using BudLogManage.Model;
using BudLogManage.BLL.Cache;

namespace BudLogManage.Controller
{
    /// <summary>
    /// MVC 控制层 用于直接与View层对话
    /// </summary>
    class ActionController
    {
        /// <summary>
        /// 启动任务调度器
        /// </summary>
        public void Start()
        {
            //加载配置
            ConfigManager.Init();

            //任务调度器启动
           TaskScheduler.Instance.Execute();
        }

        /// <summary>
        /// 停止任务调度器
        /// </summary>
        public void Stop()
        {
            TaskScheduler.Instance.Stop();
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        public Status GetStatus()
        {
            Status status = new Status()
                {
                    FilesTotalCount = StatusManager.FilesTotalCount,
                     FilesReadedCount = StatusManager.FilesReadedCount
                }; 
            
            return status;
        }
    }
}
