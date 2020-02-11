using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using BudSSH.Common.Util;
using System.Configuration;

namespace BudSSH.DAL.Config
{
    /// <summary>
    /// 专门用于保存
    /// </summary>
   public class ConfigSaver
    {
       private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="config"></param>
       public bool SaveConfig(Model.Config config)
       {
           bool isOk = true;
           try
           {
               if (config != null)
               {
                   #region save config

                   if (config.Path != null)
                   {
                      SavePath(config.Path);
                   }

                   if (config.DB != null)
                   {
                       SaveDB(config.DB);
                   }
                   
                   #endregion
               }
           }
           catch (System.Exception ex)
           {
               logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
               isOk = false;
           }
           return isOk;
       }

       /// <summary>
       /// 保存数据库配置 xiecongwen 20140822
       /// </summary>
       /// <param name="dbconfig"></param>
       /// <returns></returns>
       private void SaveDB(Model.DBConfig dbconfig)
       {
           string settingName = "BudBackup2Context";
           string connstr = string.Format("server={0};uid={1};pwd={2};database={3};", dbconfig.ServerName, dbconfig.LoginName, dbconfig.Password, dbconfig.DatabaseName);

           ExeConfigUtil.SetConnectionString(settingName, connstr);
           
       }

       /// <summary>
       /// 保存路径信息 xiecongwen 20140822
       /// </summary>
       /// <param name="ptconfig"></param>
       /// <returns></returns>
       private void SavePath(Model.PathConfig ptconfig)
       {
           ExeConfigUtil.SetAPPSettingValue("LogPath", ptconfig.InputLogPath);
           ExeConfigUtil.SetAPPSettingValue("SSHPath", ptconfig.OutputPath);
           ConfigurationManager.RefreshSection("appSettings");
       }
    }
}
