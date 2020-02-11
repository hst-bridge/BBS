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
using System.Data;
using System.Data.Odbc;

namespace BLL
{
    public class LogService : ILogService
    {
        private ILog LogDal = DALFactory.DataAccess.CreateLog();
        public int GetLogCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = LogDal.GetLogCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        public int DeleteLog(int LogId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = LogDal.DeleteLogById(LogId,loginID, conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateLog(Log Log)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = LogDal.UpdateLog(Log, conn);
                conn.Close();
            }
            return count;
        }

        public int InsertLog(Log Log)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = LogDal.InsertLog(Log, conn);
                conn.Close();
            }
            return count;
        }

        public Log GetLogById(int LogId)
        {
            OdbcConnection conn;
            IList<Log> Log;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = LogId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                Log = LogDal.GetLog(condition, conn);
                if (Log.Count > 0)
                {
                    return Log[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<Log> GetLogPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<Log> Log;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                Log = LogDal.GetLogPage(condition, page, pagesize, conn);
                conn.Close();
                return Log;
            }
        }

        public IList<Log> GetLogList()
        {
            OdbcConnection conn;
            IList<Log> Log;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                Log = LogDal.GetLog(condition, conn);
                conn.Close();
                return Log;
            }
        }
        public IList<Log> GetLogList(string filename, string transferFlg, string backupFlg, string serverGroupID, DateTime startdate, DateTime enddate, string startTime, string endTime)
        {
            OdbcConnection conn;
            IList<Log> Log;
            string where = "deleteFlg = 0";

            if (transferFlg == "1")
            {
                where += " AND transferFlg != 1";
            }

            if (backupFlg == "1")
            {
                where += " AND backupFlg = 1";
            }
            else if (backupFlg == "0")
            {
                where += " AND backupFlg = 0";
            }

            if (serverGroupID != "-1")
            {
                where += " AND backupServerGroupID = " + serverGroupID;
            }
            // 20140407 バグ対応 ファイル名検索
            if (!String.IsNullOrEmpty(filename))
            {
                where += " AND monitorFileName like " + "'%" + filename + "%'";
            }
            string strOr = "";
            string starttime = startTime == "" ? "00:00:00" : startTime;
            startTime = starttime;
            string endtime = endTime == "" ? "23:59:59" : endTime;
            endTime = endtime;
            int totalDays = Convert.ToInt32((enddate-startdate).TotalDays);
            string startDate = CommonUtil.ToShortDateString(startdate);
            string endDate = CommonUtil.ToShortDateString(enddate);
            if (startDate != "" && endDate != "")
            {
                if (totalDays == 0)
                {
                    string startDateTime = startDate + " " + startTime;
                    string endDateTime = endDate + " " + endTime;
                    strOr += "(backupStartTime >= '" + startDateTime + "'";
                    strOr += " AND backupStartTime <= '" + endDateTime + "')";
                }
                else
                {
                    for (int i = 0; i <= totalDays; i++)
                    {
                        string startDateTime = CommonUtil.ToShortDateString(startdate.AddDays(i)) + " " + startTime;
                        string endDateTime = CommonUtil.ToShortDateString(startdate.AddDays(i)) + " " + endTime;
                        if (i == 0) 
                        {
                            strOr += "(backupStartTime >= '" + startDateTime + "'";
                            strOr += " AND backupStartTime <= '" + endDateTime + "')";
                        }
                        else
                        {
                            strOr += " OR (backupStartTime >= '" + startDateTime + "'";
                            strOr += " AND backupStartTime <= '" + endDateTime + "')";
                        }
                    }
                }
            }
            if (strOr != "") 
            {
                where += " AND (" + strOr + ")";
            }
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                Log = LogDal.GetConditionLog(where, conn);
                conn.Close();
                return Log;
            }
        }

        public IList<Log> GetLogListByProc(int groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg)
        {
            DataTable dt = new DataTable();
            IList<Log> LogList;
            OdbcParameter[] parameters = {
					new OdbcParameter("@groupId", groupId),
					new OdbcParameter("@StartDate", dtStart),
					new OdbcParameter("@EndDate", dtEnd),
					new OdbcParameter("@StartTime", tmStart),
					new OdbcParameter("@EndTime", tmEnd),
					new OdbcParameter("@TranferFlg", tranferLfg),
					new OdbcParameter("@StateFlg", stateFlg),
					new OdbcParameter("@LogFlg", logFlg)};
            OdbcConnection conn;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                //sqlcommand ストアド実行方法
                //LogList = LogDal.GetLogListByProc(conn, parameters, "sp_GetTranferList");
                //odbccommand　ストアド実行方法
                LogList = LogDal.GetLogListByProc(conn, parameters, "{call sp_GetTranferList(?,?,?,?,?,?,?,?)}");
                conn.Close();
                return LogList;
            }
        }

        /// <summary>
        /// Get Log For Winform
        /// </summary>
        /// 2014-06-03 wjd modified
        /// <param name="pindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="displayFlg"></param>
        /// <param name="tranferLfg"></param>
        /// <param name="stateFlg"></param>
        /// <param name="logFlg"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IList<Log> GetLogListByProc(int pindex, int pagesize, string groupId, DateTime dtStart, DateTime dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg, string name)
        {
            IList<Log> LogList;
            OdbcParameter[] parameters = {
					new OdbcParameter("@groupId", groupId),
					new OdbcParameter("@StartDate", dtStart.ToString("yyyy-MM-dd")),
					new OdbcParameter("@EndDate", dtEnd.ToString("yyyy-MM-dd")),
					new OdbcParameter("@StartTime", tmStart),
					new OdbcParameter("@EndTime", tmEnd),
					new OdbcParameter("@TranferFlg", tranferLfg),
					new OdbcParameter("@StateFlg", stateFlg),
					new OdbcParameter("@LogFlg", logFlg),
                    new OdbcParameter("@backupServerFileName", name),
                    new OdbcParameter("@Pindex", pindex),
                    new OdbcParameter("@Psize", pagesize)};
            OdbcConnection conn;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                LogList = LogDal.GetLogListByProc(conn, parameters, "{call sp_GetTransferList2(?,?,?,?,?,?,?,?,?,?,?)}");
                conn.Close();
                return LogList;
            }
        }

        public IList<Log> GetLogListByProc(string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg, string name)
        {
            IList<Log> LogList;
            //groupId format: DBServerIP|group Id——2014-07-02 wjd modified
            string DBServerIP = "";
            string group = "";
            if (groupId.Trim('|').IndexOf('|') > 0)
            {
                string[] array = groupId.Trim('|').Split('|');
                DBServerIP = "'" + array[0] + "'";
                group = array[1];
            }
            else
            {
                DBServerIP = Common.CommonUtil.GetLoginIPWithQuote();
                group = groupId;
            }
            OdbcParameter[] parameters = {
					new OdbcParameter("@DBServerIP", DBServerIP),
					new OdbcParameter("@groupId", group),
					new OdbcParameter("@StartDate", dtStart),
					new OdbcParameter("@EndDate", dtEnd),
					new OdbcParameter("@StartTime", tmStart),
					new OdbcParameter("@EndTime", tmEnd),
					new OdbcParameter("@TranferFlg", tranferLfg),
					new OdbcParameter("@StateFlg", stateFlg),
					new OdbcParameter("@LogFlg", logFlg),
                    new OdbcParameter("@backupServerFileName", name)};
            OdbcConnection conn;
            //2014-06-03 wjd modified
            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                //sqlcommand ストアド実行方法
                //LogList = LogDal.GetLogListByProc(conn, parameters, "sp_GetTranferList");
                //odbccommand　ストアド実行方法
                LogList = LogDal.GetLogListByProc(conn, parameters, "{call sp_GetTranferList(?,?,?,?,?,?,?,?,?,?)}");
                conn.Close();
                return LogList;
            }
        }
        public IList<Log> GetLogListByProc(int pindex, int pagesize, string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg, string name)
        {
            IList<Log> LogList;
            
            OdbcParameter[] parameters = {
					//new OdbcParameter("@DBServerIP", DBServerIP),
					new OdbcParameter("@groupId", groupId),
					new OdbcParameter("@StartDate", dtStart),
					new OdbcParameter("@EndDate", dtEnd),
					new OdbcParameter("@StartTime", tmStart),
					new OdbcParameter("@EndTime", tmEnd),
					new OdbcParameter("@TranferFlg", tranferLfg),
					new OdbcParameter("@StateFlg", stateFlg),
					new OdbcParameter("@LogFlg", logFlg),
                    new OdbcParameter("@backupServerFileName", name),
                    new OdbcParameter("@Pindex", pindex),
                    new OdbcParameter("@Psize", pagesize)};
            OdbcConnection conn;
            //2014-06-03 wjd modified
            using (conn = OdbcHelper.CreateConntion()) //xiecongwen 20141217 改为访问各自数据库 98部分弃用
            {
                conn.Open();
                
                LogList = LogDal.GetLogListByProc(conn, parameters, "{call sp_GetTransferList2(?,?,?,?,?,?,?,?,?,?,?)}");

                conn.Close();
                return LogList;
            }
        }
    }
}
