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
    public class BackupServerGroupD : IBackupServerGroup
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertBackupServerGroup(BackupServerGroup BackupServerGroup, OdbcConnection conn)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateBackupServerGroup(BackupServerGroup BackupServerGroup, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroup SET backupServerGroupName = ?,"
                          + " monitorServerID = ?,"
                          + " memo = ?,"
                          + " updater = ?,"
                          + " updateDate = ?,"
                          + " synchronismFlg = 0"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@backupServerGroupName",BackupServerGroup.backupServerGroupName),
                new OdbcParameter("@monitorServerID",BackupServerGroup.monitorServerID),
                new OdbcParameter("@memo",BackupServerGroup.memo),
                new OdbcParameter("@updater",BackupServerGroup.updater),
                new OdbcParameter("@updateDate",BackupServerGroup.updateDate),
                new OdbcParameter("@id",BackupServerGroup.id)
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
        /// <param name="BackupServerGroupId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteBackupServerGroupById(int BackupServerGroupId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerGroup SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?,"
                          + " synchronismFlg = 0"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",BackupServerGroupId)
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
        public IList<BackupServerGroup> GetBackupServerGroup(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<BackupServerGroup> lBackupServerGroup = new List<BackupServerGroup>();
            string sql = "select id,backupServerGroupName,monitorServerID,memo from backupServerGroup";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServerGroup = DBTool.GetListFromDatatable<BackupServerGroup>(dt);
            return lBackupServerGroup;
        }

        /// <summary>
        /// Get BackupServerGroup from All DB
        /// </summary>
        /// 2014-07-02 wjd add
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<BackupServerGroup> GetBackupServerGroupAll(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<BackupServerGroup> lBackupServerGroup = new List<BackupServerGroup>();
            string sql = "select id,DBServerIP,backupServerGroupName,monitorServerID,memo from backupServerGroup";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServerGroup = DBTool.GetListFromDatatable<BackupServerGroup>(dt);
            return lBackupServerGroup;
        }

        /// <summary>
        /// Get BackupServerGroup By BackupServerID
        /// </summary>
        /// 2014-06-23 wjd add
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<BackupServerGroup> GetBackupServerGroupByBackupServerID(int backupServerID, OdbcConnection conn)
        {
            IList<BackupServerGroup> lBackupServerGroup = new List<BackupServerGroup>();
            string sql = @"select g.id,g.backupServerGroupName,g.monitorServerID,g.memo from [backupServerGroup] g
  left join [backupServerGroupDetail] gd on gd.[backupServerGroupID] = g.[id]
  where gd.backupServerID = " + backupServerID + " and gd.[deleteFlg] = 0 and g.[deleteFlg] = 0;";

            DataTable dt = OdbcHelper.Squery(sql, conn);
            lBackupServerGroup = DBTool.GetListFromDatatable<BackupServerGroup>(dt);
            return lBackupServerGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetBackupServerGroupCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM backupServerGroup";
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
        public IList<BackupServerGroup> GetBackupServerGroupPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
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
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServerGroup = DBTool.GetListFromDatatable<BackupServerGroup>(dt);
            return lBackupServerGroup;
        }
    }
}
