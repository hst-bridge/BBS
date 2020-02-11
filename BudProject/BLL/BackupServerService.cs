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
    public class BackupServerService : IBackupServerService
    {
        private IBackupServer BackupServerDal = DALFactory.DataAccess.CreateBackupServer();
        public int GetBackupServerCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerDal.GetBackupServerCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        public int DeleteBackupServer(int BackupServerId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerDal.DeleteBackupServerById(BackupServerId,loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateBackupServer(BackupServer BackupServer)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerDal.UpdateBackupServer(BackupServer, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertBackupServer(BackupServer BackupServer)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = BackupServerDal.InsertBackupServer(BackupServer, conn);
                conn.Close();
            }
            return count;
        }

        public BackupServer GetBackupServerById(int BackupServerId)
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = BackupServerId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetBackupServer(condition, conn);
                if (BackupServer.Count > 0)
                {
                    return BackupServer[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<BackupServer> GetBackupServerPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetBackupServerPage(condition, page, pagesize, conn);
                conn.Close();
                return BackupServer;
            }
        }

        public IList<BackupServer> GetBackupServerList()
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetBackupServer(condition, conn);
                conn.Close();
                return BackupServer;
            }
        }

        public IList<BackupServer> GetPartBackupServerList(string groupId)
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetPartBackupServerList(conn, groupId);
                conn.Close();
                return BackupServer;
            }
        }

        public IList<BackupServer> GetGroupBackupServerList(string groupId)
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetGroupBackupServerList(conn, groupId);
                conn.Close();
                return BackupServer;
            }
        }

        public IList<BackupServer> GetBackupServerListByName(string bkName)
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" }, new SearchCondition { con = "backupServerName=?", param = "@backupServerName", value = bkName } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetBackupServer(condition, conn);
                conn.Close();
                return BackupServer;
            }
        }

        /// <summary>
        /// Get BackupServer List By Name But This Id
        /// </summary>
        /// 2014-06-24 wjd add
        /// <param name="id"></param>
        /// <param name="bkName"></param>
        /// <returns></returns>
        public IList<BackupServer> GetBackupServerListByNameButId(int id, string bkName)
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            SearchCondition[] condition = new SearchCondition[]
            {
                new SearchCondition { con = "id!=?", param = "@id", value = id.ToString() },
                new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" },
                new SearchCondition { con = "backupServerName=?", param = "@backupServerName", value = bkName }
            };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetBackupServer(condition, conn);
                conn.Close();
                return BackupServer;
            }
        }

        /// <summary>
        /// Get BackupServer List By IP And StartFolder But This Id
        /// </summary>
        /// 2014-06-24 wjd add
        /// <param name="id"></param>
        /// <param name="bkIP"></param>
        /// <param name="startFolder"></param>
        /// <returns></returns>
        public IList<BackupServer> GetBackupServerListButId(int id, string bkIP, string startFolder)
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            SearchCondition[] condition = new SearchCondition[]
            {
                new SearchCondition { con = "id!=?", param = "@id", value = id.ToString() },
                new SearchCondition { con = "backupServerIP=?", param = "@backupServerIP", value = bkIP },
                new SearchCondition { con = "startFile=?", param = "@startFile", value = startFolder },
                new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" }
            };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetBackupServer(condition, conn);
                conn.Close();
                return BackupServer;
            }
        }

        /// <summary>
        /// Get Deleted BackupServer List By IP And StartFolder
        /// </summary>
        /// 2014-06-30 wjd add
        /// <param name="bkIP"></param>
        /// <param name="startFolder"></param>
        /// <returns></returns>
        public IList<BackupServer> GetDeletedBackupServerList(string bkIP, string startFolder)
        {
            OdbcConnection conn;
            IList<BackupServer> BackupServer;
            SearchCondition[] condition = new SearchCondition[]
            {
                new SearchCondition { con = "backupServerIP=?", param = "@backupServerIP", value = bkIP },
                new SearchCondition { con = "startFile=?", param = "@startFile", value = startFolder },
                new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "1" }
            };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                BackupServer = BackupServerDal.GetBackupServer(condition, conn);
                conn.Close();
                return BackupServer;
            }
        }
    }
}
