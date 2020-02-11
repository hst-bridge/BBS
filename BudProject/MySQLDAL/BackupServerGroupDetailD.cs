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
    public class BackupServerGroupDetailD : IBackupServerGroupDetail
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail, OdbcConnection conn)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail, OdbcConnection conn)
        {
            int result = -1;
            string sql = @"UPDATE backupServerGroupDetail
                               SET backupServerGroupID = ?
                                  ,backupServerID = ?                                  
                                  ,updater = ?
                                  ,updateDate = ?
                             WHERE id=?";
            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@backupServerGroupID",BackupServerGroupDetail.backupServerGroupId),
                new OdbcParameter("@backupServerID",BackupServerGroupDetail.backupServerId),
                new OdbcParameter("@updater",BackupServerGroupDetail.updater),
                new OdbcParameter("@updateDate",BackupServerGroupDetail.updateDate),
                new OdbcParameter("@id",BackupServerGroupDetail.id)
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
        /// <param name="BackupServerGroupDetailId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteBackupServerGroupDetailById(int BackupServerGroupDetailId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroupDetail SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",BackupServerGroupDetailId)
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
        /// <param name="BackupGroupId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteBackupServerGroupDetail(int BackupServerId, int BackupGroupId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroupDetail SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?,"
                          + " synchronismFlg = 0"
                          + " WHERE backupServerGroupID = ? and backupServerID = ?";

            OdbcParameter[] para = new OdbcParameter[]{                                
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@backupServerGroupID",BackupGroupId),
                new OdbcParameter("@BackupServerId",BackupServerId)
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
        /// <param name="BackupGroupId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteBackupServerGroupDetailByGroupId(int BackupGroupId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroupDetail SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?"
                          + " WHERE backupServerGroupID = ?";

            OdbcParameter[] para = new OdbcParameter[]{                                
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@backupServerGroupID",BackupGroupId)
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
        public IList<BackupServerGroupDetail> GetBackupServerGroupDetail(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<BackupServerGroupDetail> lBackupServerGroupDetail = new List<BackupServerGroupDetail>();
            string sql = @"SELECT id
                              ,backupServerGroupID
                              ,backupServerID
                              ,creater
                              ,createDate 
                          FROM backupServerGroupDetail";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServerGroupDetail = DBTool.GetListFromDatatable<BackupServerGroupDetail>(dt);
            return lBackupServerGroupDetail;
        }

        public int GetBackupServerGroupDetailCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM backupServerGroupDetail";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            int count = (int)OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<BackupServerGroupDetail> GetBackupServerGroupDetailPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<BackupServerGroupDetail> lBackupServerGroupDetail = new List<BackupServerGroupDetail>();
            string sql = @"SELECT [id]
                              ,backupServerGroupID
                              ,backupServerID
                              ,creater
                              ,createDate 
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM backupServerGroupDetail ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServerGroupDetail = DBTool.GetListFromDatatable<BackupServerGroupDetail>(dt);
            return lBackupServerGroupDetail;
        }
    }
}
