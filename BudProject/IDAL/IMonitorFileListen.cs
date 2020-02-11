using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Model;
using Common;
using System.Data.Odbc;

namespace IDAL
{
    public interface IMonitorFileListen
    {
        /// <summary>
        /// 更新バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        int UpdateMonitorFileListen(MonitorServer MonitorServer, MonitorServer MonitorServerForOld, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<MonitorFileListen> GetMonitorFileListen(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetMonitorFileListenCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<MonitorFileListen> GetMonitorFileListenPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象
        /// </summary>
        /// <param name="where"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<MonitorFileListen> GetMonitorFileListenPage(string where, int page, int pagesize, OdbcConnection conn);
        /// <summary>
        /// get part backup server list
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<MonitorFileListen> GetPartMonitorFileListenList(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// get the server group detail list 
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<MonitorFileListen> GetMonitorFileListenList(OdbcConnection conn, string groupId);
        IList<MonitorFileListen> GetPartMonitorFileListenList(string where, OdbcConnection conn);
        int GetMonitorFileListenListCount(string where, OdbcConnection conn);
        /// <summary>
        /// get monitorfilelisten list xiecongwen 20140711
        /// </summary>
        /// <param name="DBServerIP"></param>
        /// <param name="monitorServerId"></param>
        /// <param name="keyName"></param>
        /// <param name="pindex"></param>
        /// <param name="psize"></param>
        /// <returns></returns>
        IList<MonitorFileListen> GetMonitorFileListenByPage(string DBServerIP, string monitorServerId, string keyName, int pindex, int psize);
    }
}
