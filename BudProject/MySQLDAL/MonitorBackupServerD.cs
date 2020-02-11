using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using Model;
using System.Data.SqlClient;
using Common;
using System.Data;
using IDAL;
using System.Data.Odbc;

namespace MySQLDAL
{
    public class MonitorBackupServerD : IMonitorBackupServer
    {
        public OdbcDatabase db = new OdbcDatabase();
        public int InsertMonitorBackupServer(MonitorBackupServer MonitorBackupServer, OdbcConnection conn)
        {
            try
            {
                return db.insert(MonitorBackupServer, "monitorBackupServer", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateMonitorBackupServer(MonitorBackupServer MonitorBackupServer, OdbcConnection conn)
        {
            int result = -1;
            string sql = @"UPDATE monitorBackupServer
                               SET backupServerGroupID = ?
                                  ,updater = ?
                                  ,updateDate = ? 
                             WHERE monitorServerID=@monitorServerID";
            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@backupServerGroupID",MonitorBackupServer.backupServerGroupId),
                new OdbcParameter("@updater",MonitorBackupServer.updater),
                new OdbcParameter("@updateDate",MonitorBackupServer.updateDate),
                new OdbcParameter("@monitorServerID",MonitorBackupServer.monitorServerId)
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
        /// <param name="MonitorBackupServerId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteMonitorBackupServerById(int MonitorBackupServerId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorBackupServer SET deleteFlg = 1,"
                          + " deleter = ?,"
                          + " deleteDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",MonitorBackupServerId)
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
        public IList<MonitorBackupServer> GetMonitorBackupServer(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<MonitorBackupServer> lMonitorBackupServer = new List<MonitorBackupServer>();
            string sql = @"SELECT mbs.id
                              ,mbs.backupServerGroupID
                              ,bsg.backupServerGroupName
                              ,mbs.monitorServerID
                              ,mbs.creater
                              ,mbs.createDate 
                          FROM monitorBackupServer as mbs INNER JOIN backupServerGroup as bsg ON mbs.backupServerGroupID = bsg.id";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorBackupServer = DBTool.GetListFromDatatable<MonitorBackupServer>(dt);
            return lMonitorBackupServer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetMonitorBackupServerCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM MonitorBackupServer";
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
        public IList<MonitorBackupServer> GetMonitorBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<MonitorBackupServer> lMonitorBackupServer = new List<MonitorBackupServer>();
            string sql = @"SELECT id
                              ,backupServerGroupID
                              ,backupServerID
                              ,creater
                              ,createDate 
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM monitorBackupServer ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorBackupServer = DBTool.GetListFromDatatable<MonitorBackupServer>(dt);
            return lMonitorBackupServer;
        }
    }
}
