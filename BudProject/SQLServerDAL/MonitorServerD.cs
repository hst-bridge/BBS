using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using DBUtility;
using System.Data.SqlClient;
using Common;
using System.Data;


namespace SQLServerDAL
{
    public class MonitorServerD// : IMonitorServer
    {
        public Database db = new Database();
        public int InsertMonitorServer(MonitorServer MonitorServer, SqlConnection conn)
        {
            try
            {
                return db.insert(MonitorServer, "monitorServer", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateMonitorServer(MonitorServer MonitorServer, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServer SET monitorServerName = @servername,"
                          + " monitorServerIP = @ip,"
                          + " memo = @memo, "
                          + " account =@account,"
                          + " password =@password,"
                          + " startFile =@startFile,"
                          + " monitorDrive = @monitorDrive,"
                          + " updater =@updater,"
                          + " updateDate =@updateDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@servername",MonitorServer.monitorServerName),
                new SqlParameter("@ip",MonitorServer.monitorServerIP),
                new SqlParameter("@memo",MonitorServer.memo),
                new SqlParameter("@account",MonitorServer.account),
                new SqlParameter("@password",MonitorServer.password),
                new SqlParameter("@startFile",MonitorServer.startFile),
                new SqlParameter("@monitorDrive",MonitorServer.monitorDrive),
                new SqlParameter("@updater",MonitorServer.updater),
                new SqlParameter("@updateDate",MonitorServer.updateDate),
                new SqlParameter("@id",MonitorServer.id)
            };
            try
            {
                result = db.Udaquery(sql, conn, para);
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public int DeleteMonitorServerById(int MonitorServerId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServer SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",MonitorServerId),
                new SqlParameter("@deleter",loginID),
                new SqlParameter("@deleteDate",DateTime.Now)
            };
            try
            {
                result = db.Udaquery(sql, conn, para);
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public IList<MonitorServer> GetMonitorServer(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            IList<MonitorServer> lMonitorServer = new List<MonitorServer>();
            string sql = @"SELECT id,monitorServerName,monitorServerIP,memo,account,password,startFile,monitorDrive  FROM monitorServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lMonitorServer = DBTool.GetListFromDatatable<MonitorServer>(dt);
            return lMonitorServer;
        }

        public int GetMonitorServerCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM monitorServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<MonitorServer> GetMonitorServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
        {
            IList<MonitorServer> lMonitorServer = new List<MonitorServer>();
            string sql = @"SELECT id,monitorServerName,monitorServerIP,memo,account,password,startFile,monitorDrive 
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM monitorServer ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lMonitorServer = DBTool.GetListFromDatatable<MonitorServer>(dt);
            return lMonitorServer;
        }
    }
}
