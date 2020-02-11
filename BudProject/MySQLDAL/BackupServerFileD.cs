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
    public class BackupServerFileD:IBackupServerFile
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertBackupServerFile(BackupServerFile BackupServerFile, OdbcConnection conn)
        {
            try
            {
                return db.insert(BackupServerFile, "backupServerFile", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateBackupServerFile(BackupServerFile BackupServerFile, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerFile SET backupServerGroupID = ?,"
                          + " backupServerID = ?,"
                          + " backupServerFileName = ?, "
                          + " backupServerFilePath = ?,"
                          + " backupServerFileType = ?,"
                          + " backupServerFileSize = ?,"
                          + " backupStartTime = ?,"
                          + " backupEndTime = ?,"
                          + " backupTime = ?,"
                          + " backupFlg = ?,"
                          + " copyStartTime = ?,"
                          + " copyEndTime = ?,"
                          + " copyTime = ?,"
                          + " copyFlg = ?,"
                          + " updater = ?,"
                          + " updateDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@backupServerGroupID",BackupServerFile.backupServerGroupID),
                new OdbcParameter("@backupServerID",BackupServerFile.backupServerID),
                new OdbcParameter("@backupServerFileName",BackupServerFile.backupServerFileName),
                new OdbcParameter("@backupServerFilePath",BackupServerFile.backupServerFilePath),
                new OdbcParameter("@backupServerFileType",BackupServerFile.backupServerFileType),
                new OdbcParameter("@backupServerFileSize",BackupServerFile.backupServerFileSize),
                new OdbcParameter("@backupStartTime",BackupServerFile.backupStartTime),
                new OdbcParameter("@backupEndTime",BackupServerFile.backupEndTime),
                new OdbcParameter("@backupTime",BackupServerFile.backupTime),
                new OdbcParameter("@backupFlg",BackupServerFile.backupFlg),
                new OdbcParameter("@copyStartTime",BackupServerFile.copyStartTime),
                new OdbcParameter("@copyEndTime",BackupServerFile.copyEndTime),
                new OdbcParameter("@copyTime",BackupServerFile.copyTime),
                new OdbcParameter("@copyFlg",BackupServerFile.copyFlg),
                new OdbcParameter("@updater",BackupServerFile.updater),
                new OdbcParameter("@updateDate",BackupServerFile.updateDate),
                new OdbcParameter("@id",BackupServerFile.id)
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
        /// <param name="BackupServerFileId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteBackupServerFileById(int BackupServerFileId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerFile SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",BackupServerFileId)
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

        public IList<BackupServerFile> GetBackupServerFile(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<BackupServerFile> lBackupServerFile = new List<BackupServerFile>();
            string sql = @"SELECT id
                              ,backupServerGroupID
                              ,backupServerID
                              ,backupServerFileName
                              ,backupServerFilePath
                              ,backupServerFileType
                              ,backupServerFileSize
                              ,backupStartTime
                              ,backupEndTime
                              ,backupTime
                              ,backupFlg
                              ,copyStartTime
                              ,copyEndTime
                              ,copyTime
                              ,copyFlg
                          FROM backupServerFile";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServerFile = DBTool.GetListFromDatatable<BackupServerFile>(dt);
            return lBackupServerFile;
        }

        public int GetBackupServerFileCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM backupServerFile";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            int count = (int)OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<BackupServerFile> GetBackupServerFilePage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<BackupServerFile> lBackupServerFile = new List<BackupServerFile>();
            string sql = @"SELECT id
                              ,backupServerGroupID
                              ,backupServerID
                              ,backupServerFileName
                              ,backupServerFilePath
                              ,backupServerFileType
                              ,backupServerFileSize
                              ,backupStartTime
                              ,backupEndTime
                              ,backupTime
                              ,backupFlg
                              ,copyStartTime
                              ,copyEndTime
                              ,copyTime
                              ,copyFlg
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM backupServerFile ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lBackupServerFile = DBTool.GetListFromDatatable<BackupServerFile>(dt);
            return lBackupServerFile;
        }
    }
}
