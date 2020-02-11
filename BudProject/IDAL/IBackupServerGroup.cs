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
    public interface IBackupServerGroup
    {
        /// <summary>
        /// 增加バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <returns></returns>
        //int InsertBackupServerGroup(BackupServerGroup BackupServerGroup, SqlConnection conn);
        /// <summary>
        /// 更新バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <returns></returns>
        //int UpdateBackupServerGroup(BackupServerGroup BackupServerGroup, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ先対象グループid删除バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <returns></returns>
        //int DeleteBackupServerGroupById(int BackupServerGroupId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象グループ
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<BackupServerGroup> GetBackupServerGroup(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象グループ数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetBackupServerGroupCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象グループ
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<BackupServerGroup> GetBackupServerGroupPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        ////////////////////////////////////////////////////////////ODBC connection//////////////////////////////////////////////////////////////////
        /// <summary>
        /// 增加バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <returns></returns>
        int InsertBackupServerGroup(BackupServerGroup BackupServerGroup, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <returns></returns>
        int UpdateBackupServerGroup(BackupServerGroup BackupServerGroup, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ先対象グループid删除バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <returns></returns>
        int DeleteBackupServerGroupById(int BackupServerGroupId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象グループ
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<BackupServerGroup> GetBackupServerGroup(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象グループ from All DB
        /// </summary>
        /// 2014-07-02 wjd add
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<BackupServerGroup> GetBackupServerGroupAll(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象グループ
        /// </summary>
        /// 2014-06-23 wjd add
        /// <param name="backupServerID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<BackupServerGroup> GetBackupServerGroupByBackupServerID(int backupServerID, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象グループ数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetBackupServerGroupCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象グループ
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<BackupServerGroup> GetBackupServerGroupPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
    }
}
