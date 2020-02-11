using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudBackupCopy.DBInterface;
using BudBackupCopy.DBService;
using BudBackupCopy.Entities;

namespace BudBackupCopy.Models
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
