using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudFileListen.DBInterface;
using BudFileListen.DBService;
using BudFileListen.Entities;

namespace BudFileListen.Models
{
    public class BackupServerGroupDetailManager
    {
        private IBackupServerGroupDetailService backupServerGroupDetailService = null;

        public BackupServerGroupDetailManager() : this(new BackupServerGroupDetailService()) { }

        public BackupServerGroupDetailManager(IBackupServerGroupDetailService backupServerGroupDetailService)
        {
            this.backupServerGroupDetailService = backupServerGroupDetailService;
        }

        /// <summary>
        /// サーバリスト情報取得
        /// </summary>
        /// <returns></returns>
        public List<backupServerGroupDetail> GetBackupServerGroupDetailList(int backupServerGroupID)
        {
            return this.backupServerGroupDetailService.GetBackupServerGroupDetailList(backupServerGroupID);
        }
    }
}
