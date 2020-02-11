using System;
using System.Collections.Generic;
using System.Linq;
using BudBackupCopy.Entities;

namespace BudBackupCopy.DBInterface
{
    public interface IMonitorServerFolderService
    {
        /// <summary>
        /// バックアップ設定のキー検索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Entities.monitorServerFolder GetById(int id);

        /// <summary>
        /// 監視対象の設定全体取得
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="initFlg"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<Entities.monitorServerFolder> GetByAllMonitorObject(int monitorServerID, int initFlg, int deleteFlg);

        /// <summary>
        /// 監視対象取得
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileType"></param>
        /// <param name="initFlg"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<Entities.monitorServerFolder> GetByMonitorObject(int monitorServerID, string monitorFileType, int initFlg, int deleteFlg);

        /// <summary>
        /// バックアップ設定の検索
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorFilePath"></param>
        /// <param name="monitorFileType"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<Entities.monitorServerFolder> Search(int monitorServerID, string monitorFileName, string monitorFilePath, string monitorFileType, int deleteFlg);

                /// <summary>
        /// バックアップ設定の検索
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorFilePath"></param>
        /// <param name="monitorFileType"></param>
        /// <param name="initFlg"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<Entities.monitorServerFolder> GetByMonitorTopFolder(int monitorServerID, string monitorFileName, string monitorFilePath, string monitorFileType, string initFlg, int deleteFlg);

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        void Insert(Entities.monitorServerFolder monitorserverfolder);


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        void Edit(Entities.monitorServerFolder monitorserverfolder);

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        void Del(int id);
    }
}
