using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDAL;
using Model;
using DBUtility;
using System.Data.SqlClient;
using Common;
using System.Data;
using System.Data.Odbc;


namespace MySQLDAL
{
    public class MonitorServerD : IMonitorServer
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertMonitorServer(MonitorServer MonitorServer, OdbcConnection conn)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateMonitorServer(MonitorServer MonitorServer, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServer SET monitorServerName = ?,"
                          + " monitorServerIP = ?,"
                          + " monitorSystem = ?,"
                          + " memo = ?, "
                          + " account = ?,"
                          + " password = ?,"
                          + " startFile = ?,"
                          + " monitorDrive = ?,"
                          + " monitorDriveP = ?,"
                          + " monitorMacPath = ?,"
                          + " monitorLocalPath = ?,"
                          + " copyInit = ?,"
                          + " updater = ?,"
                          + " updateDate = ?,"
                          + " synchronismFlg = 0"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@monitorServerName",MonitorServer.monitorServerName),
                new OdbcParameter("@monitorServerIP",MonitorServer.monitorServerIP),
                new OdbcParameter("@monitorSystem",MonitorServer.monitorSystem),
                new OdbcParameter("@memo",MonitorServer.memo),
                new OdbcParameter("@account",MonitorServer.account),
                new OdbcParameter("@password",MonitorServer.password),
                new OdbcParameter("@startFile",MonitorServer.startFile),
                new OdbcParameter("@monitorDrive",MonitorServer.monitorDrive),
                new OdbcParameter("@monitorDriveP",MonitorServer.monitorDriveP),
                new OdbcParameter("@monitorMacPath",MonitorServer.monitorMacPath),
                new OdbcParameter("@monitorLocalPath",MonitorServer.monitorLocalPath),
                new OdbcParameter("@copyInit",MonitorServer.copyInit),
                new OdbcParameter("@updater",MonitorServer.updater),
                new OdbcParameter("@updateDate",MonitorServer.updateDate),
                new OdbcParameter("@id",MonitorServer.id)
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
        /// <summary>
        /// 
        /// </summary>
        /// set synchronismFlg = 0  2014-07-02 wjd modified
        /// <param name="MonitorServerId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteMonitorServerById(int MonitorServerId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServer SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?,"
                          + "synchronismFlg = 0"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",MonitorServerId)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateMonitorServerCopyInit(int MonitorServerId,  OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServer SET copyInit = 1 "                          
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@id",MonitorServerId)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<MonitorServer> GetMonitorServer(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<MonitorServer> lMonitorServer = new List<MonitorServer>();
            string sql = @"SELECT id,monitorServerName,monitorServerIP,monitorSystem,memo,account,password,startFile,monitorDrive,monitorDriveP,monitorMacPath,monitorLocalPath,copyInit  FROM monitorServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorServer = DBTool.GetListFromDatatable<MonitorServer>(dt);
            return lMonitorServer;
        }

        /// <summary>
        /// 根据条件获取バックアップ元 from all DB
        /// </summary>
        /// 2014-07-06 wjd add
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<MonitorServer> GetMonitorServerAll(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<MonitorServer> lMonitorServer = new List<MonitorServer>();
            string sql = @"SELECT distinct id,DBServerIP,monitorServerName,monitorServerIP,monitorSystem,memo,account,password,monitorMacPath,monitorLocalPath,copyInit  FROM monitorServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorServer = DBTool.GetListFromDatatable<MonitorServer>(dt);
            return lMonitorServer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetMonitorServerCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM monitorServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            int count = (int)OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<MonitorServer> GetMonitorServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<MonitorServer> lMonitorServer = new List<MonitorServer>();
            string sql = @"SELECT id,monitorServerName,monitorServerIP,monitorSystem,memo,account,password,startFile,monitorDrive,monitorDriveP,monitorLocalPath 
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM monitorServer ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorServer = DBTool.GetListFromDatatable<MonitorServer>(dt);
            return lMonitorServer;
        }
        public int GetMaxId(OdbcConnection conn) 
        {
            string sql = @"SELECT max(id) as maxId FROM monitorServer";
            int count = (int)OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql);
            return count;
        }
    }
}
