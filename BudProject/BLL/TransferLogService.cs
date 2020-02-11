using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using IBLL;
using Model;
using IDAL;
using DBUtility;
using System.Data.Odbc;
using Common;

namespace BLL
{
    public class TransferLogService:ITransferLogService
    {
        private ITransferLog TransferLogDal = DALFactory.DataAccess.CreateTransferLog();
        /// <summary>
        /// 転送容量の取得
        /// </summary>
        /// <param name="groupId">グループid</param>
        /// <param name="dtStart">開始日付</param>
        /// <param name="dtEnd">終了日付</param>
        /// <param name="tmStart">開始時間</param>
        /// <param name="tmEnd">終了時間</param>
        /// <param name="displayFlg">表示フラグ</param>
        /// <param name="tranferLfg">0:転送中-完了両方；1:転送中のみ</param>
        /// <param name="stateFlg">0:OK-NG両方；1:NGのみ</param>
        /// <param name="logFlg">0:ログ表示；1:転送容量表示</param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogListByProc(int groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferFlg, int stateFlg, int logFlg)
        {
            IList<TransferLog> LogList;
            OdbcParameter[] parameters = {
					new OdbcParameter("@groupId", groupId),
					new OdbcParameter("@StartDate", dtStart),
					new OdbcParameter("@EndDate", dtEnd),
					new OdbcParameter("@StartTime", tmStart),
					new OdbcParameter("@EndTime", tmEnd),
					new OdbcParameter("@TranferFlg", tranferFlg),
					new OdbcParameter("@StateFlg", stateFlg),
					new OdbcParameter("@LogFlg", logFlg)};
            OdbcConnection conn;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                //sqlcommand ストアド実行方法
                //LogList = TransferLogDal.GetTransferLogListByProc(conn, parameters, "sp_GetTranferList");
                //odbccommand　ストアド実行方法
                LogList = TransferLogDal.GetTransferLogListByProc(conn, parameters, "{call sp_GetTranferList(?,?,?,?,?,?,?,?)}");
                conn.Close();
                return LogList;
            }
        }

