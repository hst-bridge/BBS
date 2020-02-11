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
    public class MonitorFileListenService : IMonitorFileListenService
    {
        private IMonitorFileListen MonitorFileListenDal = DALFactory.DataAccess.CreateMonitorFileListen();

        public int UpdateMonitorServer(MonitorServer MonitorServer, MonitorServer MonitorServerForOld)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorFileListenDal.UpdateMonitorFileListen(MonitorServer, MonitorServerForOld, conn);
                conn.Close();
            }
            return count;
        }

        public int GetListenCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorFileListenDal.GetMonitorFileListenCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        public MonitorFileListen GetMonitorFileListenById(int LogId)
        {
            OdbcConnection conn;
            IList<MonitorFileListen> MonitorFileListen;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = LogId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorFileListen = MonitorFileListenDal.GetMonitorFileListen(condition, conn);
                if (MonitorFileListen.Count > 0)
                {
                    return MonitorFileListen[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<MonitorFileListen> GetMonitorFileListenPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<MonitorFileListen> MonitorFileListen;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorFileListen = MonitorFileListenDal.GetMonitorFileListenPage(condition, page, pagesize, conn);
                conn.Close();
                return MonitorFileListen;
            }
        }

        public IList<MonitorFileListen> GetMonitorFileListenList()
        {
            OdbcConnection conn;
            IList<MonitorFileListen> MonitorFileListen;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorFileListen = MonitorFileListenDal.GetMonitorFileListen(condition, conn);
                conn.Close();
                return MonitorFileListen;
            }
        }
        public IList<MonitorFileListen> GetMonitorFileListenList(string fileName, string updateTime)
        {
            OdbcConnection conn;
            IList<MonitorFileListen> MonitorFileListen;
            //SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" },
            //new SearchCondition { con = "monitorFileName like '%?%'", param = "@monitorFileName", value = fileName },
            //new SearchCondition { con = "Convert(varchar,updateDate,120) like '%?%'", param = "@updateDate", value = updateTime }};

            string where = "deleteFlg = 0";

            if (fileName != "")
            {
                where += " AND monitorfileName like '" + fileName + "%'";
            }
            if (updateTime != "")
            {
                where += " AND Convert(varchar,updateDate,120) like '" + updateTime + "%'";
            }            

            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                //MonitorFileListen = MonitorFileListenDal.GetPartMonitorFileListenList(condition, conn);
                MonitorFileListen = MonitorFileListenDal.GetPartMonitorFileListenList(where, conn);
                conn.Close();
                return MonitorFileListen;
            }
        }

        public IList<MonitorFileListen> GetMonitorFileListenList(string fileName)
        {
            OdbcConnection conn;
            IList<MonitorFileListen> MonitorFileListen;

            string where = "deleteFlg = 0";

            if (fileName != "")
            {
                where += " AND monitorfileName like '%" + fileName + "%'";
            }

            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorFileListen = MonitorFileListenDal.GetPartMonitorFileListenList(where, conn);
                conn.Close();
                return MonitorFileListen;
            }
        }
        public IList<MonitorFileListen> GetMonitorFileListenList(string fileName, int monitorServerId)
        {
            OdbcConnection conn;
            IList<MonitorFileListen> MonitorFileListen;
            string where = "deleteFlg = 0";

            if (fileName != "")
            {
                where += " AND monitorfileName like '%" + fileName + "%'";
            }
            if (monitorServerId != -1)
            {
                where += " AND monitorServerID = " + monitorServerId;
            }


            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorFileListen = MonitorFileListenDal.GetPartMonitorFileListenList(where, conn);
                conn.Close();
                return MonitorFileListen;
            }
        }

        public IList<MonitorFileListen> GetMonitorFileListenListByPage(string fileName, string monitorServerId, string dirname, int pindex, int psize)
        {
            OdbcConnection conn;
            IList<MonitorFileListen> MonitorFileListen;
            //simplify sql——2014-07-03 wjd modified
            string where = "";//deleteFlg = 0  AND monitorType<>'削除'

            //monitorServerId format: DBServerIP|monitorServerId Id——2014-07-06 wjd modified
            string DBServerIP = "";
            string msId = "";
            if (monitorServerId.Trim('|').IndexOf('|') > 0)
            {
                string[] array = monitorServerId.Trim('|').Split('|');
                DBServerIP = "'" + array[0] + "'";
                msId = array[1];
            }
            else
            {
                DBServerIP = Common.CommonUtil.GetLoginIPWithQuote();
                msId = monitorServerId;
            }

            string sql = string.Empty;
            //file or folder
            if (fileName != "")
            {
                if (dirname == "0")
                {
                    sql = @"select *,ROW_NUMBER() over(order by createDate) as row from MonitorFileListen ";
                    sql += " where monitorFileName like '%" + fileName + "%'";// AND monitorFileRelativeDirectory not like '%\\" + fileName + "%'
                }
                else
                {
                   sql = @"select *,ROW_NUMBER() over(order by createDate) as row
                       from (select *,ROW_NUMBER() over (PARTITION by monitorFileRelativeDirectory order by monitorFileRelativeDirectory) rowno FROM MonitorFileListen ";

                   
                    sql += "where monitorFileRelativeDirectory like '%\\" + fileName + "%'";// id IN(select min(id) from (SELECT min(id) as id,min(CHARINDEX('\\" + fileName + "', monitorFileRelativeDirectory)) AS patindexs FROM monitorFileListen where deleteFlg=0 and monitorType<>'削除' group by monitorFileRelativeDirectory) as a group by a.patindexs) AND


                }
                sql += " AND ";
            }

            //one or all monitorServers
            if (msId != "-1")
            {
                sql += " monitorServerID = " + msId + " AND  ";
            }

            //add condition DBServerIP——2014-06-04 wjd modified
            sql += " DBServerIP in (" + DBServerIP + ") ";

            if (dirname == "1")
            {
                sql += " ) as b where rowno=1 ";
            }
            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                MonitorFileListen = MonitorFileListenDal.GetMonitorFileListenPage(sql, pindex, psize, conn);
                conn.Close();
                return MonitorFileListen;
            }
        }

        /// <summary>
        /// 获取匹配项 monitFileListen xiecongwen 20140711
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="monitorServerId">此处monitorServerId format: DBServerIP|monitorServerId 用于描述monitorServerId的具体信息</param>
        /// <param name="pindex"></param>
        /// <param name="psize"></param>
        /// <returns></returns>
        public IList<MonitorFileListen> GetMonitorFileListenListByPage(string keyName, string monitorServerId,int pindex, int psize)
        {
            IList<MonitorFileListen> MonitorFileListen = null;
            #region DBServerIP和monitorServerId 获取
            //monitorServerId format: DBServerIP|monitorServerId Id——2014-07-06 wjd modified
            string DBServerIP = "";
            if (monitorServerId.Trim('|').IndexOf('|') > 0)
            {
                string[] array = monitorServerId.Trim('|').Split('|');
                DBServerIP = "'" + array[0] + "'";
                monitorServerId = array[1];
            }
            else
            {
                DBServerIP = Common.CommonUtil.GetLoginIPWithQuote();
            }
            #endregion
            MonitorFileListen = MonitorFileListenDal.GetMonitorFileListenByPage(DBServerIP, monitorServerId, keyName, pindex, psize);

            return MonitorFileListen;
        }

        public int GetMonitorFileListenListCount(string monitorFileName, string monitorServerID, string dirname)
        {
            OdbcConnection conn;
            int count = 0;
            //simplify sql——2014-07-03 wjd modified
            string where = "";//deleteFlg = 0 AND monitorType<>'削除'

            //monitorServerId format: DBServerIP|monitorServerId Id——2014-07-06 wjd modified
            string DBServerIP = "";
            string msId = "";
            if (monitorServerID.Trim('|').IndexOf('|') > 0)
            {
                string[] array = monitorServerID.Trim('|').Split('|');
                DBServerIP = "'" + array[0] + "'";
                msId = array[1];
            }
            else
            {
                DBServerIP = Common.CommonUtil.GetLoginIPWithQuote();
                msId = monitorServerID;
            }

            //file or folder
            if (monitorFileName != "")
            {
                if (dirname == "0")
                {
                    where += " monitorFileName like '%" + monitorFileName + "%'";// AND monitorFileRelativeDirectory not like '%\\" + monitorFileName + "%'
                }
                else
                {
                    where += " monitorFileRelativeDirectory like '%\\" + monitorFileName + "%'";//id IN(select min(id) from (SELECT min(id) as id,min(CHARINDEX('\\" + monitorFileName + "', monitorFileRelativeDirectory)) AS patindexs FROM monitorFileListen where deleteFlg=0 and monitorType<>'削除' group by monitorFileRelativeDirectory) as a group by a.patindexs) AND
                }
                where += " AND ";
            }

            //one or all monitorServers
            if (msId != "-1")
            {
                where += " monitorServerID = " + msId + " AND ";
            }

            //add condition DBServerIP——2014-06-04 wjd modified
            where += " DBServerIP in (" + DBServerIP + ") ";

            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                count = MonitorFileListenDal.GetMonitorFileListenListCount(where, conn);
                conn.Close();
                return count;
            }
        }
    }
}
