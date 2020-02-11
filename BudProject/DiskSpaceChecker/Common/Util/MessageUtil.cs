using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DiskSpaceChecker.Common.Util
{
    /// <summary>
    /// ヒントを得る
    /// </summary>
    class MessageUtil
    {
        /// <summary>
        /// get the message by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetMessage(string key)
        {
            return Properties.Resources.ResourceManager.GetString(key);
        }

        /// <summary>
        /// カスタム異常なメッセージを生成
        /// </summary>
        /// <param name="ex">異常なオブジェクト</param>
        /// <param name="backStr">予備の異常なメッセージ：効果的なEXがnull</param>
        /// <returns>異常な文字列のテキスト</returns>
        public static string GetExceptionMsg(System.Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(LocalIPAddress());
            sb.AppendLine("****************************Message****************************");
            sb.AppendLine("【DateTime】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【Type】：" + ex.GetType().Name);
                sb.AppendLine("【Message】：" + ex.Message);
                sb.AppendLine("【StackTrace】：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("【Unhandled exception】：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }

        public static string LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }
    }
}
