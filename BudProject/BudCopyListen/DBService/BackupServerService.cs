using System;
using System.Collections.Generic;
using System.Linq;
using BudCopyListen.DBInterface;
using System.Data;
using BudCopyListen.Entities;

namespace BudCopyListen.DBService
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
