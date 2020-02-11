using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace IBLL
{
    public interface ITransferLogService
    {
        IList<TransferLog> GetTransferLogListByProc(int groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg);
        IList<TransferLog> GetTransferLogListByProc(int pindex, int pagesize, string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg, string name);
        IList<TransferLog> GetTransferLogList(string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, string name);

        /// <summary>
        /// Get transfer volume with stateFlg
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="tmStart"></param>
        /// <param name="tmEnd"></param>
        /// <param name="name">file or folder name for searching</param>
        /// <param name="stateFlg">1:OK, 0:NG or 2:both</param>
        /// <returns></returns>
        IList<TransferLog> GetTransferLogList(string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, string name,
            int stateFlg);

        IList<TransferLog> GetTransferLogList(string groupId, DateTime dtStart, DateTime dtEnd, string tmStart, string tmEnd, string name);

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
        IList<TransferLog> GetTransferLogList(string groupId, DateTime dtStart, DateTime dtEnd, string tmStart, string tmEnd, string name,
            string backupFlg);
    }
}
