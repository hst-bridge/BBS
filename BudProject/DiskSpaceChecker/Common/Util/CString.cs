using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskSpaceChecker.Common.Util
{
    public static class CString
    {
        /// <summary>
        /// Returns an application setting based on the passed in string
        /// used primarily to cut down on typing
        /// </summary>
        /// <param name="Key">The name of the key</param>
        /// <returns>The value of the app setting in the web.Config
        //     or String.Empty if no setting found</returns>
        public static string AppSetting(this string Key)
        {
            string ret = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings[Key] != null)
                ret = ConfigurationManager.AppSettings[Key];
            return ret;
        }

        public static void AppSetting(this string Key,string Value)
        {
            string ret = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings[Key] != null)
            {
                //Create the object
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings[Key].Value = Value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }

        }
    }
}
