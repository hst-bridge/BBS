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
    public interface IMonitorServer
    {
        /// <summary>
        /// 增加バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        //int InsertMonitorServer(MonitorServer MonitorServer, SqlConnection conn);
        /// <summary>
        /// 更新バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        //int UpdateMonitorServer(MonitorServer MonitorServer, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ元id删除バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        //int DeleteMonitorServerById(int MonitorServerId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ元
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<MonitorServer> GetMonitorServer(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ元数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetMonitorServerCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ元
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<MonitorServer> GetMonitorServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        ////////////////////////////////////////////////////////////ODBC connection//////////////////////////////////////////////////////////////////
        /// <summary>
        /// 增加バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        int InsertMonitorServer(MonitorServer MonitorServer, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        int UpdateMonitorServer(MonitorServer MonitorServer, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ元id删除バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        int DeleteMonitorServerById(int MonitorServerId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ元id更新バックアップ元copyInit
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        int UpdateMonitorServerCopyInit(int MonitorServerId, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ元
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<MonitorServer> GetMonitorServer(IEnumerable<SearchCondition> conditon, OdbcConnection conn);

        /// <summary>
        /// 根据条件获取バックアップ元 from all DB
        /// </summary>
        /// 2014-07-06 wjd add
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<MonitorServer> GetMonitorServerAll(IEnumerable<SearchCondition> conditon, OdbcConnection conn);

        /// <summary>
        /// 获取满足条件的バックアップ元数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetMonitorServerCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ元
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<MonitorServer> GetMonitorServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        int GetMaxId(OdbcConnection conn);
    }
}
