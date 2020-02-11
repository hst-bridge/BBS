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
    public class BackupServerGroupService : IBackupServerGroupService
    {
        private IBackupServerGroup BackupServerGroupDal = DALFactory.DataAccess.CreateBackupServerGroup();
        public int GetBackupServerGroupCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDal.GetBackupServerGroupCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        public int DeleteBackupServerGroup(int BackupServerGroupId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDal.DeleteBackupServerGroupById(BackupServerGroupId, loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateBackupServerGroup(BackupServerGroup BackupServerGroup)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDal.UpdateBackupServerGroup(BackupServerGroup, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertBackupServerGroup(BackupServerGroup BackupServerGroup)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerGroupDal.InsertBackupServerGroup(BackupServerGroup, conn);
                conn.Close();
            }
            return count;
        }

        public BackupServerGroup GetBackupServerGroupById(int BackupServerGroupId)
        {
            OdbcConnection conn;
            IList<BackupServerGroup> BackupServerGroup;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = BackupServerGroupId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroup = BackupServerGroupDal.GetBackupServerGroup(condition, conn);
                if (BackupServerGroup.Count > 0)
                {
                    return BackupServerGroup[0];
                }
                conn.Close();
                return null;
            }
        }

        /// <summary>
        /// Get BackupServerGroup By BackupServerID
        /// </summary>
        /// 2014-06-23 wjd add
        /// <param name="backupServerID"></param>
        /// <returns></returns>
        public BackupServerGroup GetBackupServerGroupByBackupServerID(int backupServerID)
        {
            OdbcConnection conn;
            IList<BackupServerGroup> BackupServerGroup;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroup = BackupServerGroupDal.GetBackupServerGroupByBackupServerID(backupServerID, conn);
                if (BackupServerGroup.Count > 0)
                {
                    return BackupServerGroup[0];
                }
                conn.Close();
                return new BackupServerGroup();
            }
        }

        public BackupServerGroup GetBackupServerGroupByMonitorId(int MonitorId)
        {
            OdbcConnection conn;
            IList<BackupServerGroup> BackupServerGroup;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "monitorServerId=?", param = "@monitorServerId", value = MonitorId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroup = BackupServerGroupDal.GetBackupServerGroup(condition, conn);
                if (BackupServerGroup.Count > 0)
                {
                    return BackupServerGroup[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<BackupServerGroup> GetBackupServerGroupPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<BackupServerGroup> BackupServerGroup;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroup = BackupServerGroupDal.GetBackupServerGroupPage(condition, page, pagesize, conn);
                conn.Close();
                return BackupServerGroup;
            }
        }

        public IList<BackupServerGroup> GetBackupServerGroupList()
        {
            OdbcConnection conn;
            IList<BackupServerGroup> BackupServerGroup;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroup = BackupServerGroupDal.GetBackupServerGroup(condition, conn);
                conn.Close();
                return BackupServerGroup;
            }
        }

        /// <summary>
        /// Get lists from BackupServerGroup of the All DB
        /// </summary>
        /// 2014-06-04 wjd add
        /// <returns></returns>
        public IList<BackupServerGroup> GetBackupServerGroupListAll()
        {
            OdbcConnection conn;
            IList<BackupServerGroup> BackupServerGroup;
            
            IList<SearchCondition> condition = new List<SearchCondition>();
            condition.Add(new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" });
            condition = CommonUtil.GetLoginIPCondition(condition);

            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                BackupServerGroup = BackupServerGroupDal.GetBackupServerGroupAll(condition, conn);
                conn.Close();
                return BackupServerGroup;
            }
        }

        public IList<BackupServerGroup> GetBackupServerGroupByName(string groupName)
        {
            OdbcConnection conn;
            IList<BackupServerGroup> BackupServerGroup;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" }, new SearchCondition { con = "backupServerGroupName=?", param = "@backupServerGroupName", value = groupName } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServerGroup = BackupServerGroupDal.GetBackupServerGroup(condition, conn);
                conn.Close();
                return BackupServerGroup;
            }
        }
    }
}
