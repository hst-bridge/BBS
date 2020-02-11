using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace BudLogManage.Common.Util
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
            string[] dateParas = date.Split(new string[] {"年","月","日" }, StringSplitOptions.RemoveEmptyEntries);
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
                logger.Info(ex.Message);
            }

            return null;
            
        }
    }
}
