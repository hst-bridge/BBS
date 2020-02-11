using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.BLL;

namespace BudSSH.Controller
{
    /// <summary>
    /// 用于处理操作 ：开始 停止
    /// </summary>
    public class OperationController
    {
        /// <summary>
        /// 开始执行 解析并Copy
        /// </summary>
        public void Start()
        {
            BLL.TaskScheduler.Instance.Execute();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            BLL.TaskScheduler.Instance.Stop();
        }
    }
}
