using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;

namespace BudSSH.Common.Util
{
    /// <summary>
    /// 专门用于exe.config配置的保存与获取
    /// </summary>
    class ExeConfigUtil
    {

        public static string GetAPPSettingValue(string key)
        {
            string value = string.Empty;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");
            XmlElement xElem1 = (XmlElement)xDoc.SelectSingleNode("//appSettings/add[@key='" + key + "']");
            if (xElem1 != null)
            {
                value = xElem1.GetAttribute("value");
            }

            return value;
        }
        /// <summary>
        /// 获取数据库配置
        /// </summary>
        /// <param name="name"></param>
        public static string GetConnectionString(string name)
        {
            string connstr = string.Empty;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");
            XmlElement xElem1 = (XmlElement)xDoc.SelectSingleNode("//connectionStrings/add[@name='" + name + "']");
            if (xElem1 != null)
            {
                connstr = xElem1.GetAttribute("connectionString");
            }

            return connstr;
        }

        public static string AppSetting(string key)
        {
            string ret = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings[key] != null)
                ret = ConfigurationManager.AppSettings[key];
            return ret;
        }
        /// <summary>
        /// 保存数据库配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        public static void SetConnectionString(string name, string connectionString, string providerName = "System.Data.SqlClient")
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");
            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//connectionStrings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@name='" + name + "']");
            if (xElem1 != null)
            {
                xElem1.SetAttribute("connectionString", connectionString);
                xElem1.SetAttribute("providerName", providerName);
            }
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("name", name);
                xElem2.SetAttribute("connectionString", connectionString);
                xElem2.SetAttribute("providerName", providerName);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(System.Windows.Forms.Application.ExecutablePath + ".config");
        }

        ///<summary>  
        ///向.config文件的appKey结写入信息AppValue   保存设置  
        ///</summary>  
        ///<param name="AppKey">节点名</param>  
        ///<param name="AppValue">值</param>
        public static void SetAPPSettingValue(String AppKey, String AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");
            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null)
                xElem1.SetAttribute("value", AppValue);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", AppKey);
                xElem2.SetAttribute("value", AppValue);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(System.Windows.Forms.Application.ExecutablePath + ".config");
        }
    }
}
