using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Configuration;

namespace Common
{
    public class CommonUtil
    {
        public static DateTime getFormatDate(string str)
        {
            return Convert.ToDateTime(str);
        }
        /// <summary>
        /// 格式化当前时间
        /// </summary>
        /// <returns></returns>
        public static string getDateTime(DateTime dt, string format)
        {
            //String date = string.Format("{0:s}", dt);
            return dt.ToString(format);
        }

        /// <summary>
        /// 格式化当前时间
        /// </summary>
        /// <returns></returns>
        public static string DateTimeNowToString()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// 格式化为短日期
        /// </summary>
        /// <returns></returns>
        public static string ToShortDateString(DateTime dt)
        {
            if (dt == null)
            {
                dt = new DateTime(1900, 1, 1);
            }
            return dt.ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// 格式化为长时间
        /// </summary>
        /// <returns></returns>
        public static string DateTimeToString(DateTime dt)
        {
            if (dt == null)
            {
                dt = new DateTime(1900, 1, 1);
            }
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// 格式化为短时间
        /// </summary>
        /// <returns></returns>
        public static string ToShortTimeString(string timeStr)
        {
            var time = string.Empty;
            DateTime result = DateTime.Now;
            if (DateTime.TryParse(timeStr, out result))
            {
                time = result.ToString("HH:mm");
            }
            return time;
        }

        /// <summary>
        /// 写操作
        /// </summary>
        /// <param name="AppKey"></param>
        /// <param name="AppValue"></param>
        public static void SetConfigValue(string AppKey, string AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            //获取可执行文件的路径和名称
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");
            XmlNode xNode;
            XmlElement xElem1;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", AppValue);
            xDoc.Save(System.Windows.Forms.Application.ExecutablePath + ".config");
        }

        /// <summary>
        /// Get LoginIP with single quote mark in Web.config
        /// </summary>
        ///2014-06-03 wjd add
        /// <returns></returns>
        public static string GetLoginIPWithQuote()
        {
            string ips = "";
            try
            {
                ips = ConfigurationManager.AppSettings["LoginIP"];
                ips = "'" + ips.Replace(" ", "").Replace("，", "','").Replace(",", "','") + "'";
            }
            catch (Exception e)
            {
                throw new Exception("There isn't a LoginIP in appSettings of Web.config. Please add one that joined with English comma(,) if more.", e);
            }
            return ips;
        }

        /// <summary>
        /// Get LoginIP in Web.config
        /// </summary>
        ///2014-06-04 wjd add
        /// <returns></returns>
        public static string GetLoginIP()
        {
            string ips = "";
            try
            {
                ips = ConfigurationManager.AppSettings["LoginIP"];
                ips = ips.Replace(" ", "").Replace("，", ",");
            }
            catch (Exception e)
            {
                throw new Exception("There isn't a LoginIP in appSettings of Web.config. Please add one that joined with English comma(,) if more.", e);
            }
            return ips;
        }

        /// <summary>
        /// Get LoginIP Condition
        /// </summary>
        /// 2014-06-04 wjd add
        /// <returns></returns>
        public static List<SearchCondition> GetLoginIPCondition(IList<SearchCondition> oldCondition)
        {
            List<SearchCondition> condition = new List<SearchCondition>();
            if (oldCondition != null && oldCondition.Count() > 0)
            {
                condition.AddRange(oldCondition);
            }

            string[] ips = GetLoginIP().Split(',');
            for (int i = 0; i < ips.Length; i++)
            {
                condition.Add(new SearchCondition { con = "DBServerIP = ?", param = "@DBServerIP" + i, value = ips[i], Operator = Operator.Or });
            }

            return condition;
        }

        /// <summary>
        /// Get LocalIP in Web.config
        /// </summary>
        ///2014-06-05 wjd add
        /// <returns></returns>
        public static string GetLocalIP()
        {
            string ips = "";
            try
            {
                ips = ConfigurationManager.AppSettings["LocalIP"];
                ips = ips.Replace(" ", "");
            }
            catch
            {
                ips = "";
            }
            return ips;
        }

        /// <summary>
        /// Get LocalCopyPath in Web.config
        /// </summary>
        ///2014-06-09 wjd add
        /// <returns></returns>
        public static string GetLocalCopyPath()
        {
            string path = "";
            try
            {
                path = ConfigurationManager.AppSettings["LocalCopyPath"];
                path = path.Replace(" ", "");
            }
            catch
            {
                path = "C:\\";
            }
            return path;
        }

        /// <summary>
        /// Get Transfer_IP_StartFolder in Web.config
        /// 用于転送先新規作成，IPアドレス与開始フォルダ对应设置
        /// </summary>
        ///2014-06-13 wjd add
        /// <returns></returns>
        public static Dictionary<string, string> GetTransfer_IP_StartFolder()
        {
            Dictionary<string, string> ralate = new Dictionary<string, string>();
            try
            {
                string ips = ConfigurationManager.AppSettings["Transfer_IP_StartFolder"];
                if (!string.IsNullOrWhiteSpace(ips))
                {
                    ips = ips.Replace(" ", "").Replace("：", ":").Replace("；", ";");
                    string[] ip = ips.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string p in ip)
                    {
                        string[] s = p.Split(':');
                        if (s.Length >= 2)
                        {
                            ralate.Add(s[0], s[1]);
                        }
                    }
                }
            }
            catch { }
            return ralate;
        }
    }
}