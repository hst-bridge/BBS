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
    public class MonitorServerFolderService : IMonitorServerFolderService
    {
        private IMonitorServerFolder MonitorServerFolderDal = DALFactory.DataAccess.CreateMonitorServerFolder();
        public int GetMonitorServerFolderCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFolderDal.GetMonitorServerFolderCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        public int DeleteMonitorServerFolder(int MonitorServerFolderId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFolderDal.DeleteMonitorServerFolderById(MonitorServerFolderId, loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateMonitorServerFolder(MonitorServerFolder MonitorServerFolder)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFolderDal.UpdateMonitorServerFolder(MonitorServerFolder, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertMonitorServerFolder(MonitorServerFolder MonitorServerFolder)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFolderDal.InsertMonitorServerFolder(MonitorServerFolder, conn);
                conn.Close();
            }
            return count;
        }

        public MonitorServerFolder GetMonitorServerFolderById(int MonitorServerFolderId)
        {
            OdbcConnection conn;
            IList<MonitorServerFolder> MonitorServerFolder;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = MonitorServerFolderId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFolder = MonitorServerFolderDal.GetMonitorServerFolder(condition, conn);
                if (MonitorServerFolder.Count > 0)
                {
                    return MonitorServerFolder[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<MonitorServerFolder> GetMonitorServerFolderPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<MonitorServerFolder> MonitorServerFolder;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFolder = MonitorServerFolderDal.GetMonitorServerFolderPage(condition, page, pagesize, conn);
                conn.Close();
                return MonitorServerFolder;
            }
        }

        public IList<MonitorServerFolder> GetMonitorServerFolderList()
        {
            OdbcConnection conn;
            IList<MonitorServerFolder> MonitorServerFolder;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFolder = MonitorServerFolderDal.GetMonitorServerFolder(condition, conn);
                conn.Close();
                return MonitorServerFolder;
            }
        }
        public int DeleteMonitorServerFolderByServerId(int MonitorServerFolderId)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFolderDal.DeleteMonitorServerFolderByServerId(MonitorServerFolderId, conn);
                conn.Close();
            }
            return count;
        }
        public IList<MonitorServerFolder> GetMonitorServerFolderByMonitorServerID(string MonitorServerId)
        {
            OdbcConnection conn;
            IList<MonitorServerFolder> MonitorServerFolder;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "monitorServerID= ? ", param = "@monitorServerID", value = MonitorServerId } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFolder = MonitorServerFolderDal.GetMonitorServerFolder(condition, conn);
                conn.Close();
                return MonitorServerFolder;
            }
        }
        public IList<MonitorServerFolder> GetMonitorFolderByServerIDAndInitFlg(string monitorServerID) 
        {
            OdbcConnection conn;
            IList<MonitorServerFolder> MonitorServerFolder;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "monitorServerID= ? ", param = "@monitorServerID", value = monitorServerID }
            ,new SearchCondition { con = "initFlg= ? ", param = "@initFlg", value = "1" }};
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFolder = MonitorServerFolderDal.GetMonitorServerFolder(condition, conn);
                conn.Close();
                return MonitorServerFolder;
            }
        }
    }
}
