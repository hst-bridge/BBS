using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Model;
using DBUtility;

namespace SQLServerDAL
{
    public class TransferLogD//:ITransferLog
    {
        public IList<TransferLog> GetTransferLogListByProc(SqlConnection conn, SqlParameter[] paras, string strProcedureName)
        {
            IList<TransferLog> lLog = new List<TransferLog>();
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddRange(paras);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = strProcedureName;
            cmd.Connection = conn;
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    lLog = DBTool.GetListFromDatatable<TransferLog>(ds.Tables[0]);
                }
            }
            catch
            {
            }
            return lLog;
        }
    }
}
