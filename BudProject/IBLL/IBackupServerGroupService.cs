using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IBackupServerGroupService
    {
        /// <summary>
        /// 增加バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <returns></returns>
        int InsertBackupServerGroup(BackupServerGroup BackupServerGroup);
        /// <summary>
        /// 删除バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroupId"></param>
        /// <returns></returns>
        int DeleteBackupServerGroup(int BackupServerGroupId, string loginID);
        /// <summary>
        /// 更新バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroup"></param>
        /// <returns></returns>
        int UpdateBackupServerGroup(BackupServerGroup BackupServerGroup);
        /// <summary>
        /// 根据ID获取バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroupId"></param>
        /// <returns></returns>
        BackupServerGroup GetBackupServerGroupById(int BackupServerGroupId);
        /// <summary>
        /// 根据バックアップ对象ID获取バックアップ先対象グループ
        /// </summary>
        /// <param name="BackupServerGroupId"></param>
        /// <returns></returns>
        BackupServerGroup GetBackupServerGroupByMonitorId(int MonitorId);
        /// <summary>
        /// 根据条件获取バックアップ先対象グループ
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<BackupServerGroup> GetBackupServerGroupPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetBackupServerGroupCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// バックアップ先対象グループリスト
        /// </summary>
        /// <returns></returns>
        IList<BackupServerGroup> GetBackupServerGroupList();

        /// <summary>
        /// バックアップ先対象グループリスト
        /// </summary>
        /// <returns></returns>
        IList<BackupServerGroup> GetBackupServerGroupListAll();

        IList<BackupServerGroup> GetBackupServerGroupByName(string groupName);

        /// <summary>
        /// Get BackupServerGroup By BackupServerID
        /// </summary>
        /// 2014-06-23 wjd add
        /// <param name="backupServerID"></param>
        /// <returns></returns>
        BackupServerGroup GetBackupServerGroupByBackupServerID(int backupServerID);
    }
}
