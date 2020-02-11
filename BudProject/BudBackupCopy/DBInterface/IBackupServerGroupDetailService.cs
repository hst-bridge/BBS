using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudBackupCopy.Entities;

namespace BudBackupCopy.DBInterface
{
    public interface IBackupServerGroupDetailService
    {
        /// <summary>
        /// サーバリスト情報取得
        /// </summary>
        /// <returns></returns>
        List<backupServerGroupDetail> GetBackupServerGroupDetailList(int backupServerGroupID);
    }
}
