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
    public class BackupServerD// : IBackupServer
    {
        public Database db = new Database();
        public int InsertBackupServer(BackupServer BackupServer, SqlConnection conn)
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

        public int UpdateBackupServer(BackupServer BackupServer, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServer SET backupServerName = @servername,"
                          + " backupServerIP = @ip,"
                          + " memo = @memo, "
                          + " account =@account,"
                          + " password =@password,"
                          + " startFile =@startFile,"
                          + " updater =@updater,"
                          + " updateDate =@updateDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@servername",BackupServer.backupServerName),
                new SqlParameter("@ip",BackupServer.backupServerIP),
                new SqlParameter("@memo",BackupServer.memo),
                new SqlParameter("@account",BackupServer.account),
                new SqlParameter("@password",BackupServer.password),
                new SqlParameter("@startFile",BackupServer.startFile),
                new SqlParameter("@updater",BackupServer.updater),
                new SqlParameter("@updateDate",BackupServer.updateDate),
                new SqlParameter("@id",BackupServer.id)
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

        public int DeleteBackupServerById(int BackupServerId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServer SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",BackupServerId),
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

        public IList<BackupServer> GetBackupServer(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            IList<BackupServer> lBackupServer = new List<BackupServer>();
            string sql = "select id,backupServerName,backupServerIP,memo,account,password,startFile from backupServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt= SqlHelper.Squery(sql, conn, spvalues);
            lBackupServer = DBTool.GetListFromDatatable<BackupServer>(dt);
            return lBackupServer;
        }

        public int GetBackupServerCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(*) as count FROM backupServer";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<BackupServer> GetBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
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
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lBackupServer = DBTool.GetListFromDatatable<BackupServer>(dt);
            return lBackupServer;
        }
        /// <summary>
        /// get part backup server list
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IList<BackupServer> GetPartBackupServerList(SqlConnection conn, string groupId)
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
        public IList<BackupServer> GetGroupBackupServerList(SqlConnection conn, string groupId)
        {
            IList<BackupServer> gbsList = new List<BackupServer>();
            string sql = "SELECT bs.id,bs.backupServerName,bs.backupServerIP,bs.account,bs.password,bs.startFile  FROM backupServer bs"
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
