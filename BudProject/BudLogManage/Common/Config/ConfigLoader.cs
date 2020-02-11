using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Common.Util;

namespace BudLogManage.Common.Config
{
    /// <summary>
    /// 用于加载配置 
    /// 主要是当前目录下的 logConfig.xml
    /// </summary>
    public  class ConfigLoader
    {
        private static string configPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\logConfig.xml";
        public  string GetLogPaths()
        {
           //获取配置
            string path = XMLUtil.GetInnerText("/LogAnalyzerConfig/LogPath", configPath);
           //return string.IsNullOrWhiteSpace(path) ? 
           //       //默认配置
           //       System.AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" : path;

            return path;
           
        }

        public string GetFolder()
        {
            //获取配置
            string folder = XMLUtil.GetInnerText("/LogAnalyzerConfig/Folder", configPath);
            return folder;
        }

        public Model.Config LoadConfig()
        {
            Model.Config config = new Model.Config();
            config.LogPath = GetLogPaths();
            config.Folder = GetFolder();
            return config;
        }

    }
}
