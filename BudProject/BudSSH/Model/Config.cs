using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudSSH.Model
{
    /// <summary>
    /// 用于承载配置 xcw 20140821
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 路径配置 xcw 20140821
        /// </summary>
        public PathConfig Path { get; set; }

        /// <summary>
        /// 数据库配置 xcw 20140821
        /// </summary>
        public DBConfig DB { get; set; }

        public string ReadLogTime { get; set; }
        /// <summary>
        /// 同步时间间隔
        /// </summary>
        public string SynchronizingTimeInterval { get; set; }

        /// <summary>
        /// 同步数据库 ssh输出路径 mac路径
        /// 时间
        /// </summary>
        public string SSHLocalSyncTime { get; set; }

        /// <summary>
        /// 数据库同步时间
        /// </summary>
        public string DBSyncTime { get; set; }

        /// <summary>
        /// BudSSH程序启动时间
        /// </summary>
        public string SSHBootTime { get; set; }


    }

    /// <summary>
    /// 路径配置 xcw 20140821
    /// </summary>
    public class PathConfig
    {
        public string InputLogPath { get; set; }
        public string OutputPath { get; set; }
    }

    /// <summary>
    /// 数据库配置 xcw 20140821
    /// </summary>
    public class DBConfig
    {
        public string ServerName { get; set; }
        public string LoginName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        public string DatabaseName { get; set; }

        /// <summary>
        /// get the connection string 
        /// </summary>
        public string ConnString
        {
            get
            {
                StringBuilder sb = new StringBuilder("server=@ServerName;uid=@LoginName;pwd=@Password;database=@DatabaseName;");
                sb.Replace("@ServerName", ServerName).Replace("@LoginName", LoginName).Replace("@Password", Password).Replace("@DatabaseName", DatabaseName);
                return sb.ToString();
            }
        }
    }

}
