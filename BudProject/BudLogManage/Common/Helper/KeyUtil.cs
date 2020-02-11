using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudLogManage.Common.Helper
{
    /// <summary>
    /// 用于提供匹配关键字
    /// </summary>
    public class KeyUtil
    {
        public static string TopKey = "開始:";
        public static string TailKey = "合計     コピー済み";
        public static string TailTimeKey = "終了";
        public static string ErrorKey = "エラー";

        /// <summary>
        /// 此种错误忽略
        /// </summary>
        public static string RetryErrorKey = "再試行";
    }
}
