using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model;
using BudSSH.DAL.Config;

namespace BudSSH.BLL
{
    /// <summary>
    /// 管理配置
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// 获取当前配置
        /// </summary>
        /// <returns></returns>
        public static Config GetCurrentConfig()
        {
            ConfigLoader configLoader = new ConfigLoader();

            return configLoader.LoadConfig();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="config"></param>
        public static bool SaveConfig(Config config)
        {
            ConfigSaver cs = new ConfigSaver();

            return cs.SaveConfig(config);
            
        }
    }
}
