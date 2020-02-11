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
    public class BackupServerGroupDetailD// : IBackupServerGroupDetail
    {
        public Database db = new Database();
        public int InsertBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail, SqlConnection conn)
        {
            try
            {
                return db.insert(BackupServerGroupDetail, "backupServerGroupDetail", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail, SqlConnection conn)
        {
            int result = -1;
            string sql = @"UPDATE backupServerGroupDetail
                               SET backupServerGroupID = @servergroupid
                                  ,backupServerID = @serverid                                  
                                  ,updater = @updater
                                  ,updateDate = @updatedate
                             WHERE id=@id";
            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@servergroupid",BackupServerGroupDetail.backupServerGroupId),
                new SqlParameter("@serverid",BackupServerGroupDetail.backupServerId),
                new SqlParameter("@updater",BackupServerGroupDetail.updater),
                new SqlParameter("@updateDate",BackupServerGroupDetail.updateDate),
                new SqlParameter("@id",BackupServerGroupDetail.id)
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

        public int DeleteBackupServerGroupDetailById(int BackupServerGroupDetailId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroupDetail SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",BackupServerGroupDetailId),
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

        public int DeleteBackupServerGroupDetail(int BackupServerId, int BackupGroupId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroupDetail SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE backupServerGroupID = @backupServerGroupID and backupServerID = @backupServerID";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@backupServerGroupID",BackupGroupId),
                new SqlParameter("@BackupServerId",BackupServerId),
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

        public IList<BackupServerGroupDetail> GetBackupServerGroupDetail(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            IList<BackupServerGroupDetail> lBackupServerGroupDetail = new List<BackupServerGroupDetail>();
            string sql = @"SELECT id
                              ,backupServerGroupID
                              ,backupServerID
                              ,creater
                              ,createDate 
                          FROM [Category]";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lBackupServerGroupDetail = DBTool.GetListFromDatatable<BackupServerGroupDetail>(dt);
            return lBackupServerGroupDetail;
        }

        public int GetBackupServerGroupDetailCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(*) as count FROM backupServerGroupDetail";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<BackupServerGroupDetail> GetBackupServerGroupDetailPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
        {
            IList<BackupServerGroupDetail> lBackupServerGroupDetail = new List<BackupServerGroupDetail>();
            string sql = @"SELECT [id]
                              ,backupServerGroupID
                              ,backupServerID
                              ,creater
                              ,createDate 
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM [Category] ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lBackupServerGroupDetail = DBTool.GetListFromDatatable<BackupServerGroupDetail>(dt);
            return lBackupServerGroupDetail;
        }
    }
}
