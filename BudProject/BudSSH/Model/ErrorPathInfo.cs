using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Common.Helper;

namespace BudSSH.Model
{
    /// <summary>
    /// 用于封装出错路径 及其元信息
    /// </summary>
    public class ErrorPathInfo
    {
        public string source { get; set; }
        public string destination { get; set; }
        public string file { get; set; }
        public string option { get; set; }
        public string start { get; set; } //datetime
        public string end { get; set; } //datetime

        public List<ErrorEntry> PathList { get; set; }

    }
    /// <summary>
    /// 日志实体
    /// </summary>
    public class ErrorEntry
    {
       
        public string Path { get; set; }

        //时间
        public string Time { get; set; }
        //原因
        public string Cause { get; set; }


    }
}
