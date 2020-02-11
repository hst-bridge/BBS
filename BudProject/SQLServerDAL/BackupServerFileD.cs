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
    public class BackupServerFileD//:IBackupServerFile
    {
        public Database db = new Database();
        public int InsertBackupServerFile(BackupServerFile BackupServerFile, SqlConnection conn)
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

        public int UpdateBackupServerFile(BackupServerFile BackupServerFile, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerFile SET backupServerGroupID = @servergroupid,"
                          + " backupServerID = @serverid,"
                          + " backupServerFileName = @serverfilename, "
                          + " backupServerFilePath =@serverfilepath,"
                          + " backupServerFileType =@serverfiletype,"
                          + " backupServerFileSize =@serverfilesize,"
                          + " backupStartTime =@backupstarttime,"
                          + " backupEndTime =@backupendtime"
                          + " backupTime = @backuptime"
                          + " backupFlg = @backupflg"
                          + " copyStartTime = @copystarttime"
                          + " copyEndTime = @copyendtime"
                          + " copyTime = @coptime"
                          + " copyFlg = @copyflg"
                          + " updater =@updater,"
                          + " updateDate =@updateDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@servergroupid",BackupServerFile.backupServerGroupID),
                new SqlParameter("@serverid",BackupServerFile.backupServerID),
                new SqlParameter("@serverfilename",BackupServerFile.backupServerFileName),
                new SqlParameter("@serverfilepath",BackupServerFile.backupServerFilePath),
                new SqlParameter("@serverfiletype",BackupServerFile.backupServerFileType),
                new SqlParameter("@serverfilesize",BackupServerFile.backupServerFileSize),
                new SqlParameter("@backupstarttime",BackupServerFile.backupStartTime),
                new SqlParameter("@backupendtime",BackupServerFile.backupEndTime),
                new SqlParameter("@backuptime",BackupServerFile.backupTime),
                new SqlParameter("@backupflg",BackupServerFile.backupFlg),
                new SqlParameter("@copystarttime",BackupServerFile.copyStartTime),
                new SqlParameter("@copyendtime",BackupServerFile.copyEndTime),
                new SqlParameter("@coptime",BackupServerFile.copyTime),
                new SqlParameter("@copyflg",BackupServerFile.copyFlg),
                new SqlParameter("@updater",BackupServerFile.updater),
                new SqlParameter("@updateDate",BackupServerFile.updateDate),
                new SqlParameter("@id",BackupServerFile.id)
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

        public int DeleteBackupServerFileById(int BackupServerFileId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE backupServerFile SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{
                new SqlParameter("@id",BackupServerFileId),
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

        public IList<BackupServerFile> GetBackupServerFile(IEnumerable<SearchCondition> conditon, SqlConnection conn)
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
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lBackupServerFile = DBTool.GetListFromDatatable<BackupServerFile>(dt);
            return lBackupServerFile;
        }

        public int GetBackupServerFileCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(*) as count FROM backupServerFile";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<BackupServerFile> GetBackupServerFilePage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
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
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lBackupServerFile = DBTool.GetListFromDatatable<BackupServerFile>(dt);
            return lBackupServerFile;
        }
    }
}
