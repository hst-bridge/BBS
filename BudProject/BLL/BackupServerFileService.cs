using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBLL;
using Common;
using IDAL;
using System.Data.SqlClient;
using DBUtility;
using Model;
using System.Data.Odbc;

namespace BLL
{
    public class BackupServerFileService : IBackupServerFileService
    {
        private IBackupServerFile BackupServerFileDal = DALFactory.DataAccess.CreateBackupServerFile();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int GetBackupServerFileCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerFileDal.GetBackupServerFileCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerFileId"></param>
        /// <param name="loginID"></param>
        /// <returns></returns>
        public int DeleteBackupServerFile(int BackupServerFileId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerFileDal.DeleteBackupServerFileById(BackupServerFileId,loginID, conn);
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        public int UpdateBackupServerFile(BackupServerFile BackupServerFile)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerFileDal.UpdateBackupServerFile(BackupServerFile, conn);
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        public int InsertBackupServerFile(BackupServerFile BackupServerFile)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerFileDal.InsertBackupServerFile(BackupServerFile, conn);
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupServerFileId"></param>
        /// <returns></returns>
        public BackupServerFile GetBackupServerFileById(int BackupServerFileId)
        {
            OdbcConnection conn;
            IList<BackupServerFile> BackupServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = BackupServerFileId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerFile = BackupServerFileDal.GetBackupServerFile(condition, conn);
                if (BackupServerFile.Count > 0)
                {
                    return BackupServerFile[0];
                }
                conn.Close();
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public IList<BackupServerFile> GetBackupServerFilePage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<BackupServerFile> BackupServerFile;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerFile = BackupServerFileDal.GetBackupServerFilePage(condition, page, pagesize, conn);
                conn.Close();
                return BackupServerFile;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<BackupServerFile> GetBackupServerFileList()
        {
            OdbcConnection conn;
            IList<BackupServerFile> BackupServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerFile = BackupServerFileDal.GetBackupServerFile(condition, conn);
                conn.Close();
                return BackupServerFile;
            }
        }
    }
}
