using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IManualBackupServerService
    {
        /// <summary>
        /// 增加バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int InsertManualBackupServer(ManualBackupServer ManualBackupServer);
        /// <summary>
        /// 删除バックアップ先対象
        /// </summary>
        /// <param name="BackupServerId"></param>
        /// <returns></returns>
        int DeleteManualBackupServer(int BackupServerId, string loginID);
        /// <summary>
        /// 更新バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int UpdateManualBackupServer(ManualBackupServer BackupServer);
        /// <summary>
        /// 根据ID获取バックアップ先対象
        /// </summary>
        /// <param name="BackupServerId"></param>
        /// <returns></returns>
        ManualBackupServer GetManualBackupServerById(int BackupServerId);
        /// <summary>
        /// 根据条件获取バックアップ先対象
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<ManualBackupServer> GetManualBackupServerPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetManualBackupServerCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// バックアップ先対象リスト
        /// </summary>
        /// <returns></returns>
        IList<ManualBackupServer> GetManualBackupServerList();
    }
}
