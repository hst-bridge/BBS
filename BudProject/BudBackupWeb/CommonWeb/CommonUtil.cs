using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace budbackup.CommonWeb
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
        public static String getDateTime(DateTime dt, String format)
        {
            //String date = string.Format("{0:s}", dt);
            return dt.ToString(format);
        }
        /// <summary>
        /// メッセージを取得
        /// </summary>
        /// <param name="strMsgKey">メッセージ キー</param>
        /// <returns>メッセージ内容</returns>
        public static string getMessage(string strMsgKey)
        {
            string strMsg = ConfigurationManager.AppSettings[strMsgKey.Trim()];
            return strMsg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMsgKey">メッセージ キー</param>
        /// <param name="para">パラメータ</param>
        /// <returns>メッセージ内容</returns>
        public static string getMessage(string strMsgKey, string para)
        {
            string strMsg = string.Empty;
            strMsg = ConfigurationManager.AppSettings[strMsgKey.Trim()];
            strMsg = strMsg.Replace("{1}", para);
            return strMsg;
        }

        public static string showMessage(string strMsgKey,string para="")
        {
            string strShowMsg = "<script type='text/javascript'>alert('@msg');</script>";
            if (para.Trim() != "")
            {
                strShowMsg = strShowMsg.Replace("@msg", getMessage(strMsgKey));
            }
            else
            {
                strShowMsg = strShowMsg.Replace("@msg", getMessage(strMsgKey,para));
            }
            return strShowMsg;
        }
        public static bool IsExceptFile(string strfile, string strExceptAttribute1, string strExceptAttribute2, string strExceptAttribute3)
        {
            strfile = strfile.ToLower();
            strExceptAttribute1 = strExceptAttribute1.ToLower();
            strExceptAttribute2 = strExceptAttribute2.ToLower();
            strExceptAttribute3 = strExceptAttribute3.ToLower();
            bool bIsExceptFile = false;
            if (strExceptAttribute1 != "")
            {
                bIsExceptFile = strfile.EndsWith(strExceptAttribute1);
            }
            if (!bIsExceptFile && strExceptAttribute2 != "")
            {
                bIsExceptFile = strfile.EndsWith(strExceptAttribute2);
            }
            if (!bIsExceptFile && strExceptAttribute3 != "")
            {
                bIsExceptFile = strfile.EndsWith(strExceptAttribute3);
            }
            return bIsExceptFile;
        }

        public static string LoginId = string.Empty;

        /// <summary>
        /// Get NetRelatedIP in Web.config
        /// 用于替换对应的IP
        /// </summary>
        ///2014-06-13 wjd add
        /// <returns></returns>
        public static Dictionary<string, string> GetNetRelatedIP()
        {
            Dictionary<string, string> ralate = new Dictionary<string, string>();
            try
            {
                string ips = ConfigurationManager.AppSettings["NetRelatedIP"];
                if (!String.IsNullOrWhiteSpace(ips))
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