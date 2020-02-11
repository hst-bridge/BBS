using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;
using System.Data;

namespace IBLL
{
    public interface ILogService
    {
        /// <summary>
        /// 增加ログ
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        int InsertLog(Log Log);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="LogId"></param>
        /// <returns></returns>
        int DeleteLog(int LogId, string loginID);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        int UpdateLog(Log Log);
        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="LogId"></param>
        /// <returns></returns>
        Log GetLogById(int LogId);
        /// <summary>
        /// 根据条件获取
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<Log> GetLogPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetLogCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// ログリスト
        /// </summary>
        /// <returns></returns>
        IList<Log> GetLogList();
        /// <summary>
        /// 获取满足条件的ログリスト
        /// </summary>
        /// <param name="filename">ファイル名</param>
        /// <param name="transferFlg">0 転送中・完了両方; 1 転送のみ</param>
        /// <param name="backupFlg">0 OK・NG両方; 1 NGのみ</param>
        /// <param name="serverGroupID">-1 すべて</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<Log> GetLogList(string filename, string transferFlg, string backupFlg, string serverGroupID, DateTime startDate, DateTime endDate, string startTime, string endTime);

        IList<Log> GetLogListByProc(int pindex, int pagesize, string groupId, DateTime dtStart, DateTime dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg, string name);

        IList<Log> GetLogListByProc(int groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg);
        IList<Log> GetLogListByProc(string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg, string name);
        IList<Log> GetLogListByProc(int pindex, int pagesize, string groupId, string dtStart, string dtEnd, string tmStart, string tmEnd, int displayFlg, int tranferLfg, int stateFlg, int logFlg, string name);
        
    }
}
