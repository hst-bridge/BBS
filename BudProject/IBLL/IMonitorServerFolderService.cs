using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IMonitorServerFolderService
    {
        /// <summary>
        /// 增加バックアップ元フォルダ
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        int InsertMonitorServerFolder(MonitorServerFolder MonitorServerFolder);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="MonitorServerFolderId"></param>
        /// <returns></returns>
        int DeleteMonitorServerFolder(int MonitorServerFolderId, string loginID);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <returns></returns>
        int UpdateMonitorServerFolder(MonitorServerFolder MonitorServerFolder);
        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <returns></returns>
        MonitorServerFolder GetMonitorServerFolderById(int MonitorServerFolderId);
        /// <summary>
        /// 根据条件获取
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<MonitorServerFolder> GetMonitorServerFolderPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetMonitorServerFolderCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// バックアップ元フォルダリスト
        /// </summary>
        /// <returns></returns>
        IList<MonitorServerFolder> GetMonitorServerFolderList();
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="MonitorServerFolderServerId"></param>
        /// <returns></returns>
        int DeleteMonitorServerFolderByServerId(int MonitorServerFolderId);
        /// <summary>
        /// 获取满足条件的バックアップ元フォルダリスト
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <returns></returns>
        IList<MonitorServerFolder> GetMonitorServerFolderByMonitorServerID(string monitorServerID);
        IList<MonitorServerFolder> GetMonitorFolderByServerIDAndInitFlg(string monitorServerID);
    }
}
