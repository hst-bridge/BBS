using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudFileTransfer
{
    public class TransferResult
    {
        /// <summary>
        /// 結果: OK TURE, NG FALSE
        /// </summary>
        public bool OKResult { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 完了時間
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 時間期間
        /// </summary>
        public TimeSpan TimeSpan { get; set; }
    }
}
