using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudFileCheckListen.Entities;
using System.IO;

namespace BudFileCheckListen.Models
{
    class FileArgs
    {
        public monitorServer MonitorServer { get; set; }
        public FileSystemEventArgs FileSystemEventArgs { get; set; }
    }
}
