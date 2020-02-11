using System;
using System.Collections.Generic;
using System.Linq;
using BudBackupCopy.Entities;

namespace BudBackupCopy.DBInterface
{
    public interface IMonitorFileListenService
    {
        /// <summary>
        /// バックアップ設定のキー検索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Entities.monitorFileListen GetById(int id);

        /// <summary>
        /// 監視対象全体
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<monitorFileListen> GetByAllFile(int monitorServerID, int deleteFlg);

        /// <summary>
        /// 監視対象全体
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorType"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<string> GetByAllFileName(int monitorServerID, string monitorType, int deleteFlg);

        /// <summary>
        /// 監視対象のコピー中と更新中のリスト
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<monitorFileListen> GetByCopyDoingFileName(int monitorServerID, int deleteFlg);

        /// <summary>
        /// 監視対象
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileMD5"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<Entities.monitorFileListen> GetByMonitorObject(int monitorServerID, string monitorFileMD5, int deleteFlg);

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        void Insert(Entities.monitorFileListen monitorFileListen);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        void Edit(Entities.monitorFileListen monitorFileListen);

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        void Del(int id);
    }
}
