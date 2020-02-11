using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudCopyListen.Entities;

namespace BudCopyListen.DBInterface
{
    public interface IBackupServerService
    {
        /// <summary>
        /// サーバー情報取得
        /// </summary>
        /// <returns></returns>
        backupServer GetBackupServerInfo(int id);
    }
}
