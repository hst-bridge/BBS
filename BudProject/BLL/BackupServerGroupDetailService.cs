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
    public class BackupServerGroupDetailService : IBackupServerGroupDetailService
    {
        private IBackupServerGroupDetail BackupServerGroupDetailDal = DALFactory.DataAccess.CreateBackupServerGroupDetail();
        public int GetBackupServerGroupDetailCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDetailDal.GetBackupServerGroupDetailCount(condition, conn);
                conn.Close();
            }
            return count;
        }

        public int DeleteBackupServerGroupDetail(int BackupServerGroupDetailId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDetailDal.DeleteBackupServerGroupDetailById(BackupServerGroupDetailId,loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int DeleteBackupServerGroupDetail(int BackupServerId,int BackupGroupId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDetailDal.DeleteBackupServerGroupDetail(BackupServerId, BackupGroupId,loginID, conn);
                conn.Close();
            }
            return count;
        }
        public int DeleteBackupServerGroupDetailByGroupId(int BackupGroupId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDetailDal.DeleteBackupServerGroupDetailByGroupId(BackupGroupId, loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDetailDal.UpdateBackupServerGroupDetail(BackupServerGroupDetail, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDetailDal.InsertBackupServerGroupDetail(BackupServerGroupDetail, conn);
                conn.Close();
            }
            return count;
        }

        public BackupServerGroupDetail GetBackupServerGroupDetailById(int BackupServerGroupDetailId)
        {
            OdbcConnection conn;
            IList<BackupServerGroupDetail> BackupServerGroupDetail;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = BackupServerGroupDetailId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroupDetail = BackupServerGroupDetailDal.GetBackupServerGroupDetail(condition, conn);
                if (BackupServerGroupDetail.Count > 0)
                {
                    return BackupServerGroupDetail[0];
                }
                conn.Close();
                return null;
            }
        }
        public IList<BackupServerGroupDetail> GetBackupServerGroupDetailByGroupId(string backupServerGroupId) 
        {
            OdbcConnection conn;
            IList<BackupServerGroupDetail> BackupServerGroupDetail;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "backupServerGroupID=?", param = "@backupServerGroupID", value = backupServerGroupId },
            new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" }};
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroupDetail = BackupServerGroupDetailDal.GetBackupServerGroupDetail(condition, conn);
                conn.Close();
                return BackupServerGroupDetail;
            }
        }
        public IList<BackupServerGroupDetail> GetBackupServerGroupDetailPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<BackupServerGroupDetail> BackupServerGroupDetail;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroupDetail = BackupServerGroupDetailDal.GetBackupServerGroupDetailPage(condition, page, pagesize, conn);
                conn.Close();
                return BackupServerGroupDetail;
            }
        }

        public IList<BackupServerGroupDetail> GetBackupServerGroupDetailList()
        {
            OdbcConnection conn;
            IList<BackupServerGroupDetail> BackupServerGroupDetail;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroupDetail = BackupServerGroupDetailDal.GetBackupServerGroupDetail(condition, conn);
                conn.Close();
                return BackupServerGroupDetail;
            }
        }

    }
}
