using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Model;
using System.Data.Odbc;

namespace IDAL
{
    public interface ITransferLog
    {
        //IList<TransferLog> GetTransferLogListByProc(SqlConnection conn, SqlParameter[] paras, string strProcedureName);
        /////////////////////////////////////////////////ODBC connection////////////////////////////////////////
        IList<TransferLog> GetTransferLogListByProc(OdbcConnection conn, OdbcParameter[] paras, string strProcedureName);
        IList<TransferLog> GetTransferLogList(string DBServerIP, int groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, string name);

        /// <summary>
        /// Get transfer volume with stateFlg
        /// </summary>
        /// <param name="DBServerIP"></param>
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="name">file or folder name for searching</param>
        /// <param name="stateFlg">1:OK, 0:NG or 2:both</param>
        /// <returns></returns>
        IList<TransferLog> GetTransferLogList(string DBServerIP, int groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, string name,
            int stateFlg);

        IList<TransferLog> GetTransferLogList(int groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, string name);

        /// <summary>
        /// Get transfer volume with backupFlg
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="name">file or folder name for searching</param>
        /// <param name="backupFlg">1:OK, 0:NG or 99:both</param>
        /// <returns></returns>
        IList<TransferLog> GetTransferLogList(int groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, string name,
            string backupFlg);
    }
}
