using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Common.Helper;

namespace BudLogManage.Model
{
    /// <summary>
    /// 日志实体
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType LType { get; set; }

        public string Size { get; set; }

        public string Path { get; set; }
        #region Error 专用
        //时间
        public string Time { get; set; }
        //原因
        public string Cause { get; set; }
        #endregion
        public Operation operation { get; set; }
    }
}
