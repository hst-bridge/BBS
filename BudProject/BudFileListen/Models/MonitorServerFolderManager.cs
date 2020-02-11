using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudFileListen.DBInterface;
using BudFileListen.DBService;
using BudFileListen.Entities;

namespace BudFileListen.Models
{
    public class MonitorServerFolderManager
    {
        private IMonitorServerFolderService monitorServerFolderService = null;

        public MonitorServerFolderManager() : this(new MonitorServerFolderService()) { }

        public MonitorServerFolderManager(IMonitorServerFolderService monitorServerFolderService)
        {
            this.monitorServerFolderService = monitorServerFolderService;
        }

        /// <summary>
        /// バックアップ設定のキー検索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Entities.monitorServerFolder GetById(int id)
        {
            return this.monitorServerFolderService.GetById(id);
        }

        /// <summary>
        /// 監視対象の設定全体取得
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="initFlg"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorServerFolder> GetByAllMonitorObject(int monitorServerID, int initFlg, int deleteFlg)
        {
            return this.monitorServerFolderService.GetByAllMonitorObject(monitorServerID, initFlg, deleteFlg);
        }

        /// <summary>
        /// 監視対象取得
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileType"></param>
        /// <param name="initFlg"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorServerFolder> GetByMonitorObject(int monitorServerID, string monitorFileType, int initFlg, int deleteFlg)
        {
            return this.monitorServerFolderService.GetByMonitorObject(monitorServerID, monitorFileType, initFlg, deleteFlg);
        }

        /// <summary>
        /// バックアップ設定の検索
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorFilePath"></param>
        /// <param name="monitorFileType"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorServerFolder> Search(int monitorServerID, string monitorFileName, string monitorFilePath, string monitorFileType, int deleteFlg)
        {
            return this.monitorServerFolderService.Search(monitorServerID, monitorFileName, monitorFilePath, monitorFileType, deleteFlg);
        }

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
        public List<Entities.monitorServerFolder> GetByMonitorTopFolder(int monitorServerID, string monitorFileName, string monitorFilePath, string monitorFileType, string initFlg, int deleteFlg)
        {
            return this.monitorServerFolderService.GetByMonitorTopFolder(monitorServerID, monitorFileName, monitorFilePath, monitorFileType, initFlg, deleteFlg);
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorServerFolder"></param>
        public void Insert(monitorServerFolder monitorServerFolder)
        {
            this.monitorServerFolderService.Insert(monitorServerFolder);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorServerFolder"></param>
        public void Edit(monitorServerFolder monitorServerFolder)
        {
            this.monitorServerFolderService.Edit(monitorServerFolder);
        }
        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="ID"></param>
        public void Del(int id)
        {
            this.monitorServerFolderService.Del(id);
        }
    }
}
