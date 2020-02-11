using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudCopyListen.DBInterface;
using BudCopyListen.DBService;
using BudCopyListen.Entities;

namespace BudCopyListen.Models
{
    public class BackupServerManager
    {
        private IBackupServerService backupServerService = null;

        public BackupServerManager() : this(new BackupServerService()) { }

        public BackupServerManager(IBackupServerService backupServerService)
        {
            this.backupServerService = backupServerService;
        }

        /// <summary>
        /// サーバー情報取得
        /// </summary>
        /// <returns></returns>
        public backupServer GetBackupServerInfo(int id)
        {
            return this.backupServerService.GetBackupServerInfo(id);
        }
    }
}
