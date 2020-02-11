using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IMonitorBackupServerService
    {
        /// <summary>
        /// 增加バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <returns></returns>
        int InsertMonitorBackupServer(MonitorBackupServer MonitorBackupServer);
        /// <summary>
        /// 删除バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServerId"></param>
        /// <returns></returns>
        int DeleteMonitorBackupServer(int MonitorBackupServerId, string loginID);
        /// <summary>
        /// 更新バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServer"></param>
        /// <returns></returns>
        int UpdateMonitorBackupServer(MonitorBackupServer MonitorBackupServer);
        /// <summary>
        /// 根据バックアップ对象ID获取バックアップ転送サーバ関係
        /// </summary>
        /// <param name="MonitorBackupServerId"></param>
        /// <returns></returns>
        MonitorBackupServer GetMonitorBackupServerById(int MonitorServerId);
        /// <summary>
        /// 根据条件获取バックアップ転送サーバ関係
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<MonitorBackupServer> GetMonitorBackupServerPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetMonitorBackupServerCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// バックアップ転送サーバ関係リスト
        /// </summary>
        /// <returns></returns>
        IList<MonitorBackupServer> GetMonitorBackupServerList();
    }
}
