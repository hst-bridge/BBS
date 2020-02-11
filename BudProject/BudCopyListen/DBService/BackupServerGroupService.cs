using System;
using System.Collections.Generic;
using System.Linq;
using BudCopyListen.DBInterface;
using System.Data;
using BudCopyListen.Entities;


namespace BudCopyListen.DBService
{
    public class BackupServerGroupService :IBackupServerGroupService
    {
        private BudBackup2Context db = null;

        public BackupServerGroupService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// グループ情報取得
        /// </summary>
        /// <returns></returns>
        public backupServerGroup GetBackupServerGroup(int monitorServerID)
        {
            return db.backupServerGroups.Where(c => c.monitorServerID == monitorServerID).Single();
        }
    }
}
