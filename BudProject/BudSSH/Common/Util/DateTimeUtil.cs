using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace BudSSH.Common.Util
{
    /// <summary>
    /// 用于生成datetime
    /// </summary>
    public class DateTimeUtil
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 根据日期 和 时间 生成DateTime对象
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(string date, string time)
        {
            string[] dateParas = date.Split(new string[] { "年", "月", "日" }, StringSplitOptions.RemoveEmptyEntries);
            string[] timeParas = time.Split(':');
            return new DateTime(Convert.ToInt32(dateParas[0]), Convert.ToInt32(dateParas[1]), Convert.ToInt32(dateParas[2])
                                 , Convert.ToInt32(timeParas[0]), Convert.ToInt32(timeParas[1]), Convert.ToInt32(timeParas[2]));
        }

        public static DateTime? GetDateTime(string datetime)
        {
            try
            {
                string[] paras = datetime.Split(' ');
                return GetDateTime(paras[0], paras[1]);
            }
            catch (System.Exception ex)
            {
                logger.Info(MessageUtil.GetExceptionMsg(ex, ""));
            }

            return null;

        }

        /// <summary>
        /// 時間変換関数
        /// </summary>
        /// <param name="time_t"></param>
        public static DateTime Time_T2DateTime(uint time_t)
        {
            long win32FileTime = 10000000 * (long)time_t + 116444736000000000;
            return DateTime.FromFileTimeUtc(win32FileTime).ToLocalTime();
        }

        /// <summary>
        /// 获取计划任务需要的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetTaskFormatTime(DateTime dateTime)
        {
            string format = "yyyy-MM-ddTHH:mm:ss";
            if (dateTime != null)
            {
                return dateTime.ToString(format);
            }
            else
            {
                return DateTime.Now.ToString(format);
            }
        }
    }
}
