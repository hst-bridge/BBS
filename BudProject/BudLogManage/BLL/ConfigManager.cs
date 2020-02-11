using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Model;
using BudLogManage.Common.Config;

namespace BudLogManage.BLL
{
    /// <summary>
    /// 用于全局配置存储内存
    /// 及更新到文件
    /// </summary>
   static class ConfigManager
    {
       private static ConfigLoader configLoader = new ConfigLoader();
        private static Config config = null;
        public static Config GetCurrentConfig()
        {
            if (config == null)
            {
                Init();
            }

            return config;
        }

        public static string GetLogPaths()
        {
            return configLoader.GetLogPaths();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public static void Init()
        {
            config = configLoader.LoadConfig();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="config"></param>
        public static bool SaveConfig(Config config)
        {
            ConfigSaver cs = new ConfigSaver();
            bool isUpdate = false;
            #region 判断配置路径是否发生变化，如果发生变化，则告诉状态管理器
            try
            {
                string[] prePaths = GetCurrentConfig().LogPath.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] currPaths = config.LogPath.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                string preFolder = GetCurrentConfig().Folder.Trim();
                string currFolder = config.Folder.Trim();

                if (prePaths.Count() == currPaths.Count() && preFolder == currFolder)
                {
                    foreach (string path in currPaths)
                    {
                        if (!prePaths.Contains(path))
                        {
                            isUpdate = true; break;
                        }
                    }
                }
                else
                {
                    isUpdate = true;
                }

                
            }
            catch (System.Exception)
            {
                isUpdate = false;
            }
            #endregion
            if (isUpdate)
            {
                bool status = cs.SaveConfig(config);
                if (status) StatusManager.SetPathUpdate();
                return status;
            }
            else return true;
        }
    }
}
