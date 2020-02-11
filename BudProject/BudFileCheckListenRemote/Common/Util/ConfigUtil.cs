using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace BudFileCheckListen.Common.Util
{
    public class ConfigUtil
    {
        public static string AppSetting(string key)
        {
            string ret = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings[key] != null)
                ret = ConfigurationManager.AppSettings[key];
            return ret;
        }
    }
}