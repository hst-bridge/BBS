using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudFileListen.DBInterface;
using BudFileListen.DBService;
using BudFileListen.Entities;

namespace BudFileListen.Models
{
    public class MonitorFileListenManager
    {
        private IMonitorFileListenService monitorFileListenService = null;

        public MonitorFileListenManager() : this(new MonitorFileListenService()) { }

        public MonitorFileListenManager(IMonitorFileListenService monitorFileListenService)
        {
            this.monitorFileListenService = monitorFileListenService;
        }

        /// <summary>
        /// バックアップ設定のキー検索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Entities.monitorFileListen GetById(int id)
        {
            return this.monitorFileListenService.GetById(id);
        }

        /// <summary>
        /// 監視対象全体
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<monitorFileListen> GetByAllFile(int monitorServerID, int deleteFlg)
        {
            return this.monitorFileListenService.GetByAllFile(monitorServerID, deleteFlg);
        }

        /// <summary>
        /// 監視対象全体
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorType"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<string> GetByAllFileName(int monitorServerID, string monitorType, int deleteFlg)
        {
            return this.monitorFileListenService.GetByAllFileName(monitorServerID, monitorType, deleteFlg);
        }


        /// <summary>
        /// 監視対象のコピー中と更新中のリスト
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<monitorFileListen> GetByCopyDoingFileName(int monitorServerID, int deleteFlg)
        {
            return this.monitorFileListenService.GetByCopyDoingFileName(monitorServerID, deleteFlg);
        }

        /// <summary>
        /// 監視対象
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileMD5"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorFileListen> GetByMonitorObject(int monitorServerID, string monitorFileMD5, int deleteFlg)
        {
            return this.monitorFileListenService.GetByMonitorObject(monitorServerID, monitorFileMD5, deleteFlg);
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorServerFolder"></param>
        public void Insert(Entities.monitorFileListen monitorFileListen)
        {
            this.monitorFileListenService.Insert(monitorFileListen);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorServerFolder"></param>
        public void Edit(Entities.monitorFileListen monitorFileListen)
        {
            this.monitorFileListenService.Edit(monitorFileListen);
        }
        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="monitorServerID"></param>
        public void Del(int monitorServerID)
        {
            this.monitorFileListenService.Del(monitorServerID);
        }
    }
}
