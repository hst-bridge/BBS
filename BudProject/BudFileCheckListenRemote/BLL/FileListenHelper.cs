using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BudFileCheckListen.Common;
using BudFileCheckListen.Common.FileSystem;
using System.Diagnostics;
using BudFileCheckListen.Entities;

namespace BudFileCheckListen.BLL
{
    /// <summary>
    /// 用于辅助FileListen 
    /// 承担部分逻辑
    /// </summary>
   public static class FileListenHelper
    {

        #region 用于过滤监听消息类型
        private static string[] excepts = null;
        public static string[] Excepts
        {
            get
            {
                if (excepts == null)
                {
                    excepts = new string[] { "DS_Store", "com.apple.timemachine.supported","Icon"};
                }

                return excepts;
            }
        }
        #endregion
        /// <summary>
        /// 检查target目录所在盘区空间是否充足
        /// </summary>
        /// <returns></returns>
        public static bool CheckDisk(DirectoryInfo targetDirInfo)
        {
            //是否大于设置线
            return DiskUtil.GetFreeSpace(targetDirInfo.Root.FullName) > new AppConfig().getSpaceWarnline();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="targetDirInfo"></param>
        /// <param name="ms"></param>
        public static void SendMail(DirectoryInfo targetDirInfo, Entities.monitorServer ms)
        {
            double line = new AppConfig().getSpaceWarnline();
            StringBuilder strB = new StringBuilder();

            strB.Append("\"").Append(targetDirInfo.Root.FullName).Append("\": This disk space is insufficient.\r\n     There are less than ").Append(line).Append("G space left.\r\n\r\n")
                .Append("monitorServer info:\r\n")
                .Append("  monitorID: ").Append(ms.id).Append(";\r\n")
                .Append("  monitorServerName: ").Append(ms.monitorServerName).Append(";\r\n")
                .Append("  monitorServerIP: ").Append(ms.monitorServerIP).Append(";\r\n")
                .Append("  memo: ").Append(ms.memo).Append(";\r\n")
                .Append("  startFile: ").Append(ms.startFile).Append(";\r\n")
                .Append("  monitorMacPath: ").Append(ms.monitorMacPath).Append(";\r\n")
                .Append("  monitorLocalPath: ").Append(ms.monitorLocalPath).Append(".\r\n");
            //发送
            new SendMail().SendEmail(strB.ToString());
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="content"></param>
        public static void SendMail(string content,string subject)
        {
            new SendMail().SendEmailWithSubject(content,subject);
        }

        /// <summary>
        /// 用于过滤一些不重要的文件类型消息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsMessageValuable(FileSystemEventArgs item)
        {
            foreach (var except in Excepts)
            {
                if (item.Name.Contains(except))
                {
                    return false;
                }
            }
           
            return true;
        }

        /// <summary>
        /// 将文件系统信息插入到数据库
        /// </summary>
        public static void BackupFSWithoutCheck(monitorServer ms, FileListenResolver flr)
        {
            DirectoryInfo targetDirInfo = new DirectoryInfo(ms.monitorLocalPath);

           

            //遍历文件列表
            foreach (var targetFileInfo in targetDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                bool isOk = false;
                /*此处向flr发送自制消息
                 * 
                 * targetFileInfo.FullName.Substring(0, targetFileInfo.FullName.LastIndexOf('\\')) 是为了保留完整路径信息
                 * 默认情况下.net将怪异的路径修正，造成文件找不到
                 */
                string fullPath = FSUtil.GetFullPath(targetDirInfo);
            again: isOk = flr.OnProcess(new FileSystemEventArgs(WatcherChangeTypes.Created, ms.monitorLocalPath, fullPath.Substring(ms.monitorLocalPath.Length+1)));
                if (!isOk)
                {
                    //由于文件过多，需要等待一下
                    System.Threading.Thread.Sleep(2000);
                    goto again;
                }
            }
        }
      
    }
}
