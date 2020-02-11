using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudFileListen.Entities;

namespace BudFileListen.DBInterface
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
