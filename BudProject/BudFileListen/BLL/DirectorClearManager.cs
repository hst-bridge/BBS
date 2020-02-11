using BudFileListen.Common;
using BudFileListen.Entities;
using BudFileListen.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace BudFileListen.BLL
{
    /// <summary>
    /// 用于清理job端没有的文件夹
    /// </summary>
    class DirectorClearManager
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void Clean()
        {
            string cleanTime = "18:00";
            if (!TimeCheckHelper.CheckTime("cleanTime", cleanTime)) return;
            try
            {
                //获取monitorservers
                MonitorServerManager monitorServerManager = new MonitorServerManager();
                List<monitorServer> monitorServerList = monitorServerManager.GetMonitorServerList();

                foreach (monitorServer ms in monitorServerList)
                {

                    //获取job端路径及本地路径
                    string jobPath = ms.monitorDrive;
                    string localPath = ms.monitorLocalPath;

                    //获取两者第一层子目录
                    List<string> jobPathChilds = new List<string>();
                    List<string> localPathChilds = new List<string>();
                    int nameIndexForjob = jobPath.Length + 1;
                    int nameIndexForlocal = localPath.Length + 1;
                    foreach(var jp in Directory.GetFileSystemEntries(jobPath)){
                        jobPathChilds.Add(jp.Substring(nameIndexForjob));
                    }
                    foreach (var lp in Directory.GetFileSystemEntries(localPath))
                    {
                        localPathChilds.Add(lp.Substring(nameIndexForlocal));
                    }

                    //找到并删除job没有的文件夹
                    foreach (string lpc in localPathChilds)
                    {
                        if (!jobPathChilds.Contains(lpc))
                        {
                            /**
                             * 删除
                             */
                            string path = localPath + Path.DirectorySeparatorChar + lpc;
                            logger.Info(path);
                            DeleteFSE(path);

                        }
                    }

                }

               
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteFSE(string path)
        {
            
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(path);
            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                bool isOk = true;
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception)
                {
                    isOk = false;
                }

                if (!isOk)
                {
                    DeleteDirectorySpecial(path);
                }
            }
            else
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }
        /// <summary>
        /// 当删除文件夹出错时 ,使用此方法
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteDirectorySpecial(string path)
        {
            Process robocopy = new Process();

            robocopy.StartInfo.CreateNoWindow = true;
            robocopy.StartInfo.UseShellExecute = false;
            robocopy.StartInfo.FileName = "DirectoryClean.exe";

            try
            {
                //判断空文件是否存在
                string tempPath = AppDomain.CurrentDomain.BaseDirectory + @"temp";
                if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);


                string commandLine = tempPath + " " + path + "  /purge";
                robocopy.StartInfo.Arguments = commandLine;
                robocopy.Start();
                while (true)
                {
                    Thread.Sleep(3000);
                    if (robocopy.HasExited)
                    {
                        Directory.Delete(path);
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            try
            {
                robocopy.Kill();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            
        }
    }
}
