using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace budbackup.Models
{
    /// <summary>
    /// for download
    /// </summary>
    public class MonitorServer
    {
        public string ID { get; set; }
        public string DBServerIP { get; set; }
        public string MonitorLocalPath{get;set;}
        public string MonitorServerName { get; set; }
        public string MonitorServerIP { get; set; }
        public string StartFile { get; set; }
    }
}