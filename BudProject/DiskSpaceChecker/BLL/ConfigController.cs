using DiskSpaceChecker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiskSpaceChecker.Common.Util;

namespace DiskSpaceChecker.BLL
{
    /// <summary>
    /// 配置
    /// </summary>
    class ConfigController
    {
        public void Save(Config config)
        {
            if (config != null)
            {
                if (!string.IsNullOrWhiteSpace(config.Volume))
                {
                    "volume".AppSetting(config.Volume);
                }

                if (!string.IsNullOrWhiteSpace(config.Baseline))
                {
                    "baseline".AppSetting(config.Baseline);
                }
            }
        }

        public Config Get()
        {
            Config config = new Config();
            config.Volume = "volume".AppSetting();
            config.Baseline = "baseline".AppSetting();
            return config;
        }
    }
}
