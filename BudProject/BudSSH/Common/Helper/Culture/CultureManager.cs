using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Common.Helper;

namespace BudSSH.Common.Helper.Culture
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

    }
}
