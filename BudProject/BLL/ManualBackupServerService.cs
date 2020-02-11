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
    public class ManualBackupServerService : IManualBackupServerService
    {
        private IManualBackupServer ManualBackupServerDal = DALFactory.DataAccess.CreateManualBackupServer();
        public int GetManualBackupServerCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = ManualBackupServerDal.GetManualBackupServerCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        public int DeleteManualBackupServer(int BackupServerId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                count = ManualBackupServerDal.DeleteManualBackupServerById(BackupServerId, loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateManualBackupServer(ManualBackupServer ManualBackupServer)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                count = ManualBackupServerDal.UpdateManualBackupServer(ManualBackupServer, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertManualBackupServer(ManualBackupServer ManualBackupServer)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                count = ManualBackupServerDal.InsertManualBackupServer(ManualBackupServer, conn);
                conn.Close();
            }
            return count;
        }

        public ManualBackupServer GetManualBackupServerById(int BackupServerId)
        {
            OdbcConnection conn;
            IList<ManualBackupServer> ManualBackupServer;
            //add condition DBServerIP——2014-06-04 wjd modified
            IList<SearchCondition> condition = new List<SearchCondition>();
            condition.Add(new SearchCondition { con = "id=?", param = "@id", value = BackupServerId.ToString() });
            //condition = CommonUtil.GetLoginIPCondition(condition);

            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                ManualBackupServer = ManualBackupServerDal.GetManualBackupServer(condition, conn);
                if (ManualBackupServer.Count > 0)
                {
                    return ManualBackupServer[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<ManualBackupServer> GetManualBackupServerPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<ManualBackupServer> ManualBackupServer;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                ManualBackupServer = ManualBackupServerDal.GetManualBackupServerPage(condition, page, pagesize, conn);
                conn.Close();
                return ManualBackupServer;
            }
        }

        public IList<ManualBackupServer> GetManualBackupServerList()
        {
            OdbcConnection conn;
            IList<ManualBackupServer> ManualBackupServer;
            //add condition DBServerIP——2014-06-04 wjd modified
            IList<SearchCondition> condition = new List<SearchCondition>();
            condition.Add(new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" });
            //condition = CommonUtil.GetLoginIPCondition(condition);

            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                ManualBackupServer = ManualBackupServerDal.GetManualBackupServer(condition, conn);
                conn.Close();
                return ManualBackupServer;
            }
        }
    }
}
