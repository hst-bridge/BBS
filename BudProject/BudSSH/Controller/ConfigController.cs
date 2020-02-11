using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model;
using BudSSH.BLL;
using BudSSH.Common.Helper;

namespace BudSSH.Controller
{
    /// <summary>
    /// 配置管理控制器
    /// </summary>
    public class ConfigController
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public Config LoadConfig()
        {
            return ConfigManager.GetCurrentConfig();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="config"></param>
        public bool SaveConfig(Config config)
        {
            return ConfigManager.SaveConfig(config);
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="dbconfig"></param>
        /// <returns></returns>
        public bool TestDBServer(DBConfig dbconfig)
        {
            DBHelper dbhelper = new DBHelper();
            return dbhelper.LinkTest(dbconfig);
        }
    }
}
