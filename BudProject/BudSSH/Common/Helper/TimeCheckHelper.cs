using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace BudSSH.Common.Helper
{
    /// <summary>
    ///用于判断当前时间是否吻合配置时间
    /// </summary>
    class TimeCheckHelper
    {
        /// <summary>
        /// 用于存储同步flag：标识当天是否已经同步过
        /// </summary>
        private static Hashtable SyncToday = new Hashtable();

        /// <summary>
        /// 判断是否到达配置时间且每天只能有一次为TRUE
        /// 
        /// 需求：定期运行功能； 
        /// 现实：由于采用定时器来判断时间是否到达，定时器时间间隔及阻塞时间等原因
        ///       不能保证正好等于配置时间，故需要有一个容忍间隔，此容忍间隔又必须大于定时器时间间隔
        /// 实现程度： 不及格，待完善
        /// 20141211 xiecongwen
        /// </summary>
        /// <param name="configTime"></param>
        /// <returns></returns>
        public static bool CheckTime(string key, string configTime)
        {
            #region 判断时间
            string TimeRegex = @"^\d{1,2}:\d{1,2}$";
            if (Regex.IsMatch(configTime, TimeRegex))
            {
                //判断当日是否已经同步过
                object value = SyncToday[key];
                if (value != null)
                {
                    DateTime todaySyncTime = (DateTime)value;
                    if (todaySyncTime != null && (DateTime.Now.ToShortDateString().Equals(todaySyncTime.ToShortDateString())))
                    {
                        return false;
                    }
                }

                //校验时间是否在配置时间容忍范围内
                string[] times = configTime.Split(':');
                DateTime now = DateTime.Now;
                DateTime setting = new DateTime(now.Year, now.Month, now.Day, Convert.ToInt32(times[0]), Convert.ToInt32(times[1]), 0, 0);
                double diff = (now - setting).TotalMinutes;
                if (diff > 0 && diff < 60)
                {
                    SyncToday[key] = DateTime.Now;
                    return true;
                }
            }
            #endregion

            return false;
        }
    }
}
