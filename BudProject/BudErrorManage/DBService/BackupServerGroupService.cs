using System;
using System.Collections.Generic;
using System.Linq;
using BudErrorManage.DBInterface;
using System.Data;
using BudErrorManage.Entities;


namespace BudErrorManage.DBService
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
