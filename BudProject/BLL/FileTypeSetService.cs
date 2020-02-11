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
    public class FileTypeSetService : IFileTypeSetService
    {
        private IFileTypeSet FileTypeSetDal = DALFactory.DataAccess.CreateFileTypeSet();
        public int GetFileTypeSetCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = FileTypeSetDal.GetFileTypeSetCount(condition, conn);
                conn.Close();
            }
            return count;
        }

        public int DeleteFileTypeSet(int FileTypeSetId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = FileTypeSetDal.DeleteFileTypeSetById(FileTypeSetId, loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateFileTypeSet(FileTypeSet FileTypeSet)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = FileTypeSetDal.UpdateFileTypeSet(FileTypeSet, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertFileTypeSet(FileTypeSet FileTypeSet)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = FileTypeSetDal.InsertFileTypeSet(FileTypeSet, conn);
                conn.Close();
            }
            return count;
        }

        public FileTypeSet GetFileTypeSetId(int FileTypeSetId)
        {
            OdbcConnection conn;
            IList<FileTypeSet> FileTypeSet;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = FileTypeSetId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                FileTypeSet = FileTypeSetDal.GetFileTypeSet(condition, conn);
                if (FileTypeSet.Count > 0)
                {
                    return FileTypeSet[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<FileTypeSet> GetFileTypeSetPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<FileTypeSet> FileTypeSet;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                FileTypeSet = FileTypeSetDal.GetFileTypeSetPage(condition, page, pagesize, conn);
                conn.Close();
                return FileTypeSet;
            }
        }
        public IList<FileTypeSet> GetFileTypeSetList()
        {
            OdbcConnection conn;
            IList<FileTypeSet> FileTypeSet;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                FileTypeSet = FileTypeSetDal.GetFileTypeSet(condition, conn);
                conn.Close();
                return FileTypeSet;
            }
        }
        public IList<FileTypeSet> GetFileTypeSetByMonitorServerID(string MonitorServerId)
        {
            OdbcConnection conn;
            IList<FileTypeSet> FileTypeSet;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "monitorServerID= ? ", param = "@monitorServerID", value = MonitorServerId } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                FileTypeSet = FileTypeSetDal.GetFileTypeSet(condition, conn);
                conn.Close();
                return FileTypeSet;
            }
        }
        public FileTypeSet GetFileTypeSetByMonitorServerIdAndFolderName(string MonitorServerId, string folderName)
        {
            OdbcConnection conn;
            IList<FileTypeSet> FileTypeSet;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "monitorServerID= ? ", param = "@monitorServerID", value = MonitorServerId }, new SearchCondition { con = "monitorServerFolderName= ? ", param = "@monitorServerFolderName", value = folderName } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                FileTypeSet = FileTypeSetDal.GetFileTypeSet(condition, conn);
                conn.Close();
                if (FileTypeSet.Count > 0)
                {
                    return FileTypeSet[0];
                }
                return new FileTypeSet();
            }
        }
    }
}
