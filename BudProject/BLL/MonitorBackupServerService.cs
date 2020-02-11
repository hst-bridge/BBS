using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBLL;
using Common;
using System.Data.SqlClient;
using IDAL;
using DBUtility;
using Model;
using System.Data.Odbc;

namespace BLL
{
    public class MonitorBackupServerService : IMonitorBackupServerService
    {
        private IMonitorBackupServer MonitorBackupServerDal = DALFactory.DataAccess.CreateMonitorBackupServer();
        public int GetMonitorBackupServerCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorBackupServerDal.GetMonitorBackupServerCount(condition, conn);
                conn.Close();
            }
            return count;
        }

        public int DeleteMonitorBackupServer(int MonitorBackupServerId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorBackupServerDal.DeleteMonitorBackupServerById(MonitorBackupServerId, loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateMonitorBackupServer(MonitorBackupServer MonitorBackupServer)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorBackupServerDal.UpdateMonitorBackupServer(MonitorBackupServer, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertMonitorBackupServer(MonitorBackupServer MonitorBackupServer)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorBackupServerDal.InsertMonitorBackupServer(MonitorBackupServer, conn);
                conn.Close();
            }
            return count;
        }

        public MonitorBackupServer GetMonitorBackupServerById(int MonitorServerId)
        {
            OdbcConnection conn;
            IList<MonitorBackupServer> MonitorBackupServer;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "monitorServerID=?", param = "@monitorServerID", value = MonitorServerId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorBackupServer = MonitorBackupServerDal.GetMonitorBackupServer(condition, conn);
                if (MonitorBackupServer.Count > 0)
                {
                    return MonitorBackupServer[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<MonitorBackupServer> GetMonitorBackupServerPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<MonitorBackupServer> MonitorBackupServer;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorBackupServer = MonitorBackupServerDal.GetMonitorBackupServerPage(condition, page, pagesize, conn);
                conn.Close();
                return MonitorBackupServer;
            }
        }

        public IList<MonitorBackupServer> GetMonitorBackupServerList()
        {
            OdbcConnection conn;
            IList<MonitorBackupServer> MonitorBackupServer;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorBackupServer = MonitorBackupServerDal.GetMonitorBackupServer(condition, conn);
                conn.Close();
                return MonitorBackupServer;
            }
        }
    }
}
