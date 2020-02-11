using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BudSSH.Common.Util
{
    /// <summary>
    /// 用于写SSH日志 成功或失败
    /// </summary>
    class SSHLogManager
    {
        private static string _baseLogPath = string.Empty;
        /// <summary>
        /// SSH日志文件基础路径
        /// </summary>
        public string BaseLogPath {
            get
            {
                if (string.IsNullOrWhiteSpace(_baseLogPath))
                {
                    
                    //从配置中获取路径信息
                    _baseLogPath = System.Configuration.ConfigurationManager.AppSettings["SSHLogPath"];
                    if (string.IsNullOrWhiteSpace(_baseLogPath))
                    {
                        //如果没有配置，则使用默认路径
                        _baseLogPath = @"\SSHLog";
                        LogManager.WriteLog(LogFile.Warning, "SSHLogPath doesn't set in appSettings section");
                    }

                    _baseLogPath = System.AppDomain.CurrentDomain.BaseDirectory + _baseLogPath;
                    
                }
                return _baseLogPath;
            }

            set
            {
                _baseLogPath = value;
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="sshLog"></param>
        public void WriteLog(SSHLog sshLog)
        {
            //解析日志内容
            string date = string.Empty;
            string message = Analyse(sshLog,out date);

            #region 确定日志路径
            string path = BaseLogPath + "\\" + date ;
            FileSystemUtil fsu = new FileSystemUtil();
            fsu.MakeDir(path);
            path += "\\" + date + "SSH-" + sshLog.LogType.ToString() + ".txt";
            #endregion
            try
            {
                //写日志
                using (var sw = new StreamWriter(path, true))
                {
                    sw.WriteLine(message);
                }
            }
            catch (System.Exception ex)
            {
                LogManager.WriteLogFirstLayer(LogFile.Error, ex);
            }
        }

        /// <summary>
        /// 解析日志
        /// </summary>
        /// <param name="sshLog"></param>
        /// <param name="date">日期 格式为20140711</param>
        /// <returns></returns>
        private string Analyse(SSHLog sshLog,out string date)
        {
            date = sshLog.DateTime.ToString("yyyyMMdd");
            string time = string.Format("{0:t}", sshLog.DateTime);
            string message = date + " " + time + " " + sshLog.Message;

            return message;
        }
    }
}
