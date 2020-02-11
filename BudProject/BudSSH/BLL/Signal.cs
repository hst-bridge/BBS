using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudSSH.BLL
{
    public class Signal
    {
        public static bool IsSystemStoping { get; set; }

        /// <summary>
        /// 总任务数
        /// </summary>
        public static int TotalTaskCount = 0;

        /// <summary>
        /// 执行完成的任务数
        /// </summary>
        public static int CompletedTaskCount = 0;
    }
}
