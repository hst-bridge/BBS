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
    public interface IMonitorServerFolder
    {
        /// <summary>
        /// 增加バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        //int InsertMonitorServerFolder(MonitorServerFolder MonitorServerFolder, SqlConnection conn);
        /// <summary>
        /// 更新バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        //int UpdateMonitorServerFolder(MonitorServerFolder MonitorServerFolder, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ元フォルダid删除バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        //int DeleteMonitorServerFolderById(int MonitorServerFolderId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ元フォルダ
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<MonitorServerFolder> GetMonitorServerFolder(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ元フォルダ数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetMonitorServerFolderCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ元フォルダ
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<MonitorServerFolder> GetMonitorServerFolderPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ元フォルダmonitorServerID删除バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        //int DeleteMonitorServerFolderByServerId(int MonitorServerFolderId, SqlConnection conn);
        ////////////////////////////////////////////////////////////ODBC connection//////////////////////////////////////////////////////////////////
        /// <summary>
        /// 增加バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        int InsertMonitorServerFolder(MonitorServerFolder MonitorServerFolder, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        int UpdateMonitorServerFolder(MonitorServerFolder MonitorServerFolder, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ元フォルダid删除バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        int DeleteMonitorServerFolderById(int MonitorServerFolderId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ元フォルダ
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<MonitorServerFolder> GetMonitorServerFolder(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ元フォルダ数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetMonitorServerFolderCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ元フォルダ
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<MonitorServerFolder> GetMonitorServerFolderPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ元フォルダmonitorServerID删除バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        int DeleteMonitorServerFolderByServerId(int MonitorServerFolderId, OdbcConnection conn);
    }
}
