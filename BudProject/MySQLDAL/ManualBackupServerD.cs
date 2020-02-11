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
    public class ManualBackupServerD : IManualBackupServer
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertManualBackupServer(ManualBackupServer ManualBackupServer, OdbcConnection conn)
        {
            try
            {
                return db.insert(ManualBackupServer, "manualBackupServer", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateManualBackupServer(ManualBackupServer ManualBackupServer, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE manualBackupServer SET "
                          + " DBServerIP = ?,"
                          + " serverIP = ?,"
                          + " account =?,"
                          + " password =?,"
                          + " startFile =?,"
                          + " drive =?,"
                          + " updater =?,"
                          + " updateDate =?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@DBServerIP",ManualBackupServer.DBServerIP),
                new OdbcParameter("@backupServerIP",ManualBackupServer.serverIP),
                new OdbcParameter("@account",ManualBackupServer.account),
                new OdbcParameter("@password",ManualBackupServer.password),
                new OdbcParameter("@startFile",ManualBackupServer.startFile),
                new OdbcParameter("@drive",ManualBackupServer.drive),
                new OdbcParameter("@updater",ManualBackupServer.updater),
                new OdbcParameter("@updateDate",ManualBackupServer.updateDate),
                new OdbcParameter("@id",ManualBackupServer.id)
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
        /// <param name="BackupServerId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteManualBackupServerById(int BackupServerId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE manualBackupServer SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",BackupServerId)
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
        public IList<ManualBackupServer> GetManualBackupServer(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<ManualBackupServer> lManualBackupServer = new List<ManualBackupServer>();
            string sql = "select id,DBServerIP,serverIP,account,password,startFile,drive from manualBackupServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt= OdbcHelper.Squery(sql, conn, spvalues);
            lManualBackupServer = DBTool.GetListFromDatatable<ManualBackupServer>(dt);
            return lManualBackupServer;
        }

        public int GetManualBackupServerCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM manualBackupServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            int count = (int)OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<ManualBackupServer> GetManualBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<ManualBackupServer> lManualBackupServer = new List<ManualBackupServer>();
            string sql = @"SELECT id,serverIP,account,password,startFile,drive
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM manualBackupServer ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lManualBackupServer = DBTool.GetListFromDatatable<ManualBackupServer>(dt);
            return lManualBackupServer;
        }
    }
}
