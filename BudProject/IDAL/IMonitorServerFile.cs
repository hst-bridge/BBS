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
    public interface IMonitorServerFile
    {
        /// <summary>
        /// 增加バックアップ元ファイル
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        //int InsertMonitorServerFile(MonitorServerFile MonitorServerFile, SqlConnection conn);
        /// <summary>
        /// 更新バックアップ元ファイル
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        //int UpdateMonitorServerFile(MonitorServerFile MonitorServerFile, SqlConnection conn);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //int UpdateTransferFlg(string MonitorServerFileId, int transferFlg ,SqlConnection conn);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //int UpdateTransferFlgBatch(string MonitorServerFileId, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ元ファイルid删除バックアップ元ファイル
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        //int DeleteMonitorServerFileById(int MonitorServerFileId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ元ファイル
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<MonitorServerFile> GetMonitorServerFile(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ元ファイル数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetMonitorServerFileCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ元ファイル
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<MonitorServerFile> GetMonitorServerFilePage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        ////////////////////////////////////////////////////////////ODBC connection//////////////////////////////////////////////////////////////////
        /// <summary>
        /// 增加バックアップ元ファイル
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int InsertMonitorServerFile(MonitorServerFile MonitorServerFile, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ元ファイル
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int UpdateMonitorServerFile(MonitorServerFile MonitorServerFile, OdbcConnection conn);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        int UpdateTransferFlg(string MonitorServerFileId, int transferFlg, OdbcConnection conn);


        /// </summary>
        /// <param name="MonitorFileDirectory"></param>
        /// <param name="transferFlg"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        int UpdateAllTransferFlg(string MonitorFileDirectory, int transferFlg, OdbcConnection conn);

        /// 
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <param name="transferFlg"></param>
        /// <param name="transferNum"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        int UpdateNGTransferFlg(string MonitorServerFileId, int transferFlg, int transferNum, OdbcConnection conn);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        int UpdateTransferFlgBatch(string MonitorServerFileId, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ元ファイルid删除バックアップ元ファイル
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int DeleteMonitorServerFileById(int MonitorServerFileId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ元ファイル
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<MonitorServerFile> GetMonitorServerFile(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ元ファイル数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetMonitorServerFileCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ元ファイル
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<MonitorServerFile> GetMonitorServerFilePage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
    }
}
