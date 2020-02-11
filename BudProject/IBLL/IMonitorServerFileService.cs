using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IMonitorServerFileService
    {
        /// <summary>
        /// 增加バックアップ元ファイル
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int InsertMonitorServerFile(MonitorServerFile MonitorServerFile, string filepath);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <returns></returns>
        int DeleteMonitorServerFile(int MonitorServerFileId, string loginID);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int UpdateMonitorServerFile(MonitorServerFile MonitorServerFile);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int UpdateTransferFlg(string MonitorServerFileId, int transferFlg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int UpdateAllTransferFlg(string MonitorFileDirectory, int transferFlg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int UpdateNGTransferFlg(string MonitorServerFileId, int transferFlg, int transferNum);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        int UpdateTransferFlgBatch(string idarray);
        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <returns></returns>
        MonitorServerFile GetMonitorServerFileById(int MonitorServerFileId);
        /// <summary>
        /// 根据条件获取
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<MonitorServerFile> GetMonitorServerFilePage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetMonitorServerFileCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// バックアップ元ファイルリスト
        /// </summary>
        /// <returns></returns>
        IList<MonitorServerFile> GetMonitorServerFileList();
        /// <summary>
        /// バックアップ元ファイルリスト
        /// </summary>
        /// <returns></returns>
        IList<MonitorServerFile> GetMonitorServerFileList(string monitorServerID);
        /// <summary>
        /// 転送対象の抽出
        /// </summary>
        /// <returns></returns>
        IList<MonitorServerFile> GetMonitorServerFileSSBPutList(string monitorServerID, string topDir);
                /// <summary>
        /// 転送対象の抽出
        /// </summary>
        /// <returns></returns>
        IList<MonitorServerFile> GetMonitorServerFileDelList(string monitorServerID);
                /// <summary>
        /// 転送対象の抽出
        /// </summary>
        /// <returns></returns>
        IList<MonitorServerFile> GetMonitorServerFileTopDirList(string monitorServerID, string topDir);
        /// <summary>
        /// 転送対象の失敗の場合、レコードの抽出
        /// </summary>
        /// <returns></returns>
        IList<MonitorServerFile> GetTransferNGFileList(string monitorServerID);
    }
}
