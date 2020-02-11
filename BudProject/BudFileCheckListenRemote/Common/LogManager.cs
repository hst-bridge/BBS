using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BudFileCheckListen.Common
{
    /// <summary>
    /// 日志管理工具，专门用来写入系统日志到txt文件中
    /// </summary>
    public class LogManager
    {
        /// <summary>
        /// 系统日志存放的路径
        /// </summary>
        private static string logPath = string.Empty;
        /// <summary>
        /// 保存日志的文件夹
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    logPath = AppDomain.CurrentDomain.BaseDirectory + @"log\";
                }
                return logPath;
            }
            set { logPath = value; }
        }
        /// <summary>
        /// 日志文件前缀
        /// </summary>
        private static string logFielPrefix = string.Empty;
        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logFile">日志名称</param>
        /// <param name="msg">日志信息</param>
        public static void WriteLog(string logFile, string msg)
        {
            System.IO.StreamWriter sw = null;
            try
            {
                //如果不存在日志指定路径，则创建
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }

                //创建日志文件的名称 前缀+文件名+时间（年月日） 如20111011
                string FileName = LogPath + LogFielPrefix + logFile + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                //如果日志文件不存在，则创建
                if (!System.IO.File.Exists(FileName))
                {
                    FileStream tempfs = File.Create(FileName);
                    tempfs.Close();
                }
                //创建一个写入文件流，用来在文件末尾写入内容 
                while (true)
                {
                    try
                    {
                        sw = System.IO.File.AppendText(FileName);
                        break;
                    }
                    catch (Exception)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                //创建日志内容
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss "));
                sb.Append(msg);

                //写入日志内容
                sw.WriteLine(sb.ToString());
            }
            //捕获写入过程中的异常
            catch (System.IO.IOException IoEx)
            {
                //将异常写入到警告文件中
                WriteLog(LogFile.Warning, IoEx.ToString());
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Dispose();

                }
            }
        }

        /// <summary>
        /// 测试某个文件是否正在被使用
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public bool CheckIfFileIsBeingUsed(string fileName)
        {

            FileStream fsTest = null;
            try
            {
                fsTest = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                fsTest.Dispose();
            }
            catch (Exception)
            {
                return true;
            }
            finally
            {
                if(fsTest != null)
                {
                    fsTest.Dispose();
                }
            }

            return false;

        }

        /// <summary>
        /// 写日志(重载)
        /// </summary>
        /// <param name="logFile">日志名称</param>
        /// <param name="msg">日志信息</param>
        /// <param name="logPath">日志路径</param>
        public static void WriteLog(string logFile, string msg, string logPath)
        {
            try
            {
                //如果不存在日志指定路径，则创建
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                //创建日志文件的名称 前缀+文件名+时间（年月日） 如20111011
                string FileName = logPath + LogFielPrefix + logFile + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                //如果日志文件不存在，则创建
                if (!System.IO.File.Exists(FileName))
                {
                    FileStream tempfs = File.Create(FileName);
                    tempfs.Close();
                }
                //创建一个写入文件流，用来在文件末尾写入内容 
                System.IO.StreamWriter sw = System.IO.File.AppendText(FileName);

                //创建日志内容
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd"));
                sb.Append(msg);
                //写入日志内容
                sw.WriteLine(sb.ToString());

                //关闭文件流
                sw.Close();
            }
            //捕获写入过程中的异常
            catch (System.IO.IOException IoEx)
            {
                //将异常写入到警告文件中
                WriteLog(LogFile.Warning, IoEx.ToString());
            }
        }
        
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logFile">日志名称</param>
        /// <param name="msg">日志信息</param>
        public static void WriteLog(LogFile logFile, string msg)
        {
            WriteLog(logFile.ToString(), msg);
        }
        
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logFile">日志名称</param>
        /// <param name="msg">日志信息</param>
        /// <param name="logPath">日志路径</param>
        public static void WriteLog(LogFile logFile, string msg, string logPath)
        {
            WriteLog(logFile.ToString(), msg ,logPath);
        }

        /// <summary>
        /// 写异常日志，含内部异常
        /// </summary>
        /// <param name="logFile">日志名称</param>
        /// <param name="e">异常信息</param>
        public static void WriteLog(LogFile logFile, Exception e)
        {
            WriteLog(logFile.ToString(), e.Message);
            if (e.InnerException != null)
                WriteLog(logFile, e.InnerException);
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogFile
    {
        FSWDeleteEvent,
        /// <summary>
        /// 追踪日志
        /// </summary>
        Trace,
        /// <summary>
        /// 警告日志
        /// </summary>
        Warning,
        /// <summary>
        /// 错误日志
        /// </summary>
        Error,
        /// <summary>
        /// sql日志
        /// </summary>
        SQL,
        /// <summary>
        /// 普通日志
        /// </summary>
        log,
        /// <summary>
        /// 系统日志
        /// </summary>
        System,
        /// <summary>
        /// 业务日志
        /// </summary>
        Bussiness
    }
}
