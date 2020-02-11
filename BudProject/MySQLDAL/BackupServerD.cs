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
    public class BackupServerD : IBackupServer
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertBackupServer(BackupServer BackupServer, OdbcConnection conn)
        {
            try
            {
                return db.insert(BackupServer, "backupServer", conn);
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
        public int UpdateBackupServer(BackupServer BackupServer, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServer SET backupServerName = ?,"
                          + " backupServerIP = ?,"
                          + " memo = ?, "
                          + " account =?,"
                          + " password =?,"
                          + " startFile =?,"
                          + " ssbpath =?,"
                          + " updater =?,"
                          + " updateDate =?,"
                          + " synchronismFlg = 0"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@backupServerName",BackupServer.backupServerName),
                new OdbcParameter("@backupServerIP",BackupServer.backupServerIP),
                new OdbcParameter("@memo",BackupServer.memo),
                new OdbcParameter("@account",BackupServer.account),
                new OdbcParameter("@password",BackupServer.password),
                new OdbcParameter("@startFile",BackupServer.startFile),
                new OdbcParameter("@ssbpath",BackupServer.ssbpath),
                new OdbcParameter("@updater",BackupServer.updater),
                new OdbcParameter("@updateDate",BackupServer.updateDate),
                new OdbcParameter("@id",BackupServer.id)
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
        public int DeleteBackupServerById(int BackupServerId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServer SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?,"
                          + " synchronismFlg = 0"
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
        public IList<BackupServer> GetBackupServer(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<BackupServer> lBackupServer = new List<BackupServer>();
            string sql = "select id,backupServerName,backupServerIP,memo,account,password,startFile from backupServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt= OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServer = DBTool.GetListFromDatatable<BackupServer>(dt);
            return lBackupServer;
        }

        public int GetBackupServerCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM backupServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            int count = (int)OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<BackupServer> GetBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<BackupServer> lBackupServer = new List<BackupServer>();
            string sql = @"SELECT id,backupServerName,backupServerIP,memo,account,password,startFile
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM backupServer ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServer = DBTool.GetListFromDatatable<BackupServer>(dt);
            return lBackupServer;
        }
        /// <summary>
        /// get part backup server list
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IList<BackupServer> GetPartBackupServerList(OdbcConnection conn, string groupId)
        {
            IList<BackupServer> bspList = new List<BackupServer>();
            try
            {
                string sql = "select id,backupServerName,backupServerIP,memo,account,password,startFile from backupServer where deleteFlg=0"
                            + " and id not in ("
                            + "select backupServerID from backupServerGroupDetail where backupServerGroupID = " + groupId + " and deleteFlg=0"
                            + ")";
                DataTable dt = db.Squery(sql, conn);
                foreach (DataRow row in dt.Rows)
                {
                    BackupServer bs = new BackupServer();
                    bs.id = row["id"].ToString();
                    bs.backupServerName = row["backupServerName"].ToString();
                    bs.backupServerIP = row["backupServerIP"].ToString();
                    bs.memo = row["memo"].ToString();
                    bs.account = row["account"].ToString();
                    bs.password = row["password"].ToString();
                    bs.startFile = row["startFile"].ToString();
                    bspList.Add(bs);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return bspList;
        }
        /// <summary>
        /// get the server group detail list 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupCD"></param>
        /// <returns></returns>
        public IList<BackupServer> GetGroupBackupServerList(OdbcConnection conn, string groupId)
        {
            IList<BackupServer> gbsList = new List<BackupServer>();
            string sql = "SELECT bs.id,bs.backupServerName,bs.backupServerIP,bs.account,bs.password,bs.startFile,bs.ssbpath FROM backupServer bs"
                        + " INNER JOIN backupServerGroupDetail bsgd ON bs.id = bsgd.backupServerID"
                        + " WHERE bs.deleteFlg = 0 AND bsgd.deleteFlg=0 AND bsgd.backupServerGroupID = " + groupId;
            try
            {
                DataTable dt = db.Squery(sql, conn);
                foreach (DataRow row in dt.Rows)
                {
                    BackupServer bs = new BackupServer();
                    bs.id = row["id"].ToString();
                    bs.backupServerName = row["backupServerName"].ToString();
                    bs.backupServerIP = row["backupServerIP"].ToString();
                    bs.account = row["account"].ToString();
                    bs.password = row["password"].ToString();
                    bs.startFile = row["startFile"].ToString();
                    bs.ssbpath = row["ssbpath"].ToString();
                    gbsList.Add(bs);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return gbsList;
        }
    }
}
