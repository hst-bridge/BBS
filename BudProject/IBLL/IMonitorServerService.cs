using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IMonitorServerService
    {
        /// <summary>
        /// 增加バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        int InsertMonitorServer(MonitorServer MonitorServer);
        /// <summary>
        /// 删除バックアップ元
        /// </summary>
        /// <param name="MonitorServerId"></param>
        /// <returns></returns>
        int DeleteMonitorServer(int MonitorServerId, string loginID);
        /// <summary>
        /// 删除バックアップ元copyInit
        /// </summary>
        /// <param name="MonitorServerId"></param>
        /// <returns></returns>
        int UpdateMonitorServerCopyInit(int MonitorServerId);
        /// <summary>
        /// 更新バックアップ元
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <returns></returns>
        int UpdateMonitorServer(MonitorServer MonitorServer);
        /// <summary>
        /// 根据ID获取バックアップ元
        /// </summary>
        /// <param name="MonitorServerId"></param>
        /// <returns></returns>
        MonitorServer GetMonitorServerById(int MonitorServerId);
        /// <summary>
        /// 根据条件获取バックアップ元
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<MonitorServer> GetMonitorServerPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// バックアップ元List
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<MonitorServer> GetMonitorServerList();

        /// <summary>
        /// バックアップ元List from All DB
        /// </summary>
        /// 2014-06-04 wjd add
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<MonitorServer> GetMonitorServerListAll();

        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetMonitorServerCount(IEnumerable<SearchCondition> condition);

        /// <summary>
        /// バックアップ元名によって、対象リストを取得する
        /// </summary>
        /// <param name="msName"></param>
        /// <returns></returns>
        IList<MonitorServer> GetMonitorServerListByName(string msName);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetMaxId();
    }
}
