using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using Model;
using System.Data.SqlClient;
using Common;
using System.Data;

namespace SQLServerDAL
{
    public class MonitorBackupServerD// : IMonitorBackupServer
    {
        public Database db = new Database();
        public int InsertMonitorBackupServer(MonitorBackupServer MonitorBackupServer, SqlConnection conn)
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
        public int UpdateMonitorBackupServer(MonitorBackupServer MonitorBackupServer, SqlConnection conn)
        {
            int result = -1;
            string sql = @"UPDATE monitorBackupServer
                               SET backupServerGroupID = @backupServerGroupID
                                  ,updater = @updater
                                  ,updateDate = @updatedate
                             WHERE monitorServerID=@monitorServerID";
            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@backupServerGroupID",MonitorBackupServer.backupServerGroupId),
                new SqlParameter("@updater",MonitorBackupServer.updater),
                new SqlParameter("@updateDate",MonitorBackupServer.updateDate),
                new SqlParameter("@monitorServerID",MonitorBackupServer.monitorServerId)
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
        public int DeleteMonitorBackupServerById(int MonitorBackupServerId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorBackupServer SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",MonitorBackupServerId),
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<MonitorBackupServer> GetMonitorBackupServer(IEnumerable<SearchCondition> conditon, SqlConnection conn)
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
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lMonitorBackupServer = DBTool.GetListFromDatatable<MonitorBackupServer>(dt);
            return lMonitorBackupServer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetMonitorBackupServerCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM MonitorBackupServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
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
        public IList<MonitorBackupServer> GetMonitorBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
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
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lMonitorBackupServer = DBTool.GetListFromDatatable<MonitorBackupServer>(dt);
            return lMonitorBackupServer;
        }
    }
}