        /// <summary>
        /// 転送容量の取得
        /// </summary>
        /// <param name="groupId">グループid</param>
        /// <param name="dtStart">開始日付</param>
        /// <param name="dtEnd">終了日付</param>
        /// <param name="tmStart">開始時間</param>
        /// <param name="tmEnd">終了時間</param>
        /// <param name="displayFlg">表示フラグ</param>
        /// <param name="tranferLfg">0:転送中-完了両方；1:転送中のみ</param>
        /// <param name="stateFlg">0:OK-NG両方；1:NGのみ</param>
        /// <param name="logFlg">0:ログ表示；1:転送容量表示</param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogListByProc(int pindex, int pagesize, string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferFlg, int stateFlg, int logFlg, string name)
        {
            IList<TransferLog> LogList;
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
					new OdbcParameter("@TranferFlg", tranferFlg),
					new OdbcParameter("@StateFlg", stateFlg),
					new OdbcParameter("@LogFlg", logFlg),
                    new OdbcParameter("@backupServerFileName", name),
                    new OdbcParameter("@Pindex", pindex),
                    new OdbcParameter("@Psize", pagesize)};
            OdbcConnection conn;
            //2014-06-03 wjd modified
            using (conn = OdbcHelper.CreateConntionAll())
            {
                conn.Open();
                //sqlcommand ストアド実行方法
                //LogList = TransferLogDal.GetTransferLogListByProc(conn, parameters, "sp_GetTranferList");
                //odbccommand　ストアド実行方法
                LogList = TransferLogDal.GetTransferLogListByProc(conn, parameters, "{call sp_GetTransferList2(?,?,?,?,?,?,?,?,?,?,?,?)}");
                conn.Close();
                return LogList;
            }
        }

        /// <summary>
        /// 転送容量の取得 
        /// 忽略OK・NG両方 OKのみ NGのみ 即都是一样的 xiecongwen 20140710
        /// </summary>
        /// <param name="groupId">グループid</param>
        /// <param name="dtStart">開始日付</param>
        /// <param name="dtEnd">終了日付</param>
        /// <param name="tmStart">開始時間</param>
        /// <param name="tmEnd">終了時間</param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogList(string groupIdString, string dtStart, string dtEnd, string tmStart, string tmEnd, string name)
        {
            IList<TransferLog> LogList;
            #region DBServerIP和groupID 获取
            //groupId format: DBServerIP|group Id——2014-07-02 wjd modified
            string DBServerIP = "";
            int groupID = 0;
            if (groupIdString.Trim('|').IndexOf('|') > 0)
            {
                string[] array = groupIdString.Trim('|').Split('|');
                DBServerIP = "'" + array[0] + "'";
                Int32.TryParse(array[1],out groupID);
            }
            else
            {
                DBServerIP = Common.CommonUtil.GetLoginIPWithQuote();
                Int32.TryParse(groupIdString, out groupID);
            }
            #endregion
            #region 时间获取 暂时简单处理 xiecongwen 20140710
            if (string.IsNullOrWhiteSpace(tmStart))
            {
                tmStart = "00:00:00";
            }
            if (string.IsNullOrWhiteSpace(tmEnd))
            {
                tmEnd = "23:59:59";
            }
            #endregion

            LogList = TransferLogDal.GetTransferLogList(DBServerIP,groupID, dtStart, dtEnd, tmStart, tmEnd, name);
            return LogList;
        }

        /// <summary>
        /// 転送容量の取得 
        /// 启用OK・NG両方 OKのみ NGのみ
        /// </summary>
        /// <param name="groupId">グループid</param>
        /// <param name="dtStart">開始日付</param>
        /// <param name="dtEnd">終了日付</param>
        /// <param name="tmStart">開始時間</param>
        /// <param name="tmEnd">終了時間</param>
        /// <param name="name">file or folder name for searching</param>
        /// <param name="stateFlg">1:OK, 0:NG or 2:both</param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogList(string groupIdString, string dtStart, string dtEnd, string tmStart, string tmEnd, string name,
            int stateFlg)
        {
            IList<TransferLog> LogList;
            #region DBServerIP和groupID 获取
            //groupId format: DBServerIP|group Id——2014-07-02 wjd modified
            string DBServerIP = "";
            int groupID = 0;
            if (groupIdString.Trim('|').IndexOf('|') > 0)
            {
                string[] array = groupIdString.Trim('|').Split('|');
                DBServerIP = "'" + array[0] + "'";
                Int32.TryParse(array[1], out groupID);
            }
            else
            {
                DBServerIP = Common.CommonUtil.GetLoginIPWithQuote();
                Int32.TryParse(groupIdString, out groupID);
            }
            #endregion
            #region 时间获取 暂时简单处理 xiecongwen 20140710
            if (string.IsNullOrWhiteSpace(tmStart))
            {
                tmStart = "00:00:00";
            }
            if (string.IsNullOrWhiteSpace(tmEnd))
            {
                tmEnd = "23:59:59";
            }
            #endregion

            LogList = TransferLogDal.GetTransferLogList(groupID, dtStart, dtEnd, tmStart, tmEnd, name,
                stateFlg.ToString());
            return LogList;
        }

        /// <summary>
        /// 転送容量の取得 
        /// 忽略OK・NG両方 OKのみ NGのみ 即都是一样的 xiecongwen 20140710
        /// </summary>
        /// For Winform——2014-8-25 wjd add
        /// <param name="groupId">グループid</param>
        /// <param name="dtStart">開始日付</param>
        /// <param name="dtEnd">終了日付</param>
        /// <param name="tmStart">開始時間</param>
        /// <param name="tmEnd">終了時間</param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogList(string groupIdString, DateTime dtStart, DateTime dtEnd, string tmStart, string tmEnd, string name)
        {
            IList<TransferLog> LogList;
            int groupID = 0;
            Int32.TryParse(groupIdString, out groupID);
            string startDate = CommonUtil.ToShortDateString(dtStart);
            string endDate = CommonUtil.ToShortDateString(dtEnd);
            #region 时间获取 暂时简单处理 xiecongwen 20140710
            if (string.IsNullOrWhiteSpace(tmStart))
            {
                tmStart = "00:00:00";
            }
            if (string.IsNullOrWhiteSpace(tmEnd))
            {
                tmEnd = "23:59:59";
            }
            #endregion

            LogList = TransferLogDal.GetTransferLogList(groupID, startDate, endDate, tmStart, tmEnd, name);
            return LogList;
        }

        /// <summary>
        /// 転送容量の取得 
        /// 启用OK・NG両方 OKのみ NGのみ
        /// </summary>
        /// For Winform——2014-12-02 wjd add
        /// <param name="groupId">グループid</param>
        /// <param name="dtStart">開始日付</param>
        /// <param name="dtEnd">終了日付</param>
        /// <param name="tmStart">開始時間</param>
        /// <param name="tmEnd">終了時間</param>
        /// <param name="backupFlg">1:OK, 0:NG or 99:both</param>
        /// <returns></returns>
        public IList<TransferLog> GetTransferLogList(string groupIdString, DateTime dtStart, DateTime dtEnd, string tmStart, string tmEnd, string name,
            string backupFlg)
        {
            IList<TransferLog> LogList;
            int groupID = 0;
            Int32.TryParse(groupIdString, out groupID);
            string startDate = CommonUtil.ToShortDateString(dtStart);
            string endDate = CommonUtil.ToShortDateString(dtEnd);
            #region 时间获取 暂时简单处理 xiecongwen 20140710
            if (string.IsNullOrWhiteSpace(tmStart))
            {
                tmStart = "00:00:00";
            }
            if (string.IsNullOrWhiteSpace(tmEnd))
            {
                tmEnd = "23:59:59";
            }
            #endregion

            LogList = TransferLogDal.GetTransferLogList(groupID, startDate, endDate, tmStart, tmEnd, name,
                backupFlg);
            return LogList;
        }
    }
}
