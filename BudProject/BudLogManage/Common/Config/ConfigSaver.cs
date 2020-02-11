using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using BudLogManage.Common.Util;

namespace BudLogManage.Common.Config
{
    /// <summary>
    /// 专门用于保存
    /// </summary>
   public class ConfigSaver
    {
       private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
       private static string configPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\logConfig.xml";
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
                   SetLogPath(config.LogPath);
                   SetFolder(config.Folder);
               }
           }
           catch (System.Exception ex)
           {
               logger.Error(ex.Message);
               isOk = false;
           }
           return isOk;
       }

       public void SetLogPath(string path)
       {
           XMLUtil.SetInnerText("/LogAnalyzerConfig/LogPath", configPath,path);
       }

       public void SetFolder(string folder)
       {
           XMLUtil.SetInnerText("/LogAnalyzerConfig/Folder", configPath, folder);
       }

    }
}
