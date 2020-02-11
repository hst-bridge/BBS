using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IBackupServerGroupDetailService
    {
        /// <summary>
        /// 增加バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        int InsertBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail);
        /// <summary>
        /// 删除バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetailId"></param>
        /// <returns></returns>
        int DeleteBackupServerGroupDetail(int BackupServerGroupDetailId, string loginID);
        /// <summary>
        /// 删除バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetailId"></param>
        /// <returns></returns>
        int DeleteBackupServerGroupDetail(int BackupServerId, int BackupGroupId, string loginID);
        /// <summary>
        /// 删除バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupGroupId"></param>
        /// <param name="loginID"></param>
        /// <returns></returns>
        int DeleteBackupServerGroupDetailByGroupId(int BackupGroupId, string loginID);
        /// <summary>
        /// 更新バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetail"></param>
        /// <returns></returns>
        int UpdateBackupServerGroupDetail(BackupServerGroupDetail BackupServerGroupDetail);
        /// <summary>
        /// 根据ID获取バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupDetailId"></param>
        /// <returns></returns>
        BackupServerGroupDetail GetBackupServerGroupDetailById(int BackupServerGroupDetailId);
        /// <summary>
        /// 根据グループID获取バックアップ先対象グループ明細
        /// </summary>
        /// <param name="BackupServerGroupId"></param>
        /// <returns></returns>
        IList<BackupServerGroupDetail> GetBackupServerGroupDetailByGroupId(string BackupServerGroupId);
        /// <summary>
        /// 根据条件获取バックアップ先対象グループ明細
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<BackupServerGroupDetail> GetBackupServerGroupDetailPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetBackupServerGroupDetailCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// バックアップ先対象グループ明細リスト
        /// </summary>
        /// <returns></returns>
        IList<BackupServerGroupDetail> GetBackupServerGroupDetailList();
    }
}
