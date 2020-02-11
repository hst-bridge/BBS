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
    public interface IBackupServer
    {
        /// <summary>
        /// 增加バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        //int InsertBackupServer(BackupServer BackupServer, SqlConnection conn);
        /// <summary>
        /// 更新バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        //int UpdateBackupServer(BackupServer BackupServer, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ先対象id删除バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        //int DeleteBackupServerById(int BackupServerId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<BackupServer> GetBackupServer(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetBackupServerCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<BackupServer> GetBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        /// <summary>
        /// get part backup server list
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<BackupServer> GetPartBackupServerList(SqlConnection conn, string groupId);
        /// <summary>
        /// get the server group detail list 
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<BackupServer> GetGroupBackupServerList(SqlConnection conn, string groupId);
        /////////////////////////////////ODBC connection///////////////////////////////////////
        /// <summary>
        /// 增加バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int InsertBackupServer(BackupServer BackupServer, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int UpdateBackupServer(BackupServer BackupServer, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ先対象id删除バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int DeleteBackupServerById(int BackupServerId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<BackupServer> GetBackupServer(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetBackupServerCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<BackupServer> GetBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
        /// <summary>
        /// get part backup server list
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<BackupServer> GetPartBackupServerList(OdbcConnection conn, string groupId);
        /// <summary>
        /// get the server group detail list 
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<BackupServer> GetGroupBackupServerList(OdbcConnection conn, string groupId);
    }
}
