using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BudErrorManage.Common.Util
{
    /// <summary>
    /// 文件系统工具类 如 make directory
    /// </summary>
    class FileSystemUtil
    {
        /// <summary>
        /// 创建目录 
        /// </summary>
        /// <param name="dir">绝对路径</param>
        public static void MakeDir(string dir)
        {
            if(!Directory.Exists(dir))Directory.CreateDirectory(dir);
        }

    }
}
