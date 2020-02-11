using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudFileCheckListen.Common.Constant
{
    /// <summary>
    /// 用于存储常量
    /// </summary>
    static class DefaultValue
    {
        /// <summary>
        /// 文字列のディフォルト値
        /// </summary>
       public const string DEFAULTCHAR_VALUE = "";
       /// <summary>
       /// 数字フィールドのディフォルト値
       /// </summary>
       public const int DEFAULTINT_VALUE = 0;

       /// <summary>
       /// 日付時間フィールドのディフォルト値
       /// </summary>
       public static DateTime DEFAULTDATETIME_VALUE = new DateTime(1900, 1, 1, 0, 0, 0);

    }
}
