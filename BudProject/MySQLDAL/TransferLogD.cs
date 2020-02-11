using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IDAL;
using Model;
using DBUtility;
using System.Data.Odbc;
using System.Reflection;

namespace MySQLDAL
{
    public class TransferLogD:ITransferLog
    {
        /// <summary>
        /// log4net
        /// </summary>
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IList<TransferLog> GetTransferLogListByProc(OdbcConnection conn, OdbcParameter[] paras, string strProcedureName)
        {
            IList<TransferLog> lLog = new List<TransferLog>();
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand();
            cmd.Parameters.AddRange(paras);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = strProcedureName;
            cmd.Connection = conn;
            cmd.CommandTimeout = 150;
            try
            {
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
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

        /// <summary>
        /// 获取传送容量 xiecongwen 20140710
        /// </summary>
        /// <param name="DBServerIP">like '192.168.253.131,192.168.254.33'</param>
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogList(string DBServerIP,int groupID, string dtStart, string dtEnd, string tmStart, string tmEnd, string name)
        {
            IList<TransferLog> lLog = new List<TransferLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select monitorFileName,monitorFileSize,convert(varchar(10),backupStartTime,111) as DT,DATEPART(HH,backupStartTime) as HH from [log] where backupStartTime between '@StartDatetime' and '@EndDatetime' and DBServerIP in (@DBServerIP) ");
            if (groupID > 0) { sb.Append(" and backupServerGroupID=@groupID "); }
            if (!string.IsNullOrWhiteSpace(name))
            {
                sb.Append("  and backupServerFileName like '%@backupServerFileName%' ");
            }
            sb.Replace("@StartDatetime", dtStart + " " + tmStart).Replace("@EndDatetime", dtEnd + " " + tmEnd).Replace("@DBServerIP", DBServerIP).Replace("@groupID", groupID.ToString()).Replace("@backupServerFileName", name);
            string sql = @"select DT as transferDate, SUM(cast(monitorFileSize as bigint)) as transferFileSize,COUNT(monitorFileSize) as transferFileCount,HH as transferTime from (" + sb.ToString() + ") as a group by DT,HH order by DT,HH ";

            try
            {
                using (SqlConnection conn = SqlHelper.CreateConntion())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 150;

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        lLog = DBTool.GetListFromDatatable<TransferLog>(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            
            return lLog;
        }

        /// <summary>
        /// 获取传送容量
        /// </summary>
        /// <param name="DBServerIP">like '192.168.253.131,192.168.254.33'</param>
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="name">file or folder name for searching</param>
        /// <param name="stateFlg">1:OK, 0:NG or 2:both</param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogList(string DBServerIP, int groupID, string dtStart, string dtEnd, string tmStart, string tmEnd, string name,
            int stateFlg)
        {
            IList<TransferLog> lLog = new List<TransferLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select monitorFileName,monitorFileSize,convert(varchar(10),backupStartTime,111) as DT,DATEPART(HH,backupStartTime) as HH from [log] where backupStartTime between '@StartDatetime' and '@EndDatetime' and DBServerIP in (@DBServerIP) ");
            if (groupID > 0) { sb.Append(" and backupServerGroupID=@groupID "); }
            if (!string.IsNullOrWhiteSpace(name))
            {
                sb.Append("  and backupServerFileName like '%@backupServerFileName%' ");
            }
            if (stateFlg != 2)
            {
                sb.Append(" and backupFlg = ").Append(stateFlg);
            }
            sb.Replace("@StartDatetime", dtStart + " " + tmStart).Replace("@EndDatetime", dtEnd + " " + tmEnd).Replace("@DBServerIP", DBServerIP).Replace("@groupID", groupID.ToString()).Replace("@backupServerFileName", name);
            string sql = @"select DT as transferDate, SUM(cast(monitorFileSize as bigint)) as transferFileSize,COUNT(monitorFileSize) as transferFileCount,HH as transferTime from (" + sb.ToString() + ") as a group by DT,HH order by DT,HH ";

            try
            {
                using (SqlConnection conn = SqlHelper.CreateConntion())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 150;

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        lLog = DBTool.GetListFromDatatable<TransferLog>(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return lLog;
        }

        /// <summary>
        /// 获取传送容量 xiecongwen 20140710
        /// </summary>
        /// For Winform——2014-8-25 wjd add
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogList(int groupID, string dtStart, string dtEnd, string tmStart, string tmEnd, string name)
        {
            IList<TransferLog> lLog = new List<TransferLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select monitorFileName,monitorFileSize,convert(varchar(10),backupStartTime,111) as DT,DATEPART(HH,backupStartTime) as HH from [log] where backupStartTime between '@StartDatetime' and '@EndDatetime' ");
            if (groupID > 0) { sb.Append(" and backupServerGroupID=@groupID "); }
            if (!string.IsNullOrWhiteSpace(name))
            {
                sb.Append("  and backupServerFileName like '%@backupServerFileName%' ");
            }
            sb.Replace("@StartDatetime", dtStart + " " + tmStart).Replace("@EndDatetime", dtEnd + " " + tmEnd).Replace("@groupID", groupID.ToString()).Replace("@backupServerFileName", name);
            string sql = @"select DT as transferDate, SUM(cast(monitorFileSize as bigint)) as transferFileSize,COUNT(monitorFileSize) as transferFileCount,HH as transferTime from (" + sb.ToString() + ") as a group by DT,HH order by DT,HH ";

            try
            {
                using (OdbcConnection conn = OdbcHelper.CreateConntion())
                {
                    conn.Open();
                    DataTable dt = OdbcHelper.Squery(sql, conn);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        lLog = DBTool.GetListFromDatatable<TransferLog>(dt);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return lLog;
        }

        /// <summary>
        /// 获取传送容量 For Winform——2014-12-02 wjd add
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="name"></param>
        /// <param name="backupFlg">1:OK, 0:NG or 99:both</param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogList(int groupID, string dtStart, string dtEnd, string tmStart, string tmEnd, string name,
            string backupFlg)
        {
            IList<TransferLog> lLog = new List<TransferLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select monitorFileName,monitorFileSize,convert(varchar(10),backupStartTime,111) as DT,DATEPART(HH,backupStartTime) as HH from [log] where backupStartTime between '@StartDatetime' and '@EndDatetime' ");
            if (groupID > 0) { sb.Append(" and backupServerGroupID=@groupID "); }
            if (!string.IsNullOrWhiteSpace(name))
            {
                sb.Append("  and backupServerFileName like '%@backupServerFileName%' ");
            }
            if (backupFlg != "99")
            {
                sb.Append(" and backupFlg = ").Append(backupFlg);
            }
            sb.Replace("@StartDatetime", dtStart + " " + tmStart).Replace("@EndDatetime", dtEnd + " " + tmEnd).Replace("@groupID", groupID.ToString()).Replace("@backupServerFileName", name);
            string sql = @"select DT as transferDate, SUM(cast(monitorFileSize as bigint)) as transferFileSize,COUNT(monitorFileSize) as transferFileCount,HH as transferTime from (" + sb.ToString() + ") as a group by DT,HH order by DT,HH ";

            try
            {
                using (OdbcConnection conn = OdbcHelper.CreateConntion())
                {
                    conn.Open();
                    DataTable dt = OdbcHelper.Squery(sql, conn);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        lLog = DBTool.GetListFromDatatable<TransferLog>(dt);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return lLog;
        }
    }
}
