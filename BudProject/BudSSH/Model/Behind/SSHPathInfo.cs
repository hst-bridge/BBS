using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudSSH.Model.Behind
{
    /// <summary>
    /// SSH PathInfo
    /// </summary>
    class SSHPathInfo
    {
        public string ID { get; set; }
        /// <summary>
        /// Mac IP
        /// </summary>
        public string MonitorServerIP { get; set; }
        /// <summary>
        /// Mac 路径
        /// </summary>
        public string MacPath { get; set; }
        /// <summary>
        /// 路径最后一层名字
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// 深度
        /// </summary>
        public int depth { get; set; }
        /// <summary>
        /// 0 目录 1 文件
        /// </summary>
        public int typeflag { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int updateTime { get; set; }

    }
}
