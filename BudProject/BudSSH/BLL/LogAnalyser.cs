using System;
using System.Collections.Generic;
using BudSSH.DAL.IBLL;
using System.IO;
using System.Reflection;
using log4net;
using BudSSH.Model;
using BudSSH.Common.Util;
using BudSSH.DAL.LogFile;
using System.Text.RegularExpressions;
using System.Collections;
using BudSSH.Common.Helper;

namespace BudSSH.BLL
{
    /// <summary>
    /// 用于解析日志
    /// </summary>
    public class LogAnalyser : ILogAnalyser
    {

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private static volatile object _lock = new object();
        /// <summary>
        /// 处理文件 暂时只考虑Robocopy
        /// 
        /// 构思：读日志、预处理、解析、存数据库
        /// </summary>
        /// <param name="fpath"></param>
        public void Analyse(Config config)
        {
            lock (_lock)
            {
                try
                {
                    //判断当前时间与配置时间是否匹配
                    if (TimeCheckHelper.CheckTime("readLogTime", config.ReadLogTime))
                    {
                        Common.Util.LogManager.WriteLog(Common.Util.LogFile.LogAnalyser, "start");

                        #region process
                        FileSystemUtil fsu = new FileSystemUtil();

                        //获取所有日志文件
                        IEnumerable<FileInfo> copyLogList = GetFileInfos(config.Path.InputLogPath);
                        if (copyLogList == null)
                        {
                            //执行了一个任务
                            Signal.CompletedTaskCount++;

                            Common.Util.LogManager.WriteLog(Common.Util.LogFile.LogAnalyser, "copyLogList is null");
                            return;
                        }

                        foreach (FileInfo fileinfo in copyLogList)
                        {
                            Common.Util.LogManager.WriteLog(Common.Util.LogFile.LogAnalyser,fileinfo.FullName);
                            #region 处理日志文件
                            ILogReader ilr = null;
                            try
                            {
                                //获取出错文件
                                ilr = new RoboCopyLogReader() { Path = fileinfo.FullName };
                                ErrorPathInfo epi = ilr.GetErrorList();
                                if (epi != null && epi.PathList != null && epi.PathList.Count > 0)
                                {
                                    //预先过滤不必要文件
                                    ErrorPathFilter.PreFilter(epi);
                                    if (epi.PathList.Count > 0)
                                    {
                                        //获取文件详细信息，然后判断是否更新，若更新则执行copy
                                        SSHCopyManager scm = new SSHCopyManager();
                                        scm.SSHCopy(config, epi);
                                    }
                                }

                            }
                            catch (System.Exception ex)
                            {
                                Common.Util.LogManager.WriteLog(Common.Util.LogFile.LogAnalyser, MessageUtil.GetExceptionMsg(ex, ""));
                                break;
                            }
                            finally
                            {
                                //释放资源
                                if (ilr != null) ilr.Dispose();
                            }
                            #endregion

                            //删除日志文件
                            fsu.DeleteFile(fileinfo);
                        }
                        #endregion

                        Common.Util.LogManager.WriteLog(Common.Util.LogFile.LogAnalyser, "end");

                        //执行了一个任务
                        Signal.CompletedTaskCount++;
                    }
                }
                catch(System.Exception ex)
                {
                    Common.Util.LogManager.WriteLog(Common.Util.LogFile.LogAnalyser, MessageUtil.GetExceptionMsg(ex, ""));
                   
                }

            }


        }

        

        /// <summary>
        /// 获取符合条件的日志文件信息
        /// </summary>
        /// <returns></returns>
        private FileInfo[] GetFileInfos(string logPath)
        {
            try
            {
                FileSystemUtil fsu = new FileSystemUtil();

                string ydirPath = logPath + @"\" + "RoboCopyLogToCopyBK-" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                string tdirPath = logPath + @"\" + "RoboCopyLogToCopyBK-" + DateTime.Now.ToString("yyyy-MM-dd");
                // 昨日と一昨日のファイル処理
                DirectoryInfo yesterdayLogDir = new DirectoryInfo(ydirPath);
                if (yesterdayLogDir.Exists)
                {
                    #region 将昨天目录中未处理的日志文件复制到今天目录中,最后清空昨天目录
                    // ファイルリスト
                    FileInfo[] yesterdayLogFileInfoList = yesterdayLogDir.GetFiles("*.log", SearchOption.TopDirectoryOnly);
                    if (yesterdayLogFileInfoList.Length > 0)
                    {
                        fsu.MakeDir(tdirPath);
                        #region copy to today dir
                        foreach (FileInfo yesterdayLogFileInfo in yesterdayLogFileInfoList)
                        {
                            try
                            {
                                string yesterdayLogFileBKPath = Path.Combine(
                                    tdirPath,
                                    yesterdayLogFileInfo.FullName.Substring(yesterdayLogFileInfo.FullName.LastIndexOf(@"\") + 1));

                                yesterdayLogFileInfo.CopyTo(yesterdayLogFileBKPath, true);
                            }
                            catch (System.Exception ex)
                            {
                                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                                continue;
                            }
                        }
                        #endregion
                        //clear yesterday dir
                        fsu.ClearDirectory(yesterdayLogDir.FullName);
                    }
                    #endregion
                }

                DirectoryInfo copyLogDir = new DirectoryInfo(tdirPath);
                if (!copyLogDir.Exists) return null;

                return copyLogDir.GetFiles("*.log", SearchOption.TopDirectoryOnly);
            }
            catch (System.Exception ex)
            {
                Common.Util.LogManager.WriteLog(Common.Util.LogFile.LogAnalyser, MessageUtil.GetExceptionMsg(ex, ""));
            }

            return null;
        }


    }
}
