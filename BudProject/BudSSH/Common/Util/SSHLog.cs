using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudSSH.Common.Util
{
    /// <summary>
    /// SSH日志类型
    /// </summary>
    enum SSHLogType{
        Success,
        Failure
    }
    /// <summary>
    /// 用于描述SSH日志model
    /// </summary>
    class SSHLog
    {
        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// SSH日志类型
        /// </summary>
        public SSHLogType LogType { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Message { get; set; }
    }
}
