using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Common.Helper;

namespace BudLogManage.Common.Culture
{
    /// <summary>
    /// 用于获取适当编码
    /// </summary>
   public class CultureManager
    {
       public static Encoding GetEncoding()
       {
           //932 为日文Shift-JIS 编码codepage 编号
           return Encoding.GetEncoding(932);
       }

       /// <summary>
       /// 根据字符串名字 获取对应的日志类型
       /// </summary>
       /// <param name="chars"></param>
       /// <returns></returns>
       public static LogType GetLogType(string chars)
       {
           string str = chars.Trim();
           switch (str)
           {
               case "新しい": return LogType.NEW;
               case "古い": return LogType.OLD;
               case "*EXTRA File": return LogType.EXTRAFILE;
               case "*EXTRA Dir": return LogType.EXTRADIR;
               default: return LogType.ERROR;
           }
           
       }
    }
}
