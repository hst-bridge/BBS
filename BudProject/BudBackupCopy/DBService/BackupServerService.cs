using System;
using System.Collections.Generic;
using System.Linq;
using BudBackupCopy.DBInterface;
using System.Data;
using BudBackupCopy.Entities;

namespace BudBackupCopy.DBService
{
    public class BackupServerService : IBackupServerService
    {
        private BudBackup2Context db = null;

        public BackupServerService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// サーバー情報取得
        /// </summary>
        /// <returns></returns>
        public backupServer GetBackupServerInfo(int id)
        {
            backupServer backupServer = db.backupServers.Find(id);
            return backupServer;
        }
    }
}
