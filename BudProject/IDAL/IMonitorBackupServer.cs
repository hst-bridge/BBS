using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.Data.SqlClient;
using Common;
using System.Data.Odbc;

namespace IDAL
{
    public interface IMonitorBackupServer
    {
        /// <summary>
        /// 增加バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <returns></returns>
        //int InsertMonitorBackupServer(MonitorBackupServer MonitorBackupServer, SqlConnection conn);
        /// <summary>
        /// 更新バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <returns></returns>
        //int UpdateMonitorBackupServer(MonitorBackupServer MonitorBackupServer, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ転送サーバ関係id删除バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <returns></returns>
        //int DeleteMonitorBackupServerById(int MonitorBackupServerId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ転送サーバ関係
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<MonitorBackupServer> GetMonitorBackupServer(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ転送サーバ関係数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetMonitorBackupServerCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ転送サーバ関係
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<MonitorBackupServer> GetMonitorBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        ////////////////////////////////////////////////////////////ODBC connection//////////////////////////////////////////////////////////////////
        /// <summary>
        /// 增加バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <returns></returns>
        int InsertMonitorBackupServer(MonitorBackupServer MonitorBackupServer, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <returns></returns>
        int UpdateMonitorBackupServer(MonitorBackupServer MonitorBackupServer, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ転送サーバ関係id删除バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <returns></returns>
        int DeleteMonitorBackupServerById(int MonitorBackupServerId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ転送サーバ関係
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<MonitorBackupServer> GetMonitorBackupServer(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ転送サーバ関係数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetMonitorBackupServerCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ転送サーバ関係
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<MonitorBackupServer> GetMonitorBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
    }
}
