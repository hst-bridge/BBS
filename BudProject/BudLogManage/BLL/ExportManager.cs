using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Model;
using System.Data;
using BudLogManage.Common.Export;
using System.Reflection;
using log4net;

namespace BudLogManage.BLL
{
    /// <summary>
    /// 专门用于导出
    /// </summary>
    public class ExportManager
    {

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 导出统计结果
        /// </summary>
        /// <param name="total"></param>
        /// <param name="fpath"></param>
        public void Export(Total total, string fpath)
        {
            try
            {
                //将total 转换成datatable
                DataTable dt = new DataTable();
                dt.Columns.Add("type", typeof(string));
                dt.Columns.Add("blank1", typeof(string));

                dt.Columns.Add("copy", typeof(string));
                dt.Columns.Add("blank2", typeof(string));

                dt.Columns.Add("delete", typeof(string));
                dt.Columns.Add("blank3", typeof(string));

                dt.Columns.Add("transfer", typeof(string));
                dt.Columns.Add("blank4", typeof(string));

                dt.Columns.Add("transferDelete", typeof(string));

                string blank = "     ";
                //时间
                if (total.TimeInterval != null)
                {
                    dt.Rows.Add(new object[] { blank, total.TimeInterval.Start.ToLongDateString(), total.TimeInterval.Start.ToLongTimeString(), "    -", total.TimeInterval.End.ToLongDateString(), total.TimeInterval.End.ToLongTimeString() ,blank,blank});
                }

                dt.Rows.Add(new object[] { blank, "コピー済み", blank, "削除", blank, "転送", blank, "削除" });
                dt.Rows.Add(new object[] { "ファイル", total.FilesCopied, blank, total.FilesDeleted, blank, total.FileTransfered,blank,total.FileTransDeleted });
                dt.Rows.Add(new object[] { "バイト", total.BytesCopied, blank, total.BytesDeleted, blank, total.BytesTransfered ,blank,total.BytesTransDeleted});

                CSVExporter csvExporter = new CSVExporter();
                csvExporter.Export(dt, fpath);
            }
            catch (System.Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }
    }
}
