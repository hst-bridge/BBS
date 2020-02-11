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
    public interface IManualBackupServer
    {
        /// <summary>
        /// 增加バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int InsertManualBackupServer(ManualBackupServer ManualBackupServer, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int UpdateManualBackupServer(ManualBackupServer ManualBackupServer, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ先対象id删除バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int DeleteManualBackupServerById(int BackupServerId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<ManualBackupServer> GetManualBackupServer(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetManualBackupServerCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<ManualBackupServer> GetManualBackupServerPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
    }
}
