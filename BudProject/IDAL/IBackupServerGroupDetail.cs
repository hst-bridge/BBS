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
    public interface IBackupServerGroupDetail
    {
        /// <summary>
        /// 增加バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        //int InsertBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail, SqlConnection conn);
        /// <summary>
        /// 更新バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        //int UpdateBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ先対象グループ明細id删除バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        //int DeleteBackupServerGroupDetailById(int BackupServerGroupDetailId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ先対象グループidとバックアップ先対象id 删除バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        //int DeleteBackupServerGroupDetail(int BackupServerId, int BackupGroupId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象グループ明細
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<BackupServerGroupDetail> GetBackupServerGroupDetail(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象グループ明細数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetBackupServerGroupDetailCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象グループ明細
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<BackupServerGroupDetail> GetBackupServerGroupDetailPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        ////////////////////////////////////////////////////////////ODBC connection//////////////////////////////////////////////////////////////////
        /// <summary>
        /// 增加バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        int InsertBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        int UpdateBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ先対象グループ明細id删除バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        int DeleteBackupServerGroupDetailById(int BackupServerGroupDetailId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ先対象グループidとバックアップ先対象id 删除バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        int DeleteBackupServerGroupDetail(int BackupServerId, int BackupGroupId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ先対象グループid删除バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupGroupId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        int DeleteBackupServerGroupDetailByGroupId(int BackupGroupId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象グループ明細
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<BackupServerGroupDetail> GetBackupServerGroupDetail(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象グループ明細数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetBackupServerGroupDetailCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象グループ明細
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<BackupServerGroupDetail> GetBackupServerGroupDetailPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
    }
}
