using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudSSH.Common.Helper.Culture
{
    /// <summary>
    /// 用于提供匹配关键字
    /// </summary>
    public class KeyUtil
    {
        /// <summary>
        /// 根据不同语言 返回不同返回值（当前暂时直接返回日文，待完善）
        /// </summary>
        public static string ErrorKey { get { 
            return  "エラー";
        } }

        public static string TopKey = "開始:";
        /// <summary>
        /// 此种错误忽略
        /// </summary>
        public static string RetryErrorKey = "再試行";

        //用于判断日志记录格式是否混杂
        public static string CauseKey = "ファイル名";
    }
}
