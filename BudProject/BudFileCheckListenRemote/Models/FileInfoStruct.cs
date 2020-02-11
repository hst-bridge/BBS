using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudFileCheckListen.Models
{
   public struct FileInfoStruct
    {
        public string Name;
        public string Extension;
        public string Length;
        public DateTime CreationTime;
        public DateTime LastWriteTime;
        public DateTime LastAccessTime;
    }
}
