using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;
using System.Data;

namespace IBLL
{
    public interface IMonitorFileListenService
    {
        int UpdateMonitorServer(MonitorServer MonitorServer, MonitorServer MonitorServerForOld);
        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="LogId"></param>
        /// <returns></returns>
        MonitorFileListen GetMonitorFileListenById(int listenId);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetListenCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// ファイルダウンロードリスト
        /// </summary>
        /// <returns></returns>
        IList<MonitorFileListen> GetMonitorFileListenList();
        /// <summary>
        /// 获取满足条件的ファイルダウンロードリスト
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="updateDate">バックアップ日</param>
        /// <returns></returns>
        IList<MonitorFileListen> GetMonitorFileListenList(string fileName, string updateDate);
        IList<MonitorFileListen> GetMonitorFileListenList(string fileName);
        IList<MonitorFileListen> GetMonitorFileListenList(string fileName, int monitorServerId);
        IList<MonitorFileListen> GetMonitorFileListenListByPage(string fileName, string monitorServerId, string dirname, int pindex, int psize);
        int GetMonitorFileListenListCount(string monitorFileName, string monitorServerID, string dirname);
    }
}
