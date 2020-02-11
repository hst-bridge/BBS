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
    public class MonitorServerService : IMonitorServerService
    {
        private IMonitorServer MonitorServerDal = DALFactory.DataAccess.CreateMonitorServer();
        public int GetMonitorServerCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerDal.GetMonitorServerCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        public int DeleteMonitorServer(int MonitorServerId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerDal.DeleteMonitorServerById(MonitorServerId,loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateMonitorServerCopyInit(int MonitorServerId)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerDal.UpdateMonitorServerCopyInit(MonitorServerId, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateMonitorServer(MonitorServer MonitorServer)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerDal.UpdateMonitorServer(MonitorServer, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertMonitorServer(MonitorServer MonitorServer)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerDal.InsertMonitorServer(MonitorServer, conn);
                conn.Close();
            }
            return count;
        }

        public MonitorServer GetMonitorServerById(int MonitorServerId)
        {
            OdbcConnection conn;
            IList<MonitorServer> MonitorServer;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = MonitorServerId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServer = MonitorServerDal.GetMonitorServer(condition, conn);
                if (MonitorServer.Count > 0)
                {
                    return MonitorServer[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<MonitorServer> GetMonitorServerPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<MonitorServer> MonitorServer;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServer = MonitorServerDal.GetMonitorServerPage(condition, page, pagesize, conn);
                conn.Close();
                return MonitorServer;
            }
        }

        public IList<MonitorServer> GetMonitorServerList()
        {
            OdbcConnection conn;
            IList<MonitorServer> MonitorServer;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServer = MonitorServerDal.GetMonitorServer(condition, conn);
                conn.Close();
                return MonitorServer;
            }
        }

        /// <summary>
        /// Get MonitorServerList from All DB
        /// </summary>
        /// 2014-06-04 wjd add
        /// <returns></returns>
        public IList<MonitorServer> GetMonitorServerListAll()
        {
            OdbcConnection conn;
            IList<MonitorServer> MonitorServer;

            IList<SearchCondition> condition = new List<SearchCondition>();
            condition.Add(new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" });
            condition = CommonUtil.GetLoginIPCondition(condition);

            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                MonitorServer = MonitorServerDal.GetMonitorServerAll(condition, conn);
                conn.Close();
                return MonitorServer;
            }
        }

        public IList<MonitorServer> GetMonitorServerListByName(string msName)
        {
            OdbcConnection conn;
            IList<MonitorServer> MonitorServer;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" },new SearchCondition { con = "monitorServerName=?", param = "@monitorServerName", value = msName } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServer = MonitorServerDal.GetMonitorServer(condition, conn);
                conn.Close();
                return MonitorServer;
            }
        }
        public int GetMaxId() 
        {
            OdbcConnection conn;
            int maxId;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                maxId = MonitorServerDal.GetMaxId(conn);
                conn.Close();
                return maxId;
            }
        }
    }
}
