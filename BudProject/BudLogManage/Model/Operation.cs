using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudLogManage.Model
{
    /// <summary>
    /// 用于承载一次操作
    /// </summary>
    public class Operation
    {
        public Provider provider { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public string file { get; set; }
        public string option { get; set; }
        public string start { get; set; } //datetime
        public string end { get; set; } //datetime
        //操作概述
        //dirs
        public string dirs_total { get; set; }
        public string dirs_copied { get; set; }
        public string dirs_skipped { get; set; }
        public string dirs_mismatch { get; set; }
        public string dirs_failed { get; set; }
        public string dirs_extras { get; set; }
        //files
        public string files_total { get; set; }
        public string files_copied { get; set; }
        public string files_skipped { get; set; }
        public string files_mismatch { get; set; }
        public string files_failed { get; set; }
        public string files_extras { get; set; }
        //bytes
        public string bytes_total { get; set; }
        public string bytes_copied { get; set; }
        public string bytes_skipped { get; set; }
        public string bytes_mismatch { get; set; }
        public string bytes_failed { get; set; }
        public string bytes_extras { get; set; }

        public List<LogEntry> logEntrys { get; set; }
    }
}
