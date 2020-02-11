using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Common
{
    public class LogManager
    {
        private static string logPath = string.Empty;
        private LogManager() { }
        private static LogManager instance = new LogManager();

        public static LogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogManager();
                }
                return instance;
            }
        }
        /// <summary>
        /// save log folder
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    logPath = AppDomain.CurrentDomain.BaseDirectory + @"Log\";
                }
                return logPath;
            }
            set { logPath = value; }
        }

        private static string logFielPrefix = string.Empty;
        /// <summary>
        /// log file prefix
        /// </summary>
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        /// <summary>
        /// write log
        /// </summary>
        public static void WriteLog(string logFile, string msg)
        {
            try
            {
                lock (Instance)
                {
                    if (!Directory.Exists(LogPath))
                    {
                        Directory.CreateDirectory(LogPath);
                    }
                    string FileName = LogPath + LogFielPrefix + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    if (!System.IO.File.Exists(FileName))
                    {
                        FileStream tempfs = File.Create(FileName);
                        tempfs.Close();
                    }
                    System.IO.StreamWriter sw = System.IO.File.AppendText(FileName);
                    StringBuilder sb = new StringBuilder();
                    //sb.Append("---------------------------------------------------------------------\r\n");
                    sb.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  [" + logFile + "]  ");
                    sb.Append(msg);
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (System.IO.IOException IoEx)
            {
                WriteLog(LogFile.ERROR, IoEx.ToString());
            }
        }
        /// <summary>
        /// write log
        /// </summary>
        public static void WriteLog(string logFile, string msg, string fileName)
        {
            try
            {
                lock (Instance)
                {
                    if (!Directory.Exists(LogPath))
                    {
                        Directory.CreateDirectory(LogPath);
                    }
                    string FileName = LogPath + LogFielPrefix + DateTime.Now.ToString("yyyyMMdd") + fileName + ".txt";
                    if (!System.IO.File.Exists(FileName))
                    {
                        FileStream tempfs = File.Create(FileName);
                        tempfs.Close();
                    }
                    System.IO.StreamWriter sw = System.IO.File.AppendText(FileName);

                    StringBuilder sb = new StringBuilder();
                    //sb.Append("---------------------------------------------------------------------\r\n");
                    sb.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  [" + logFile + "]  ");
                    sb.Append(msg);
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (System.IO.IOException IoEx)
            {
                WriteLog(LogFile.ERROR, IoEx.ToString());
            }
        }
        /// <summary>
        /// write log
        /// </summary>
        public static void WriteLog(LogFile logFile, string msg)
        {
            WriteLog(logFile.ToString(), msg);
        }
        /// <summary>
        /// write log
        /// </summary>
        public static void WriteLog(LogFile logFile, string msg, string fileName)
        {
            WriteLog(logFile.ToString(), msg, fileName);
        }
    }

    /// <summary>
    /// log type
    /// </summary>
    public enum LogFile
    {
        TRACE,
        WARNING,
        ERROR,
        DEBUG,
        SQL,
        SYSTEM
    }
}
