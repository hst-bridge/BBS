using System;
using System.Collections.Generic;
using System.Linq;
using BudCopyListen.DBInterface;
using System.Data;
using BudCopyListen.Entities;

namespace BudCopyListen.DBService
{
    public class BackupServerGroupDetailService : IBackupServerGroupDetailService
    {
        private BudBackup2Context db = null;

        public BackupServerGroupDetailService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// サーバリスト情報取得
        /// </summary>
        /// <returns></returns>
        public List<backupServerGroupDetail> GetBackupServerGroupDetailList(int backupServerGroupID)
        {
            return db.backupServerGroupDetails.Where(c => c.backupServerGroupID == backupServerGroupID).ToList();
        }
    }
}
