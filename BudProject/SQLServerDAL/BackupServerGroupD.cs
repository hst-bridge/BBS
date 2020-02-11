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
    public class BackupServerGroupD// : IBackupServerGroup
    {
        public Database db = new Database();
        public int InsertBackupServerGroup(BackupServerGroup BackupServerGroup, SqlConnection conn)
        {
            try
            {
                return db.insert(BackupServerGroup, "backupServerGroup", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateBackupServerGroup(BackupServerGroup BackupServerGroup, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroup SET backupServerGroupName = @servername,"
                          + " memo = @memo,"
                          + " updater = @updater,"
                          + " updateDate = @updateDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@servername",BackupServerGroup.backupServerGroupName),
                new SqlParameter("@memo",BackupServerGroup.memo),
                new SqlParameter("@updater",BackupServerGroup.updater),
                new SqlParameter("@updateDate",BackupServerGroup.updateDate),
                new SqlParameter("@id",BackupServerGroup.id)
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

        public int DeleteBackupServerGroupById(int BackupServerGroupId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroup SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",BackupServerGroupId),
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

        public IList<BackupServerGroup> GetBackupServerGroup(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            IList<BackupServerGroup> lBackupServerGroup = new List<BackupServerGroup>();
            string sql = "select id,backupServerGroupName,memo from backupServerGroup";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lBackupServerGroup = DBTool.GetListFromDatatable<BackupServerGroup>(dt);
            return lBackupServerGroup;
        }

        public int GetBackupServerGroupCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(*) as count FROM backupServerGroup";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<BackupServerGroup> GetBackupServerGroupPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
        {
            IList<BackupServerGroup> lBackupServerGroup = new List<BackupServerGroup>();
            string sql = @"SELECT id,backupServerGroupName,memo
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM backupServerGroup ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lBackupServerGroup = DBTool.GetListFromDatatable<BackupServerGroup>(dt);
            return lBackupServerGroup;
        }
    }
}
