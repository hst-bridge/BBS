using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudCopyListen.DBInterface;
using BudCopyListen.DBService;
using BudCopyListen.Entities;

namespace BudCopyListen.Models
{
    public class BackupServerGroupManager
    {
        private IBackupServerGroupService backupServerGroupService = null;

        public BackupServerGroupManager() : this(new BackupServerGroupService()) { }

        public BackupServerGroupManager(IBackupServerGroupService backupServerGroupService)
        {
            this.backupServerGroupService = backupServerGroupService;
        }

        /// <summary>
        /// グループ情報取得
        /// </summary>
        /// <returns></returns>
        public backupServerGroup GetBackupServerGroup(int monitorServerID)
        {
            return this.backupServerGroupService.GetBackupServerGroup(monitorServerID);
        }
    }
}
