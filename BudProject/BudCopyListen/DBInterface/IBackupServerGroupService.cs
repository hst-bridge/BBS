using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudCopyListen.Entities;

namespace BudCopyListen.DBInterface
{
    public interface IBackupServerGroupService
    {
        /// <summary>
        /// グループ情報取得
        /// </summary>
        /// <returns></returns>
        backupServerGroup GetBackupServerGroup(int monitorServerID);
    }
}
