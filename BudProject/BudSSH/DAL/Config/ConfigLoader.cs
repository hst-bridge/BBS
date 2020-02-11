using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Common.Util;
using System.Configuration;

namespace BudSSH.DAL.Config
{
    /// <summary>
    /// 用于加载配置 
    /// 主要是当前目录下的 logConfig.xml
    /// </summary>
    public  class ConfigLoader
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public Model.Config LoadConfig()
        {
            Model.Config config = new Model.Config();
            config.Path = new Model.PathConfig()
            {
                InputLogPath = ExeConfigUtil.GetAPPSettingValue("LogPath") ?? "",
                OutputPath = ExeConfigUtil.GetAPPSettingValue("SSHPath") ?? ""
            };
            string dbconnctionstring = ExeConfigUtil.GetConnectionString("BudBackup2Context");

            //待完善：应使用正则表达式校验格式,防止用户收到配置错误
            string[] paras = dbconnctionstring.Split(';');
            config.DB = new Model.DBConfig()
            {
                ServerName = paras[0].Split('=')[1],
                LoginName = paras[1].Split('=')[1],
                Password = paras[2].Split('=')[1],
                DatabaseName = paras[3].Split('=')[1]
            };

            //调度间隔
            config.SynchronizingTimeInterval = ExeConfigUtil.GetAPPSettingValue("SynchronizingTimeInterval") ?? "";

            //同步开始时间
            config.SSHLocalSyncTime = ExeConfigUtil.GetAPPSettingValue("SSHLocalSyncTime") ?? "";

            //read log time
            config.ReadLogTime = ExeConfigUtil.GetAPPSettingValue("readLogTime") ?? "";

            //DB Sync Time
            config.DBSyncTime = ExeConfigUtil.GetAPPSettingValue("DBSyncTime") ?? "";

            //SSH Boot Time
            config.SSHBootTime = ExeConfigUtil.GetAPPSettingValue("SSHBootTime") ?? "";

            return config;
        }

    }
}
